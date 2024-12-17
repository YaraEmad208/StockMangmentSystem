module SupplierRepository

open DbContext
open System
open System.Data.SqlClient
open ProductRepository

type SupplierRepository(dbContext: DbContext, productRepo: ProductRepository) =
    
    member this.AddSupplier(name: string, contact: string, email: string, address: string) =
        use connection = dbContext.GetDbConnection()
        connection.Open()

        let query = 
            """
            INSERT INTO Supplier (Name, Contact, Email, Address)
            VALUES (@Name, @Contact, @Email, @Address)
            """
        
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@Name", name) |> ignore
        command.Parameters.AddWithValue("@Contact", contact) |> ignore
        command.Parameters.AddWithValue("@Email", email) |> ignore
        command.Parameters.AddWithValue("@Address", address) |> ignore
        
        command.ExecuteNonQuery() |> ignore
        printfn "Supplier %s added successfully!" name

    member this.ListSuppliers() =
        use connection = dbContext.GetDbConnection()
        connection.Open()

        let query = "SELECT SupplierId, Name, Contact, Email, Address FROM Supplier"
        use command = new SqlCommand(query, connection)
        use reader = command.ExecuteReader()

        printfn "Suppliers:"
        while reader.Read() do
            let id = reader.GetInt32(0)
            let name = reader.GetString(1)
            let contact = reader.GetString(2)
            let email = reader.GetString(3)
            let address = reader.GetString(4)
            printfn "ID: %d, Name: %s, Contact: %s, Email: %s, Address: %s" id name contact email address

    member this.SupplierExists(supplierId: int) =
        use connection = dbContext.GetDbConnection()
        connection.Open()

        let query = "SELECT COUNT(1) FROM Supplier WHERE SupplierId = @SupplierId"
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@SupplierId", supplierId) |> ignore
        
        let count = command.ExecuteScalar() :?> int
        count > 0

    member this.AddOrUpdateProduct(supplierId: int, name: string, quantity: int, price: decimal) =
        use connection = dbContext.GetDbConnection()
        connection.Open()

        let checkQuery = 
            """
            SELECT ProductId, Quantity 
            FROM Product 
            WHERE Name = @Name AND SupplierId = @SupplierId
            """
        
        use checkCommand = new SqlCommand(checkQuery, connection)
        checkCommand.Parameters.AddWithValue("@Name", name) |> ignore
        checkCommand.Parameters.AddWithValue("@SupplierId", supplierId) |> ignore
        
        use reader = checkCommand.ExecuteReader()

        if reader.HasRows then
            reader.Read() |> ignore
            let productId = reader.GetInt32(0)
            let existingQuantity = reader.GetInt32(1)
            reader.Close()

            productRepo.IncomingStock(productId, quantity)
            printfn "Product %s already exists. Quantity updated to %d!" name (existingQuantity + quantity)
            productId
        else
            reader.Close()
            let insertQuery = 
                """
                INSERT INTO Product (SupplierId, Name, Quantity, Price)
                OUTPUT INSERTED.ProductId
                VALUES (@SupplierId, @Name, @Quantity, @Price)
                """
            
            use insertCommand = new SqlCommand(insertQuery, connection)
            insertCommand.Parameters.AddWithValue("@SupplierId", supplierId) |> ignore
            insertCommand.Parameters.AddWithValue("@Name", name) |> ignore
            insertCommand.Parameters.AddWithValue("@Quantity", quantity) |> ignore
            insertCommand.Parameters.AddWithValue("@Price", price) |> ignore
            
            let productId = insertCommand.ExecuteScalar() :?> int
            printfn "New product %s added successfully under Supplier ID %d with Product ID %d!" name supplierId productId
            productId

    member this.ListProductsBySupplier(supplierId: int) =
        use connection = dbContext.GetDbConnection()
        connection.Open()

        let query = 
            """
            SELECT ProductId, Name, Quantity, Price 
            FROM Product 
            WHERE SupplierId = @SupplierId
            """
        
        use command = new SqlCommand(query, connection)
        command.Parameters.AddWithValue("@SupplierId", supplierId) |> ignore
        use reader = command.ExecuteReader()

        printfn "Products for Supplier ID %d:" supplierId
        while reader.Read() do
            let productId = reader.GetInt32(0)
            let name = reader.GetString(1)
            let quantity = reader.GetInt32(2)
            let price = reader.GetDecimal(3)
            printfn "Product ID: %d, Name: %s, Quantity: %d, Price: %.2f" productId name quantity price
