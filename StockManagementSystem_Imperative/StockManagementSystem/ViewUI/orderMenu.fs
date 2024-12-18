module OrderMenu

open System
open DbContext
open OrderService
open ProductRepository
open RegisterAdmin
open RegisterAdmin

type OrderMenu(orderService:OrderService,register:RegisterAdminView) =
    
    let mutable userId = 0 
    
    member this.TakeOrderItems (acc: (int * int) list) =
        let rec loop (acc: (int * int) list) =
            printfn "Enter Product ID (or 0 to finish): "
            match Console.ReadLine() |> Int32.TryParse with
            | true, 0 -> acc
            | true, productId ->
                printfn "Enter Quantity: "
                match Console.ReadLine() |> Int32.TryParse with
                | true, quantity when quantity > 0 -> 
                    loop ((productId, quantity) :: acc) 
                | _ -> 
                    printfn "Invalid quantity. Try again."
                    loop acc
            | _ -> 
                printfn "Invalid Product ID. Try again."
                loop acc
        loop acc

    member this.ConfirmOrder (orderId: int) =
        printfn "Order ID %d has been confirmed." orderId

    member this.PlaceOrder (orderItems: (int * int) list) =
        let orderId = orderService.PlaceOrder userId orderItems
        if orderId > 0 then
            printfn "Order placed successfully! Order ID: %d" orderId
        else
            printfn "Failed to place the order."
    
    member this.ShowMenu () =
        let mutable exitMenu = false
        while not exitMenu do
            printfn "\n--- Order Menu ---"
            printfn "1. Place an Order"
            printfn "2. Confirm an Order"
            printfn "3. Exit"
            printf "Select an option: "
            
            match Console.ReadLine() |> Int32.TryParse with
            | true, 1 -> 
                userId <- register.EnsureUserExists() 
                if userId > 0 then
                    let orderItems = this.TakeOrderItems []
                    if orderItems = [] then 
                        printfn "No items were added to the order."
                    else
                        this.PlaceOrder orderItems
                else
                    printfn "User validation failed. Cannot place the order."
            | true, 2 -> 
                printfn "Enter Order ID to confirm: "
                match Console.ReadLine() |> Int32.TryParse with
                | true, orderId -> 
                    try
                        this.ConfirmOrder orderId
                    with ex -> 
                        printfn "Error: %s" ex.Message
                | _ -> 
                    printfn "Invalid Order ID."
            | true, 3 -> 
                printfn "Exiting the Order Menu. Goodbye!"
                exitMenu <- true
            | _ -> 
                printfn "Invalid option. Please try again."
