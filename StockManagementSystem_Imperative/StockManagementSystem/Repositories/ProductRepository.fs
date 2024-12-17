module ProductRepository

open System
open System.Data.SqlClient
open Product
open DbContext

type ProductRepository(dbContext: DbContext) =

    member private this.UpdateStock(productId: int, quantity: int, isOutgoing: bool) =
        use connection = dbContext.GetDbConnection()
        connection.Open() 
        
        let query =
            if isOutgoing then
                "UPDATE Product SET Quantity = Quantity - @Quantity WHERE ProductId = @ProductId"
            else
                "UPDATE Product SET Quantity = Quantity + @Quantity WHERE ProductId = @ProductId"

        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@ProductId", productId) |> ignore
        command.Parameters.AddWithValue("@Quantity", quantity) |> ignore
        command.ExecuteNonQuery() |> ignore
        
        printfn "Stock updated for Product %d, Quantity %d, Outgoing: %b" productId quantity isOutgoing

    member this.OutgoingStock(productId: int, quantity: int) =
        this.UpdateStock(productId, quantity, true) 

    member this.IncomingStock(productId: int, quantity: int) =
        this.UpdateStock(productId, quantity, false) 

    member this.GetAllProducts() =
        let query = "SELECT ProductId, SupplierId, Name, Quantity, Price FROM Product"
        use connection = dbContext.GetDbConnection()
        connection.Open()  // Opening connection here
        
        use command = new SqlCommand(query, connection)
        use reader = command.ExecuteReader()
        
        [ while reader.Read() do
            yield {
                ProductId = reader.GetInt32(0)
                SupplierId = if reader.IsDBNull(1) then 0 else reader.GetInt32(1)
                Name = reader.GetString(2)
                Quantity = reader.GetInt32(3)
                Price = reader.GetDecimal(4)
            }
        ]

    member this.GetProductById(id: int) =
        let query = "SELECT ProductId, SupplierId, Name, Quantity, Price FROM Product WHERE ProductId = @ProductId"
        use connection = dbContext.GetDbConnection()
        connection.Open()  // Opening connection here
        
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@ProductId", id) |> ignore
        use reader = command.ExecuteReader()
        
        if reader.Read() then
            Some {
                ProductId = reader.GetInt32(0)
                SupplierId = if reader.IsDBNull(1) then 0 else reader.GetInt32(1)
                Name = reader.GetString(2)
                Quantity = reader.GetInt32(3)
                Price = reader.GetDecimal(4)
            }
        else
            None

    member this.AddProduct(product: Product) =
        let query = "INSERT INTO Product (SupplierId, Name, Quantity, Price) VALUES (@SupplierId, @Name, @Quantity, @Price)"
        use connection = dbContext.GetDbConnection()
        connection.Open()  // Opening connection here
        
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@SupplierId", product.SupplierId) |> ignore
        command.Parameters.AddWithValue("@Name", product.Name) |> ignore
        command.Parameters.AddWithValue("@Quantity", product.Quantity) |> ignore
        command.Parameters.AddWithValue("@Price", product.Price) |> ignore
        command.ExecuteNonQuery() |> ignore

    member this.UpdateProduct(product: Product) =
        let query = "UPDATE Product SET  Name = @Name, Price = @Price, Quantity = @Quantity WHERE ProductId = @ProductId"
        use connection = dbContext.GetDbConnection()
        connection.Open()  // Opening connection here
        
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@Name", product.Name) |> ignore
        command.Parameters.AddWithValue("@Price", product.Price) |> ignore
        command.Parameters.AddWithValue("@Quantity", product.Quantity) |> ignore
        command.Parameters.AddWithValue("@ProductId", product.ProductId) |> ignore
        command.ExecuteNonQuery() |> ignore

    member this.DeleteProduct(id: int) =
        let query = "DELETE FROM Product WHERE ProductId = @ProductId"
        use connection = dbContext.GetDbConnection()
        connection.Open()  // Opening connection here
        
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@ProductId", id) |> ignore
        command.ExecuteNonQuery() |> ignore
