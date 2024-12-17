module mainMenu
open DisplayProduct
open OrderMenu
open ReportView
open System
open supplierMenue

let rec mainMenu () =
    printfn "\nMain Menu:"
    printfn "1. Product Management"
    printfn "2. Process Orders"
    printfn "3. Reports"
    printfn "4. Supplier Management"
    printfn "5. Exit"
    
    match Console.ReadLine() with
    | "1" -> 
        handleMenuSelection() 
        mainMenu() 
    | "2" -> 
        orderProcess()
        mainMenu() 
    | "3" -> 
        generateAllReports 5 
        mainMenu() 
    | "4" -> 
        supplierMenu() 
        mainMenu() 
    | "5" -> 
        printfn "Exiting the system. Goodbye!"
        ()
    | _ -> 
        printfn "Invalid choice, please try again."
        mainMenu() 

