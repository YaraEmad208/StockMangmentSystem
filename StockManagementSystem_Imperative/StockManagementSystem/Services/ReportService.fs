module ReportService
open System
open DbContext
open System.Data.SqlClient
open ListFunctions
open ProductRepository
open Product

// Define the ReportService class
type ReportService(productRepo: ProductRepository, dbContext: DbContext) =
    // Method to generate low-stock report
    member this.GenerateLowStockReport (threshold: int) =
        // Get all products from the ProductRepository
        let products = productRepo.GetAllProducts()

        // Filter products where quantity is below the threshold
        let mutable lowStockProducts = 
            products |> List.filter (fun p -> p.Quantity < threshold)

        if List.isEmpty lowStockProducts then
            printfn "No products with low stock."
        else
            printfn "Low-Stock Items Report (Quantity below %d):" threshold
            printfn "----------------------------------------------------"
            
            // Loop through each product and print its details
            for product in lowStockProducts do
                printfn "Product ID: %d, Name: %s, Quantity: %d, Price: %M"
                    product.ProductId product.Name product.Quantity product.Price
            printfn "----------------------------------------------------"

    // Method to generate total sales report
    member this.GenerateTotalSalesReport () =
        use connection = dbContext.OpenConnection()

        // Query to sum the total price of completed orders
        let query = 
            """
            SELECT SUM(TotalPrice)
            FROM [Order]
            """

        use command = new SqlCommand(query, connection)

        let mutable totalSales = 0m
        let result = command.ExecuteScalar()
        if result <> DBNull.Value then
            totalSales <- result :?> decimal

        printfn "Total Sales Report:"
        printfn "----------------------------------------------------"
        printfn "Total Sales: %M" totalSales
        printfn "----------------------------------------------------"

    // Method to generate inventory value report
    member this.GenerateInventoryValueReport () =
        use connection = dbContext.OpenConnection()

        // Query to calculate the total value of all products in stock
        let query = 
            """
            SELECT SUM(Quantity * Price)
            FROM Product
            WHERE Quantity > 0
            """

        use command = new SqlCommand(query, connection)
        let mutable totalValue = 0m

        // Execute query and handle result
        let result = command.ExecuteScalar()
        if result <> DBNull.Value then
            totalValue <- result :?> decimal

        printfn "Inventory Value Report:"
        printfn "----------------------------------------------------"
        printfn "Total Inventory Value: %M" totalValue
        printfn "----------------------------------------------------"

        let mutable totalValue = 0m

        // Execute query and handle result
        let result = command.ExecuteScalar()
        if result <> DBNull.Value then
            totalValue <- result :?> decimal

        printfn "Inventory Value Report:"
        printfn "----------------------------------------------------"
        printfn "Total Inventory Value: %M" totalValue
        printfn "----------------------------------------------------"
