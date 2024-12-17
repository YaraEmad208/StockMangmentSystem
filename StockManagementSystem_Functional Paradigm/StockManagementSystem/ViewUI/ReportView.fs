module ReportView
open ReportService

let generateAllReports threshold =
    printfn "Generating reports..."
    printfn "\n"
    
    // Generate low-stock report
    ReportService.generateLowStockReport threshold
    printfn "\n"

    // Generate total sales report
    ReportService.generateTotalSalesReport ()
    printfn "\n"

    // Generate inventory value report
    ReportService.generateInventoryValueReport ()
    printfn "\n"
