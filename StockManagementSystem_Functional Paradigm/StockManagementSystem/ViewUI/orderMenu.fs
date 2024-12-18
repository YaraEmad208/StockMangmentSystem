module OrderMenu

open DbContext
open System.Data.SqlClient
open System
open OrderService
open ProductRepository
open RegisterAdmin


let rec takeOrderItems (acc: (int * int) list) =
    printfn "Enter Product ID (or 0 to finish): "
    match Console.ReadLine() |> Int32.TryParse with
    | true, 0 -> acc 
    | true, productId ->
        printfn "Enter Quantity: "
        match Console.ReadLine() |> Int32.TryParse with
        | true, quantity when quantity > 0 -> 
            takeOrderItems ((productId, quantity) :: acc) 
        | _ -> 
            printfn "Invalid quantity. Try again."
            takeOrderItems acc
    | _ ->
        printfn "Invalid Product ID. Try again."
        takeOrderItems acc

let confirmOrder (orderId: int) =
    printfn "Order ID %d has been confirmed." orderId



let rec mainMenu () =
    printfn "\n--- Order Menu ---"
    printfn "1. Place an Order"
    printfn "2. Confirm an Order"
    printfn "3. Exit"
    printf "Select an option: "
    
    match Console.ReadLine() |> Int32.TryParse with
    | true, 1 -> 
        let userId = ensureUserExists ()
        if userId > 0 then
            let orderItems = takeOrderItems []
            if orderItems = [] then 
                printfn "No items were added to the order."
            else
                let orderId = placeOrder userId orderItems
                if orderId > 0 then
                    printfn "Order placed successfully! Order ID: %d" orderId
                else
                    printfn "Failed to place the order."
        else
            printfn "User validation failed. Cannot place the order."

        mainMenu ()
    | true, 2 -> 
        printfn "Enter Order ID to confirm: "
        match Console.ReadLine() |> Int32.TryParse with
        | true, orderId -> 
            try
                confirmOrder orderId
            with ex -> 
                printfn "Error: %s" ex.Message
        | _ -> 
            printfn "Invalid Order ID."
        mainMenu ()
    | true, 3 -> 
        printfn "Exiting the Order Menu. Goodbye!"
        ()
    | _ -> 
        printfn "Invalid option. Please try again."
        mainMenu ()

