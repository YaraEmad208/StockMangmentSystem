module OrderService

open System
open System.Data.SqlClient
open DbContext
open ProductRepository

type OrderService(dbContext: DbContext, productRepo: ProductRepository) =

    let mutable isValid = true
    member this.ValidateQuantities (orderItems: (int * int) list) : bool =
        isValid <- true

        for (productId, quantity) in orderItems do
            let productOption = productRepo.GetProductById productId
            if productOption.IsSome then
                let product = productOption.Value
                if product.Quantity < quantity then
                    printfn "Insufficient stock for Product ID: %d (Requested: %d, Available: %d)" productId quantity product.Quantity
                    isValid <- false
            else
                printfn "Product not found for Product ID: %d" productId
                isValid <- false

        isValid

    member this.CalculateTotalCost (orderItems: (int * int) list) : decimal =
        let mutable totalCost = 0.0m

        for (productId, quantity) in orderItems do
            let productOption = productRepo.GetProductById productId
            if productOption.IsSome then
                let product = productOption.Value
                totalCost <- totalCost + (product.Price * decimal quantity)
            else
                printfn "Product not found for Product ID: %d" productId

        totalCost

    member this.PlaceOrder (userId: int) (orderItems: (int * int) list) : int =
        if not (this.ValidateQuantities orderItems) then
            printfn "Order validation failed due to insufficient stock or missing products."
            0 
        else 

        use connection = dbContext.GetDbConnection()
        connection.Open()

        use transaction = connection.BeginTransaction()

        try
            let totalCost = this.CalculateTotalCost orderItems

            let mutable orderId = 0
            let query = "INSERT INTO [Order] (UserId, TotalPrice, OrderDate, Status) OUTPUT INSERTED.OrderId VALUES (@UserId, @TotalPrice, GETDATE(), 'Pending')"
            use command = new SqlCommand(query, connection, transaction)
            command.Parameters.AddWithValue("@UserId", userId) |> ignore
            command.Parameters.AddWithValue("@TotalPrice", totalCost) |> ignore

            orderId <- command.ExecuteScalar() :?> int

            for (productId, quantity) in orderItems do
                let detailQuery = "INSERT INTO OrderDetails (OrderId, ProductId, Quantity, PricePerUnit) VALUES (@OrderId, @ProductId, @Quantity, (SELECT Price FROM Product WHERE ProductId = @ProductId))"
                use detailCommand = new SqlCommand(detailQuery, connection, transaction)
                detailCommand.Parameters.AddWithValue("@OrderId", orderId) |> ignore
                detailCommand.Parameters.AddWithValue("@ProductId", productId) |> ignore
                detailCommand.Parameters.AddWithValue("@Quantity", quantity) |> ignore
                detailCommand.ExecuteNonQuery() |> ignore

                let stockQuery = "UPDATE Product SET Quantity = Quantity - @Quantity WHERE ProductId = @ProductId"
                use stockCommand = new SqlCommand(stockQuery, connection, transaction)
                stockCommand.Parameters.AddWithValue("@ProductId", productId) |> ignore
                stockCommand.Parameters.AddWithValue("@Quantity", quantity) |> ignore
                stockCommand.ExecuteNonQuery() |> ignore

            transaction.Commit()
            printfn "Order placed successfully! Total Cost: %M" totalCost
            orderId

        with ex -> 
            transaction.Rollback()
            printfn "Order failed: %s" ex.Message
            0 
