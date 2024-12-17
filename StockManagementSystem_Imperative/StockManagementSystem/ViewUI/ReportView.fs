module ReportView

open ReportService

// Define the ReportView class
type ReportView(reportService: ReportService) =
    // Method to generate all reports
    member this.GenerateAllReports(threshold: int) =
        printfn "Generating reports..."
        printfn "\n"
        
        // Generate low-stock report
        reportService.GenerateLowStockReport threshold
        printfn "\n"

        // Generate total sales report
        reportService.GenerateTotalSalesReport()
        printfn "\n"

        // Generate inventory value report
        reportService.GenerateInventoryValueReport()
        printfn "\n"


