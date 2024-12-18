module LoginModule
open System
open AuthService

type Login() =

    member this.Login() =
        let mutable attempts = 0
        let maxAttempts = 3
        let mutable loginSuccessful = false

        while attempts < maxAttempts && not loginSuccessful do
            printfn "----------------------------------------------------"
            printfn "Welcome to the Login System"
            printfn "----------------------------------------------------"

            printf "Enter your email: "
            let email = Console.ReadLine()

            printf "Enter your password: "
            let password = Console.ReadLine()

            let userOption = UserService().Login(email, password)
            if userOption.IsSome then
                let user = userOption.Value
                printfn "----------------------------------------------------"
                printfn "Login successful! Welcome, %s" user.Name
                printfn "----------------------------------------------------"
                loginSuccessful <- true 
            else
                attempts <- attempts + 1
                printfn "----------------------------------------------------"
                printfn "Login failed. Attempt %d of %d." attempts maxAttempts
                printfn "----------------------------------------------------"

        loginSuccessful
