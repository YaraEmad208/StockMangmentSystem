module OrderService

open System
open System.Data.SqlClient
open DbContext
open ProductRepository
open ListFunctions
open Product
open ProductRepository

let calculateTotalCost (orderItems: (int * int) list) : decimal =
    ListMap (fun (productId, quantity) ->
        match getProductById productId with
        | Some product when product.Quantity >= quantity -> product.Price * decimal quantity
        | Some _ -> 0.0m
        | None -> 0.0m
    ) orderItems
    |>ListSum

let validateStock (orderItems: (int * int) list) : bool =
    ListFilter (fun (productId, quantity) ->
        match getProductById productId with
        | Some product when product.Quantity >= quantity -> true
        | Some _ ->
            printfn "Error: The Quantity is not avilabile in stock "
            false
        | None ->
            printfn "Error: Product not found for Product ID: %d" productId
            false
    ) orderItems
    |> fun validItems -> validItems.Length = orderItems.Length

let placeOrder (userId: int) (orderItems: (int * int) list) : int =
    if not (validateStock orderItems) then
        printfn "Order validation failed due to insufficient stock or invalid products."
        0
    else
        use connection = getDbConnection()
        connection.Open()

        use transaction = connection.BeginTransaction()

        try
            let totalCost = calculateTotalCost orderItems

            let orderId =
                let query = "INSERT INTO [Order] (UserId, TotalPrice, OrderDate, Status) OUTPUT INSERTED.OrderId VALUES (@UserId, @TotalPrice, GETDATE(), 'Pending')"
                use command = new SqlCommand(query, connection, transaction)
                command.Parameters.AddWithValue("@UserId", userId) |> ignore
                command.Parameters.AddWithValue("@TotalPrice", totalCost) |> ignore
                command.ExecuteScalar() :?> int

            ListIter (fun (productId, quantity) ->
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
            ) orderItems

            transaction.Commit()
            printfn "Order placed successfully! Total Cost: %M" totalCost
            orderId

        with ex ->
            transaction.Rollback()
            printfn "Order failed: %s" ex.Message
            0
