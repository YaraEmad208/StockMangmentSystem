module AuthService

open User
open DbContext
open System.Data.SqlClient
open System
open ListFunctions

let register (user: User) =
    let query = """
        INSERT INTO [User] (Name, Email, Password, Phone, Role)
        OUTPUT INSERTED.ID
        VALUES (@Name, @Email, @Password, @Phone, @Role)
    """
    let parameters = [
        ("@Name", box user.Name)
        ("@Email", box user.Email)
        ("@Password", box user.Password) 
        ("@Phone", box user.Phone)
        ("@Role", box user.Role)
    ]
    
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
    parameters |> ListIter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)
    
    // Execute and return the ID
    let ID = command.ExecuteScalar()
    match ID with
    | :? int as id ->
        printfn "User %s added successfully with ID %d!" user.Name id
        id
    | _ ->
        printfn "Failed to register user."
        -1 


let login (email: string) (password: string) =
    let query = "SELECT * FROM [User] WHERE role='admin'"
    let parameters = [("@Email", box email)]
    use connection = getDbConnection()
    connection.Open()
    use command = new SqlCommand(query, connection)
   
    parameters |> ListIter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)
    
    use reader = command.ExecuteReader()
    
    if reader.Read() then
        let storedPassword = reader.["Password"] :?> string
        if storedPassword = password then
            let user = {
                ID = reader.["ID"] :?> int
                Name = reader.["Name"] :?> string
                Email = reader.["Email"] :?> string
                Phone = reader.["Phone"] :?> string
                Role = reader.["Role"] :?> string
                Password = reader.["Password"]:?> string
            }
            printfn "Login successful! Welcome, %s." user.Name
            Some user.Name
        else
            printfn "Incorrect password."
            None
    else
        printfn "User not found."
        None



