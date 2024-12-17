module OrderService
open System
open System.Data.SqlClient
open DbContext
open ProductRepository
open Product
open order
open ListFunctions

// Function to calculate the total cost of the order
let totalCost (orderItems: (int * int) list) : decimal =
    orderItems
    |> ListMap (fun (productId, quantity) -> 
        match getProductById productId with
        | Some product when product.Quantity >= quantity -> product.Price * decimal quantity
        | Some _ -> 
            printfn "Not enough stock available for Product ID: %d" productId
            0.0m
        | None -> 
            printfn "Product not found for Product ID: %d" productId
            0.0m
    )
    |> ListSum

// Function to place an order
let placeOrder (userId: int) (orderItems: (int * int) list) : int =
    use connection = getDbConnection()
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
        |> ListIter (fun (productId, quantity) -> 
            let product = getProductById productId |> Option.get
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

            outgoingStock productId quantity
        )

        transaction.Commit()
        printfn "Order placed successfully! Total Cost: %M" totalCost
        orderId

    with ex -> 
        transaction.Rollback()
        printfn "Order failed: %s" ex.Message
        0

// Function to confirm an order
let confirmOrder (orderId: int) =
    printfn "New Order Received (Order ID: %d)" orderId

    let fetchOrderQuery = 
        "SELECT OrderId, UserId, TotalPrice, OrderDate, Status FROM [Order] WHERE OrderId = @OrderId"
    let fetchOrderDetailsQuery = 
        "SELECT OrderDetailId, OrderId, ProductId, Quantity, PricePerUnit FROM OrderDetails WHERE OrderId = @OrderId"

    use connection = getDbConnection()
    connection.Open()

    let order =
        use command = new SqlCommand(fetchOrderQuery, connection)
        command.Parameters.AddWithValue("@OrderId", orderId) |> ignore
        use reader = command.ExecuteReader()
        if reader.Read() then
            { OrderId = reader.GetInt32(0)
              UserId = reader.GetInt32(1)
              TotalPrice = reader.GetDecimal(2) 
              OrderDate = reader.GetDateTime(3)
              Status = reader.GetString(4)
              }
        else
            failwith "Order not found!"

    let orderDetails =
        use command = new SqlCommand(fetchOrderDetailsQuery, connection)
        command.Parameters.AddWithValue("@OrderId", orderId) |> ignore
        use reader = command.ExecuteReader()
        [ while reader.Read() do
            yield { OrderDetailId = reader.GetInt32(0)
                    OrderId = reader.GetInt32(1)
                    ProductId = reader.GetInt32(2)
                    Quantity = reader.GetInt32(3)
                    PricePerUnit = reader.GetDecimal(4) }
        ]

    printfn "Order Summary:"
    printfn "Total Price: %M" order.TotalPrice
    orderDetails |> ListIter (fun detail -> 
        printfn "Product ID: %d, Quantity: %d, Price Per Unit: %M" detail.ProductId detail.Quantity detail.PricePerUnit
    )
