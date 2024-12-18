module RegisterAdmin
open System
open User
open AuthService
open DbContext

type RegisterAdminView() =
    
    member this.RegisterAdmin() =
        let mutable name = ""
        let mutable email = ""
        let mutable password = ""
        let mutable phone = ""

        while String.IsNullOrEmpty(name) do
            printfn "Enter the admin's name:"
            name <- Console.ReadLine()
        
        while String.IsNullOrEmpty(email) do
            printfn "Enter the admin's email:"
            email <- Console.ReadLine()

        while String.IsNullOrEmpty(password) do
            printfn "Enter the admin's password:"
            password <- Console.ReadLine()

        while String.IsNullOrEmpty(phone) do
            printfn "Enter the admin's phone number:"
            phone <- Console.ReadLine()

        let role = "Admin"

        let user = User(0, name, email, password, phone, role)

        let userService = UserService()
        let ID = userService.Register(user)
        
        if ID > 0 then
            printfn "Admin %s has been successfully registered with ID %d!" user.Name ID
        else
            printfn "Failed to register admin %s." user.Name

        printfn "\n----------------------------------------------------\n"

    member this.EnsureUserExists() =
        let mutable phone = ""

        while String.IsNullOrEmpty(phone) do
            printfn "Enter your phone number:"
            phone <- Console.ReadLine()

        let userService = UserService()
        let userId = userService.AuthenticateByPhone phone

        if userId = 0 then 
            printfn "User not found. Registering a new user..."
            
            let mutable name = ""
            while String.IsNullOrEmpty(name) do
                printfn "Enter your name:"
                name <- Console.ReadLine()

            let user = User(0, name, "", "", phone, "User")
            let ID = userService.Register(user)

            if ID > 0 then
                printfn "User %s has been successfully registered with ID %d!" user.Name ID
                ID
            else
                printfn "Failed to register user %s." user.Name
                0
        else
            printfn "User found with ID %d. Enter '1' to proceed." userId
            let input = Console.ReadLine()
            if input = "1" then
                userId
            else
                printfn "Invalid input. Exiting..."
                0