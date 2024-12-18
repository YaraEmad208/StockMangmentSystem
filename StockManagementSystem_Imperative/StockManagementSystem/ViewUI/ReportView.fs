module ReportView
open ReportService

type ReportView(reportService: ReportService) =
    member this.GenerateAllReports(threshold: int) =
        printfn "Generating reports..."
        printfn "\n"
        
        reportService.GenerateLowStockReport threshold
        printfn "\n"

        reportService.GenerateTotalSalesReport()
        printfn "\n"

        reportService.GenerateInventoryValueReport()
        printfn "\n"


