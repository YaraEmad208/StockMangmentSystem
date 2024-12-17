module DisplayProduct

open System
open ProductService
open ProductRepository
open Product
open ListFunctions


// Display the product management menu
let showMenu () =
    printfn "----- Product Management -----"
    printfn "1. Add Product"
    printfn "2. Update Product"
    printfn "3. Delete Product"
    printfn "4. View All Products"
    printfn "5. Exit"
    printfn "\n"
    printf "Enter your choice: "
    printfn "\n"

// Handle the add product menu
let addProductMenu () =
    printfn "Enter product details:"
    printf "SupplierId: "
    let supplierId = Int32.Parse(Console.ReadLine())
    printf "Name: "
    let name = Console.ReadLine()
    printf "Price: "
    let price = Decimal.Parse(Console.ReadLine())
    printf "Quantity: "
    let quantity = Int32.Parse(Console.ReadLine())

    let product = { ProductId = 0;SupplierId= supplierId; Name = name; Price = price; Quantity = quantity }
    let addedProduct = ProductRepository.addProduct product
    printfn "Added Product: %A" addedProduct

// Handle the update product menu
let updateProductMenu () =
    printfn "Enter the Product ID to update:"
    let productId = Int32.Parse(Console.ReadLine())
    
    match ProductRepository.getProductById productId with
    | Some product ->
        printfn "Updating product: %s" product.Name
        printf "New Name (press enter to keep current): "
        let name = Console.ReadLine()
        let newName = if String.IsNullOrWhiteSpace(name) then product.Name else name

        // Handle Price input safely
        printf "New Price (press enter to keep current): "
        let priceStr = Console.ReadLine()
        let newPrice = 
            if String.IsNullOrWhiteSpace(priceStr) then 
                product.Price 
            else 
                match Decimal.TryParse(priceStr) with
                | (true, parsedPrice) -> parsedPrice
                | (false, _) -> 
                    printfn "Invalid price entered. Keeping current price."
                    product.Price

        // Handle Quantity input safely
        printf "New Quantity (press enter to keep current): "
        let quantityStr = Console.ReadLine()
        let newQuantity = 
            if String.IsNullOrWhiteSpace(quantityStr) then 
                product.Quantity 
            else 
                match Int32.TryParse(quantityStr) with
                | (true, parsedQuantity) -> parsedQuantity
                | (false, _) -> 
                    printfn "Invalid quantity entered. Keeping current quantity."
                    product.Quantity

        // Update and save the product
        let updatedProduct = { product with Name = newName; Price = newPrice; Quantity = newQuantity }
        ProductRepository.updateProduct updatedProduct
        printfn "Updated Product: %A" updatedProduct
    | None -> 
        printfn "Product with ID %d not found!" productId

// Handle the delete product menu
let deleteProductMenu () =
    printfn "Enter the Product ID to delete:"
    let productId = Int32.Parse(Console.ReadLine())
    
    match ProductService.getProductById productId with
    | Some product ->
        ProductService.deleteProduct productId
        printfn "Deleted Product: %A" product
    | None -> 
        printfn "Product with ID %d not found!" productId

// View all products
let viewAllProducts () =
    printfn "\n----------------------- ALL PRODUCTS ------------------------------\n"
    let products = ProductService.getAllProducts ()
    
    if products.Length > 0 then
        products |> ListIter (fun p -> 
            printfn "Product: %d - %s - Quantity: %d - Price: %.2f" p.ProductId p.Name p.Quantity p.Price
        )
        printfn "\n-----------------------------------------------------"
    else
        printfn "No products found."

// Main function to handle the menu selection
let rec handleMenuSelection () =
    showMenu ()
    match Console.ReadLine() with
    | "1" -> 
        addProductMenu ()
        handleMenuSelection ()  // Recurse back to menu after operation
    | "2" -> 
        updateProductMenu ()
        handleMenuSelection ()  // Recurse back to menu after operation
    | "3" -> 
        deleteProductMenu ()
        handleMenuSelection ()  // Recurse back to menu after operation
    | "4" -> 
        viewAllProducts ()
        handleMenuSelection ()  // Recurse back to menu after operation
    | "5" -> 
        printfn "Exiting Product Management."
        ()   // Exit the function (end the program)
    | _ -> 
        printfn "Invalid choice, please try again."
        handleMenuSelection ()  // Recurse back to menu for invalid input
