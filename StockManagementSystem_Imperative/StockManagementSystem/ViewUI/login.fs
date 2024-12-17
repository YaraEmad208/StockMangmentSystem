module LoginModule

open System
open AuthServiceModule 

let authService = AuthService()

let login () =
    printfn "----------------------------------------------------"
    printfn "Welcome to the Login System"
    printfn "----------------------------------------------------"

    let maxAttempts = 3  
    
    match authService.LoginWithRetries(maxAttempts) with
    | Some user -> 
        printfn "----------------------------------------------------"
        printfn "Login successful! Welcome, %s" user.Name
        printfn "----------------------------------------------------"
        true
    | None -> 
        printfn "----------------------------------------------------"
        printfn "Login failed after %d attempts. Goodbye!" maxAttempts
        printfn "----------------------------------------------------"
        false
