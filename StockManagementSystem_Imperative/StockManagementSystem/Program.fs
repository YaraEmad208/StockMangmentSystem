open System
open ProductRepository
open Product
open DisplayProduct
open LoginModule
open OrderMenu
open ReportService
open ReportView
open OrderService
open showMainMenu

[<EntryPoint>]
let main argv =
    // Display a welcome message
    printfn "----------------------------------------------------"
    printfn "      Welcome to the Stock Management System!       "
    printfn "----------------------------------------------------\n"

    // Call the main menu function
    showMainMenu()

    // Exit the application
    0 // Return exit code 0
