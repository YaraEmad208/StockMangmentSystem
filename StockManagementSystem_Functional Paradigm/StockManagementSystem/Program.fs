open System
open ProductRepository
open Product
open DisplayProduct
open login
open registerAdmin
open OrderMenu
open ReportService
open ReportView
open ProductService
open OrderService
open showMainMenu
[<EntryPoint>]
let main argv =
    printfn "Welcome to the Stock Management System!"
    showMainMenu()
    0