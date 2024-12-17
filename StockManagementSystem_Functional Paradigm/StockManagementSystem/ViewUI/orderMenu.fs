module OrderMenu
open DbContext
open System.Data.SqlClient
open User
open System
open OrderService
// Function to fetch a user by ID from the database

// Function to fetch user by ID from the database
let getUserById (Id: int) =
    // Define the SQL query to fetch the user by ID
    let query = "SELECT * FROM [User] WHERE ID = @ID"
    
    // Open a database connection using your custom getDbConnection function
    use connection = getDbConnection()
    connection.Open()

    // Create a SQL command with the query and connection
    use command = new SqlCommand(query, connection)
    command.Parameters.AddWithValue("@ID", Id) |> ignore
    
    // Execute the query and handle the result
    use reader = command.ExecuteReader()
    
    if reader.HasRows then
        reader.Read() |> ignore
        Some {
            ID = reader.GetInt32(0)
            Name = reader.GetString(1)
            Email = reader.GetString(2)
            Password = reader.GetString(3)  
            Phone = reader.GetString(4)  
            Role = reader.GetString(5)  
        }
    else
        None

let orderProcess () =
    // Select user by ID
    let rec selectUser () =
        printfn "Enter your User ID:"
        let ID = Int32.Parse(Console.ReadLine())
        
        match getUserById ID with
        | Some user -> 
            printfn "User found: %s (%d)" user.Name user.ID
            user
        | None -> 
            printfn "User ID not found. Please try again."
            selectUser()
    
    // Select user
    let user = selectUser()

    printfn "-----------------------------------------------------------------------------------"
    printfn "\nWelcome to the Order System, %s! User ID: %d\n" user.Name user.ID
    printfn "You can now place an order. Please enter products in the format: productId,quantity."
    printfn "-----------------------------------------------------------------------------------"

    // Recursive function to collect orders
    let rec collectOrders orders =
        printfn "\nEnter a product and quantity or type 'done' to finish:"
        match Console.ReadLine() with
        | "done" -> 
            printfn "\nYou have completed your order. Summary of your order:"
            orders
        | input -> 
            let parts = input.Split(',')
            if parts.Length = 2 then
                try
                    let productId = Int32.Parse(parts.[0])
                    let quantity = Int32.Parse(parts.[1])
                    printfn "Added Product ID: %d, Quantity: %d" productId quantity
                    collectOrders ((productId, quantity) :: orders)
                with
                | :? FormatException -> 
                    printfn "Invalid input. Please enter in the correct format (productId,quantity)."
                    collectOrders orders
            else
                printfn "Invalid format. Please enter in the correct format (productId,quantity)."
                collectOrders orders

    // Collect orders and place the order
    let orders = collectOrders []
    
    if orders.Length > 0 then
        // Place the order using the OrderService
        let orderId = OrderService.placeOrder user.ID orders   
        if orderId > 0 then
            printfn "\nOrder placed successfully! Order ID: %d" orderId
            // Confirm the order using the OrderService
            OrderService.confirmOrder orderId
        else
            printfn "\nOrder failed. Please try again later."
    else
        printfn "\nNo items were ordered. Returning to the main menu."
