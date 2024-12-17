module ProductRepository
open System.Data.SqlClient
open DbContext
open Product
open ListFunctions



let getAllProducts () =
    let query = "SELECT ProductId, SupplierId, Name, Quantity, Price FROM Product"
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    use reader = command.ExecuteReader()
    let products =
        [ while reader.Read() do
            yield {
                ProductId = reader.GetInt32(0)
                SupplierId = 
                    if reader.IsDBNull(1) then 0 else reader.GetInt32(1) // Handle nullable SupplierId
                Name = reader.GetString(2)
                Quantity = reader.GetInt32(3)
                Price = reader.GetDecimal(4) // Ensure Price is retrieved as DECIMAL
            }
        ]
    products

let getProductById id =
    let query = "SELECT ProductId, SupplierId, Name, Quantity, Price FROM Product WHERE ProductId = @ProductId"
    use connection = getDbConnection()
    connection.Open()

    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@ProductId", id) |> ignore
    use reader = command.ExecuteReader()

    if reader.Read() then
        Some {
                ProductId = reader.GetInt32(0)
                SupplierId = 
                    if reader.IsDBNull(1) then 0 else reader.GetInt32(1) // Handle nullable SupplierId
                Name = reader.GetString(2)
                Quantity = reader.GetInt32(3)
                Price = reader.GetDecimal(4) // Ensure Price is retrieved as DECIMAL
        }
    else
        None


let addProduct (product: Product) =
    let query = "INSERT INTO Product (SupplierId, Name, Quantity, Price) VALUES (@SupplierId, @Name, @Quantity, @Price)"
    let parameters = [
        ("@SupplierId", box product.SupplierId)
        ("@Name", box product.Name)
        ("@Quantity", box product.Quantity)
        ("@Price", box product.Price)
    ]
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    parameters |> ListIter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)
    command.ExecuteNonQuery() |> ignore
    product

let updateProduct (product: Product) =
    let query = "UPDATE Product SET SupplierId = @SupplierId, Name = @Name, Price = @Price, Quantity = @Quantity WHERE ProductId = @ProductId"
    let parameters = [
        ("@SupplierId", box product.SupplierId)
        ("@Name", box product.Name)
        ("@Price", box (decimal product.Price)) // Explicit cast to decimal
        ("@Quantity", box product.Quantity)
        ("@ProductId", box product.ProductId)
    ]
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    parameters |> ListIter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)
    command.ExecuteNonQuery() |> ignore

let deleteProduct id =
    let query = "DELETE FROM Product WHERE ProductId = @ProductId"
    let parameters = [("@ProductId", box id)]
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    parameters |> ListIter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)
    command.ExecuteNonQuery() |> ignore

let listProductsBySupplier (supplierId: int) =
    use connection = getDbConnection()
    connection.Open()

    let query =
        """
        SELECT ProductId,SupplierId, Name, Quantity, Price
        FROM Product
        WHERE SupplierId = @SupplierId
        """
    
    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@SupplierId", supplierId) |> ignore
    use reader = command.ExecuteReader()
    
    printfn "Products from Supplier ID %d:" supplierId
    while reader.Read() do
        let ProductId = reader.GetInt32(0)
        let SupplierId = reader.GetInt32(1)
        let name = reader.GetString(2)
        let quantity = reader.GetInt32(3)
        let price = reader.GetDecimal(4)
        printfn "Product ID: %d, Name: %s, Quantity: %d, Price: %M" ProductId name quantity price

let updateStock (productId: int) (quantity: int) (isOutgoing: bool) =
    use connection = getDbConnection()
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

let outgoingStock (productId: int) (quantity: int) =
    updateStock productId quantity true  

let incomingStock (productId: int) (quantity: int) =
    updateStock productId quantity false
