module showMainMenu
open DisplayProduct
open OrderMenu
open ReportView
open System
open SupplierMenu
open MainMenu
open LoginModule
open RegisterAdmin
open DbContext
open ProductRepository
open SupplierRepository
open OrderService
open ReportService

type LoginSystem() =

    member this.ShowLoginMenu() =
        printfn "1. Login"
        printfn "2. Register a New Admin"
        printfn "3. Exit"
        printf "Please enter your choice: "
    
    member this.HandleLogin() =
        let mutable exit = false
        
        while not exit do
            this.ShowLoginMenu()
            match Console.ReadLine() with
            | "1" -> 
                let login = Login()
                if login.Login() then
                    printfn "Login successful!"
                    this.StartMainMenu()
                else
                    printfn "Login failed. Please try again."
            | "2" -> 
                let registerService = RegisterAdminView()
                registerService.RegisterAdmin()
            | "3" ->
                printfn "Exiting the system. Goodbye!"
                exit <- true
            | _ -> 
                printfn "Invalid choice. Please enter 1, 2, or 3."
    
    member this.StartMainMenu() =
        let dbContext = new DbContext()
        let productRepo = new ProductRepository(dbContext)
        let supplierRepo = new SupplierRepository(dbContext, productRepo)
        let supplierMenuInstance = new SupplierMenu(supplierRepo)
        let productManager = new ProductManager(productRepo)
        let register = new RegisterAdminView()
        let orderService = new OrderService(productRepo = productRepo, dbContext = dbContext)
        let reportService = new ReportService(productRepo = productRepo, dbContext = dbContext)
        let reportView = new ReportView(reportService)
        let orderMenuInstance = new OrderMenu(orderService, register)
        let mainMenu = new MainMenu(productManager, orderMenuInstance, reportView, supplierMenuInstance)
        
        mainMenu.HandleMenuSelection()

