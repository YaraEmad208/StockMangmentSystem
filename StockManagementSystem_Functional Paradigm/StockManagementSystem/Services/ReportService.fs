module ReportService
open System
open DbContext
open System.Data.SqlClient
open ListFunctions
// Function to generate low-stock report
let generateLowStockReport (threshold: int) =
    // Get all products from the ProductService
    let products = ProductService.getAllProducts()

    // Filter products where quantity is below the threshold
    let lowStockProducts = 
        products 
        |> ListFilter (fun p -> p.Quantity < threshold)

    if List.isEmpty lowStockProducts then
        printfn "No products with low stock."
    else
        printfn "Low-Stock Items Report (Quantity below %d):" threshold
        printfn "----------------------------------------------------"
        lowStockProducts
        |> ListIter (fun product ->
            printfn "Product ID: %d, Name: %s, Quantity: %d, Price: %M"
                product.ProductId product.Name product.Quantity product.Price)
        printfn "----------------------------------------------------"

// Function to generate total sales report
let generateTotalSalesReport () =
    use connection = getDbConnection()
    connection.Open()

    // Query to sum the total price of completed orders
    let query = 
        """
        SELECT SUM(TotalPrice)
        FROM [Order]
        """

    use command = new SqlCommand(query, connection)
  
    let totalSales =
        let result = command.ExecuteScalar()
        if result = DBNull.Value then
            0m  
        else
            result :?> decimal
    printfn "Total Sales Report:"
    printfn "----------------------------------------------------"
    printfn "Total Sales: %M" totalSales
    printfn "----------------------------------------------------"

// Function to generate inventory value report
let generateInventoryValueReport () =
    use connection = getDbConnection()
    connection.Open()

    // Query to calculate the total value of all products in stock
    let query = 
        """
        SELECT SUM(Quantity * Price)
        FROM Product
        WHERE Quantity > 0
        """

    use command = new SqlCommand(query, connection)
    let totalValue = command.ExecuteScalar() :?> decimal

    printfn "Inventory Value Report:"
    printfn "----------------------------------------------------"
    printfn "Total Inventory Value: %M" totalValue
    printfn "----------------------------------------------------"
