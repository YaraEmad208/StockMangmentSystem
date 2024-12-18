module AuthService
open System
open System.Data.SqlClient
open User
open DbContext

type UserService() =
    let dbContext = DbContext()

    member this.Register(user: User) =
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
        
        use connection = dbContext.GetDbConnection()
        connection.Open()
        use command = new SqlCommand(query, connection)
        parameters |> List.iter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)

        let ID = command.ExecuteScalar()
        
        if ID :? int then
            let id = ID :?> int
            printfn "User %s added successfully with ID %d!" user.Name id
            id
        else
            printfn "Failed to register user."
            -1

    member this.AuthenticateByPhone(phone: string) : int =
        let connection = dbContext.GetDbConnection()
        let query = "SELECT ID FROM [User] WHERE Phone = @Phone"
        
        try
            connection.Open()
            use command = new SqlCommand(query, connection)
            command.Parameters.AddWithValue("@Phone", phone) |> ignore
            let result = command.ExecuteScalar()
            
            if result :? int then
                let userId = result :?> int
                if userId > 0 then
                    userId
                else
                    0
            else
                0
        with ex ->
            printfn "Error: %s" ex.Message
            0

    member this.Login(email: string, password: string) =
        let query = "SELECT * FROM [User] WHERE Email = @Email AND Password = @Password AND Role = 'admin'"
        let parameters = [("@Email", box email); ("@Password", box password)]

        use connection = dbContext.GetDbConnection()
        connection.Open()
        use command = new SqlCommand(query, connection)
        parameters |> List.iter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)

        use reader = command.ExecuteReader()

        if reader.Read() then
            let user = User(
                reader.["ID"] :?> int,
                reader.["Name"] :?> string,
                reader.["Email"] :?> string,
                reader.["Password"] :?> string,
                reader.["Phone"] :?> string,
                reader.["Role"] :?> string
            )
            printfn "Login successful! Welcome, %s." user.Name
            Some user
        else
            printfn "User not found or incorrect password."
            None
