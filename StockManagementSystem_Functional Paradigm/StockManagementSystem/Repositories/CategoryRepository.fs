module CategoryRepository

open System.Data.SqlClient
open DbContext

open Category

let getAllCategories () =
    let query = "SELECT CategoryId, CategoryName FROM Category"
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    use reader = command.ExecuteReader()
    [
        while reader.Read() do
            { CategoryId = reader.GetInt32(0); CategoryName = reader.GetString(1) }
    ]


let getCategoryById (id: int) =
    let query = "SELECT CategoryId, CategoryName FROM Category WHERE CategoryId = @Id"
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@Id", id) |> ignore
    use reader = command.ExecuteReader()
    if reader.Read() then
        Some { CategoryId = reader.GetInt32(0); CategoryName = reader.GetString(1) }
    else
        None


let addCategory (category: Category) =
    let query = "INSERT INTO Category (CategoryName) VALUES (@CategoryName)"
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@CategoryName", category.CategoryName) |> ignore
    command.ExecuteNonQuery() |> ignore


let updateCategory (category: Category) =
    let query = "UPDATE Category SET CategoryName = @CategoryName WHERE CategoryId = @CategoryId"
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@CategoryName", category.CategoryName) |> ignore
    command.Parameters.AddWithValue("@CategoryId", category.CategoryId) |> ignore
    command.ExecuteNonQuery() |> ignore

let deleteCategory (id: int) =
    let query = "DELETE FROM Category WHERE CategoryId = @CategoryId"
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@CategoryId", id) |> ignore
    command.ExecuteNonQuery() |> ignore
