module SupplierRepository

open DbContext
open System
open System.Data.SqlClient
open ProductRepository


let addSupplier (name: string) (contact: string) (email: string) (address: string) =
    use connection = getDbConnection()
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

let listSuppliers () =
    use connection = getDbConnection()
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


let addOrUpdateProduct (supplierId: int) (Name: string) (quantity: int) (price: decimal) =
    use connection = getDbConnection()
    connection.Open()
    
    // Check if the product exists in the database
    let checkQuery = 
        """
        SELECT ProductId, Quantity 
        FROM Product 
        WHERE Name = @Name AND SupplierId = @SupplierId
        """
    
    use checkCommand = new SqlCommand(checkQuery, connection)
    checkCommand.Parameters.AddWithValue("@Name", Name) |> ignore
    checkCommand.Parameters.AddWithValue("@SupplierId", supplierId) |> ignore
    
    use reader = checkCommand.ExecuteReader()
    
    if reader.HasRows then
        // Product exists, update the quantity
        reader.Read() |> ignore
        let productId = reader.GetInt32(0)
        let existingQuantity = reader.GetInt32(1)
        reader.Close()
        
        // Assuming incomingStock is a function that updates the product quantity
        incomingStock productId quantity
        
        printfn "Product %s already exists. Quantity updated to %d!" Name (existingQuantity + quantity)
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
        insertCommand.Parameters.AddWithValue("@Name", Name) |> ignore
        insertCommand.Parameters.AddWithValue("@Quantity", quantity) |> ignore
        insertCommand.Parameters.AddWithValue("@Price", price) |> ignore
        
        let productId = insertCommand.ExecuteScalar() :?> int
        printfn "New product %s added successfully under Supplier ID %d with Product ID %d!" Name supplierId productId
        productId
