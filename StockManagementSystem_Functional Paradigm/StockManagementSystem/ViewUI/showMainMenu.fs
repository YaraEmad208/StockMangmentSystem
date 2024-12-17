module showMainMenu
open DisplayProduct
open OrderMenu
open ReportView
open System
open supplierMenue
open mainMenu
open login
open registerAdmin

let rec showMainMenu () =
    printfn "Choose an option:"
    printfn "1. Login"
    printfn "2. Register a new admin"
    printfn "Please enter your choice:"
    
    match Console.ReadLine() with
    | "1" -> 
        if login () then
            printfn "Login successful. Redirecting to Main Menu..."
            mainMenu() 
        else
            printfn "Login failed. Please try again."
            showMainMenu() 
    | "2" -> 
        registerAdmin() 
        printfn "Admin registered successfully. Please log in."
        showMainMenu() 
    | _ -> 
        printfn "Invalid choice, please try again."
        showMainMenu() 