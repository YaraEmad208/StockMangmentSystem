module RegisterAdminModule

open System
open User
open AuthServiceModule

let private readInput prompt =
    printf "%s" prompt
    Console.ReadLine()

let registerAdmin () =
    printfn "------------ Admin Registration ------------"
    let name = readInput "Enter admin's name: "
    let email = readInput "Enter admin's email: "
    let password = readInput "Enter admin's password: "
    let phone = readInput "Enter admin's phone number: "
    let role = "Admin"

    let admin = User(0, name, email, password, phone, role)
    let authService = AuthService()

    let ID = authService.Register(admin)
    if ID > 0 then
        printfn "Admin '%s' registered successfully with ID %d." admin.Name ID
    else
        printfn "Failed to register admin '%s'." admin.Name
    printfn "--------------------------------------------"

let registerUser () =
    printfn "------------ User Registration ------------"
    let name = readInput "Enter your name: "
    let phone = readInput "Enter your phone number: "
    let role = "User"

    let user = User(0, name, "", "", phone, role)
    let authService = AuthService()

    let ID = authService.Register(user)
    if ID > 0 then
        printfn "Admin '%s' registered successfully with ID %d." user.Name ID
    else
        printfn "Failed to register admin '%s'." user.Name
    printfn "--------------------------------------------"

    
