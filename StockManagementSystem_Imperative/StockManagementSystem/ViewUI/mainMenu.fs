module MainMenu

open DisplayProduct
open OrderMenu
open ReportView
open System
open SupplierMenu


type MainMenu(productManager: ProductManager, orderMenuInstance: OrderMenu, reportView: ReportView, supplierMenuInstance: SupplierMenu) =

    member this.DisplayMenu() =
        printfn "\nMain Menu:"
        printfn "1. Product Management"
        printfn "2. Process Orders"
        printfn "3. Reports"
        printfn "4. Supplier Management"
        printfn "5. Exit"

    member this.HandleMenuSelection() =
        let mutable exit = false
        
        while not exit do
            this.DisplayMenu()
            printf "Enter your choice: "
            let choice = Console.ReadLine()

            if choice = "1" then
                productManager.HandleMenuSelection()
            elif choice = "2" then
                orderMenuInstance.ShowMenu()
            elif choice = "3" then
                reportView.GenerateAllReports(5)
            elif choice = "4" then
                supplierMenuInstance.StartMenu()
            elif choice = "5" then
                printfn "Exiting the system. Goodbye!"
                exit <- true
            else
                printfn "Invalid choice, please try again."
