module showMainMenu
open DisplayProduct
open OrderMenu
open ReportView
open System
open SupplierMenu
open MainMenu
open LoginModule
open RegisterAdminModule

let rec showMainMenu () =

    printfn "1. Login"
    printfn "2. Register a New Admin"
    printfn "3. Exit"
    printf "Please enter your choice: "
    
    match Console.ReadLine() with
    | "1" -> 
        
        if login () then
            mainMenu()  
        else
            printfn "Login failed. Please try again."
            showMainMenu()  

    | "2" -> 
        registerAdmin() 
        showMainMenu()

    | "3" ->
        printfn "Exiting the system. Goodbye!"
        Environment.Exit(0)  

    | _ -> 
        printfn "Invalid choice. Please enter 1, 2, or 3."
        showMainMenu()
