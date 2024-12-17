module MainMenu

open DisplayProduct
open OrderMenu
open ReportView
open System
open SupplierMenu
open SupplierRepository
open DbContext
open ProductRepository
open ReportService
open OrderService
open AuthServiceModule

let dbContext = new DbContext()
let productRepo = new ProductRepository(dbContext)
let supplierRepo = new SupplierRepository(dbContext, productRepo)
let supplierMenuInstance = SupplierMenu(supplierRepo)
let productManager = ProductManager(productRepo)

let orderService = new OrderService(productRepo = productRepo, dbContext = dbContext)
let authService = new AuthService()

let reportService = new ReportService(productRepo = productRepo, dbContext = dbContext)
let reportView = new ReportView(reportService)

let orderMenuInstance = OrderMenu(orderService, authService)

let rec mainMenu () =
    printfn "\nMain Menu:"
    printfn "1. Product Management"
    printfn "2. Process Orders"
    printfn "3. Reports"
    printfn "4. Supplier Management"
    printfn "5. Exit"
    
    printf "Enter your choice: "
    let choice = Console.ReadLine()
    
    if choice = "1" then
        productManager.HandleMenuSelection()
        mainMenu()
    elif choice = "2" then
        orderMenuInstance.ProcessOrder()
        mainMenu()
    elif choice = "3" then
        reportView.GenerateAllReports(5)
        mainMenu()
    elif choice = "4" then
        supplierMenuInstance.StartMenu()
        mainMenu()
    elif choice = "5" then
        printfn "Exiting the system. Goodbye!"
    else
        printfn "Invalid choice, please try again."
        mainMenu()

mainMenu()
