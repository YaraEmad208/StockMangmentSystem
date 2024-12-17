module OrderService
open System
open System.Data.SqlClient
open DbContext
open ProductRepository
open Product
open order

type OrderService(dbContext: DbContext, productRepo: ProductRepository) =

    let totalCost (orderItems: (int * int) list) : decimal =
        orderItems
        |> List.map (fun (productId, quantity) -> 
            match productRepo.GetProductById(productId) with
            | Some product when product.Quantity >= quantity -> product.Price * decimal quantity
            | Some _ -> 
                printfn "Not enough stock available for Product ID: %d" productId
                0.0m
            | None -> 
                printfn "Product not found for Product ID: %d" productId
                0.0m
        )
        |> List.sum

    member this.PlaceOrder(userId: int, orderItems: (int * int) list) : int =
        use connection = dbContext.GetDbConnection()
        connection.Open()

        let totalCost = totalCost orderItems

        use transaction = connection.BeginTransaction()

        try
            let insertOrderQuery = 
                "INSERT INTO [Order] (UserId, TotalPrice, OrderDate, Status) OUTPUT INSERTED.OrderId VALUES (@UserId, @TotalPrice, GETDATE(), 'Pending')"
            
            let orderId = 
                use command = new SqlCommand(insertOrderQuery, connection, transaction)
                command.Parameters.AddWithValue("@UserId", userId) |> ignore
                command.Parameters.AddWithValue("@TotalPrice", totalCost) |> ignore
                command.ExecuteScalar() :?> int

            orderItems
            |> List.iter (fun (productId, quantity) -> 
                let product = productRepo.GetProductById(productId) |> Option.get
                let pricePerUnit = product.Price

                let insertDetailsQuery = 
                    "INSERT INTO OrderDetails (OrderId, ProductId, Quantity, PricePerUnit) VALUES (@OrderId, @ProductId, @Quantity, @PricePerUnit)"
                let updateStockQuery = 
                    "UPDATE Product SET Quantity = Quantity - @Quantity WHERE ProductId = @ProductId"

                use insertDetailsCommand = new SqlCommand(insertDetailsQuery, connection, transaction)
                insertDetailsCommand.Parameters.AddWithValue("@OrderId", orderId) |> ignore
                insertDetailsCommand.Parameters.AddWithValue("@ProductId", productId) |> ignore
                insertDetailsCommand.Parameters.AddWithValue("@Quantity", quantity) |> ignore
                insertDetailsCommand.Parameters.AddWithValue("@PricePerUnit", pricePerUnit) |> ignore
                insertDetailsCommand.ExecuteNonQuery() |> ignore

                use updateStockCommand = new SqlCommand(updateStockQuery, connection, transaction)
                updateStockCommand.Parameters.AddWithValue("@ProductId", productId) |> ignore
                updateStockCommand.Parameters.AddWithValue("@Quantity", quantity) |> ignore
                updateStockCommand.ExecuteNonQuery() |> ignore

                productRepo.OutgoingStock(productId, quantity)
            )

            transaction.Commit()
            printfn "Order placed successfully! Total Cost: %M" totalCost
            orderId

        with ex -> 
            
            transaction.Rollback()
            printfn "Order failed: %s" ex.Message
            0

    member this.ConfirmOrder(orderId: int) =
        printfn "New Order Received (Order ID: %d)" orderId

        let fetchOrderQuery = 
            "SELECT OrderId, UserId, TotalPrice, OrderDate, Status FROM [Order] WHERE OrderId = @OrderId"
        let fetchOrderDetailsQuery = 
            "SELECT OrderDetailId, OrderId, ProductId, Quantity, PricePerUnit FROM OrderDetails WHERE OrderId = @OrderId"

        use connection = dbContext.GetDbConnection()
        connection.Open()

        let order =
            use command = new SqlCommand(fetchOrderQuery, connection)
            command.Parameters.AddWithValue("@OrderId", orderId) |> ignore
            use reader = command.ExecuteReader()
            if reader.Read() then
                // Map the data to the Order class
                let orderId = reader.GetInt32(0)
                let userId = reader.GetInt32(1)
                let totalPrice = reader.GetDecimal(2)
                // Create an Order instance
                new Order(orderId, userId, totalPrice)
            else
                failwith "Order not found!"

        // Fetch order details and map to the OrderDetails class
        let orderDetails =
            use command = new SqlCommand(fetchOrderDetailsQuery, connection)
            command.Parameters.AddWithValue("@OrderId", orderId) |> ignore
            use reader = command.ExecuteReader()
            [ while reader.Read() do
                let orderDetailId = reader.GetInt32(0)
                let orderId = reader.GetInt32(1)
                let productId = reader.GetInt32(2)
                let quantity = reader.GetInt32(3)
                let pricePerUnit = reader.GetDecimal(4)
                // Create an OrderDetails instance for each record
                yield new OrderDetails(orderDetailId, orderId, productId, quantity, pricePerUnit)
            ]

        // Print the order details
        printfn "Order Summary:"
        printfn "Total Price: %M" order.TotalPrice
        orderDetails |> List.iter (fun detail -> 
            printfn "Product ID: %d, Quantity: %d, Price Per Unit: %M" detail.ProductId detail.Quantity detail.PricePerUnit
        )
