module OrderMenu
open DbContext
open System.Data.SqlClient
open User
open System
open OrderService
open AuthServiceModule
type OrderMenu(orderService: OrderService,auth:AuthService) =

    // Method to fetch user by ID

    // Method to select a user by ID
    member private this.SelectUser () =
        printfn "Enter your User ID:"
        let userId = Int32.Parse(Console.ReadLine())
        
        match auth.GetUserById (userId) with
        | Some user -> 
            printfn "User found: %s (%d)" user.Name user.ID
            user
        | None -> 
            printfn "User ID not found. Please try again."
            this.SelectUser()

    // Method to collect order items (product ID and quantity)
    member private this.CollectOrders () =
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
        collectOrders []

    // Method to process the order
    member this.ProcessOrder () =
        // Select user
        let user = this.SelectUser()

        printfn "-----------------------------------------------------------------------------------"
        printfn "\nWelcome to the Order System, %s! User ID: %d\n" user.Name user.ID
        printfn "You can now place an order. Please enter products in the format: productId,quantity."
        printfn "-----------------------------------------------------------------------------------"

        // Collect orders
        let orders = this.CollectOrders()

        if List.length orders > 0 then
            // Place the order using the OrderService
            let orderId = orderService.PlaceOrder(user.ID, orders)
            if orderId > 0 then
                printfn "\nOrder placed successfully! Order ID: %d" orderId
                // Confirm the order using the OrderService
                orderService.ConfirmOrder(orderId)
            else
                printfn "\nOrder failed. Please try again later."
        else
            printfn "\nNo items were ordered. Returning to the main menu."
