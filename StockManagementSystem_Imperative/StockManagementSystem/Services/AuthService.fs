module AuthServiceModule

open System
open System.Data.SqlClient
open User
open DbContext

type AuthService() =

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

        use connection = dbContext.OpenConnection()
        use command = new SqlCommand(query, connection)

        parameters |> List.iter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)

        let ID = command.ExecuteScalar()
        match ID with
        | :? int as id ->
            printfn "User %s added successfully with ID %d!" user.Name id
            id
        | _ ->
            printfn "Failed to register user."
            -1

    // Login a user with retry attempts
    member this.LoginWithRetries(maxAttempts: int) =
        let mutable attempts = 0
        let mutable isLoggedIn = false
        let mutable loggedInUser: User option = None

        while not isLoggedIn && attempts < maxAttempts do
            printf "Enter Email: "
            let email = Console.ReadLine()

            printf "Enter Password: "
            let password = Console.ReadLine()

            let query = "SELECT * FROM [User] WHERE Email = @Email"
            let parameters = [("@Email", box email)]

            use connection = dbContext.OpenConnection()
            use command = new SqlCommand(query, connection)

            // Add parameters
            parameters |> List.iter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)

            use reader = command.ExecuteReader()

            if reader.Read() then
                let storedPassword = reader.["Password"] :?> string
                if storedPassword = password then
                    let user = User(
                        reader.["ID"] :?> int,
                        reader.["Name"] :?> string,
                        reader.["Email"] :?> string,
                        reader.["Password"] :?> string,
                        reader.["Phone"] :?> string,
                        reader.["Role"] :?> string
                    )
                    printfn "Login successful! Welcome, %s." user.Name
                    loggedInUser <- Some user
                    isLoggedIn <- true
                else
                    printfn "Incorrect password. Please try again."
            else
                printfn "User not found. Please try again."

            attempts <- attempts + 1

            // Check for retry attempts
            if not isLoggedIn && attempts < maxAttempts then
                printfn "Attempt %d of %d failed." attempts maxAttempts
            elif attempts = maxAttempts then
                printfn "Maximum login attempts reached. Exiting login process."

        loggedInUser

    member this.GetUserById(userId: int) =
        let query = "SELECT * FROM [User] WHERE ID = @ID"
        let parameters = [("@ID", box userId)]

        use connection = dbContext.OpenConnection()
        use command = new SqlCommand(query, connection)

        // Add parameters
        parameters |> List.iter (fun (param, value) -> command.Parameters.AddWithValue(param, value) |> ignore)

        use reader = command.ExecuteReader()

        if reader.Read() then
            Some(
                User(
                    reader.["ID"] :?> int,
                    reader.["Name"] :?> string,
                    reader.["Email"] :?> string,
                    reader.["Password"] :?> string,
                    reader.["Phone"] :?> string,
                    reader.["Role"] :?> string
                )
            )
        else
            None
