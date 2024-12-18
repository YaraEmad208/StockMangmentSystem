module RegisterAdmin
open System
open User
open AuthService
open DbContext
open System.Data.SqlClient

let registerAdmin () =
    printfn "Enter the admin's name:"
    let name = Console.ReadLine()

    printfn "Enter the admin's email:"
    let email = Console.ReadLine()

    printfn "Enter the admin's password:"
    let password = Console.ReadLine()

    printfn "Enter the admin's phone number:"
    let phone = Console.ReadLine()

    let role = "Admin"

    let user = { ID = 0; Name = name; Email = email; Password = password; Phone = phone; Role = role }

    let ID = register user
    if ID > 0 then
        printfn "Admin %s has been successfully registered with ID %d!" user.Name ID
    else
        printfn "Failed to register admin %s." user.Name

    printfn "\n----------------------------------------------------\n"


let ensureUserExists () =
    printfn "Enter your phone number:"
    let phone = Console.ReadLine()

    match authenticateByPhone phone with
    | 0 -> 
        printfn "User not found. Registering a new user..."
        printfn "Enter your name:"
        let name = Console.ReadLine()

        let user = { ID = 0; Name = name; Email = ""; Password = ""; Phone = phone; Role = "User" }
        let ID = register user

        if ID > 0 then
            printfn "User %s has been successfully registered with ID %d!" user.Name ID
            ID
        else
            printfn "Failed to register user %s." user.Name
            0
    | userId -> 
        printfn "User found with ID %d. Enter '1' to proceed." userId
        let input = Console.ReadLine()
        if input = "1" then
            userId 
        else
            printfn "Invalid input. Exiting..."
            0
