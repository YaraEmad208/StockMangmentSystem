module login
open System

let login () =
    printfn "----------------------------------------------------"
    printfn "Welcome to the Login System"
    printfn "----------------------------------------------------"
    
    printfn "Please enter your email:"
    let email = Console.ReadLine()

    printfn "Please enter your password:"
    let password = Console.ReadLine()

    match AuthService.login email password with
    | Some userName -> 
       
        printfn "----------------------------------------------------"
        printfn "Login successful! Welcome, %s" userName
        printfn "----------------------------------------------------"
        printfn "\n"
        true 
    | None -> 
       
        printfn "----------------------------------------------------"
        printfn "Not an Admin or Invalid email or Invalid password. Please try again."
        printfn "----------------------------------------------------"
        printfn "\n"
        false  
