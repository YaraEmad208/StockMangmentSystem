module login
open System

let login () =
    // Display the prompt for user email
    printfn "----------------------------------------------------"
    printfn "Welcome to the Login System"
    printfn "----------------------------------------------------"
    
    printfn "Please enter your email:"
    let email = Console.ReadLine()

    // Display the prompt for user password
    printfn "Please enter your password:"
    let password = Console.ReadLine()

    // Attempt login and handle the result
    match AuthService.login email password with
    | Some userName -> 
        // Successful login
        printfn "----------------------------------------------------"
        printfn "Login successful! Welcome, %s" userName
        printfn "----------------------------------------------------"
        printfn "\n"
        true  // Indicating successful login
    | None -> 
        // Failed login
        printfn "----------------------------------------------------"
        printfn "Not an Admin or Invalid email or Invalid password. Please try again."
        printfn "----------------------------------------------------"
        printfn "\n"
        false  // Indicating failed login
