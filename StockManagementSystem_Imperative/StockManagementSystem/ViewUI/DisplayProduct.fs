module DisplayProduct

open DbContext
open System
open ProductRepository
open Product



// Menu display function
let showMenu () =
    printfn "\n----- Product Management -----"
    printfn "1. Add Product"
    printfn "2. Update Product"
    printfn "3. Delete Product"
    printfn "4. View All Products"
    printfn "5. Exit"
    printf "\nEnter your choice: "

// ProductManager Class
type ProductManager(productRepo: ProductRepository) =
    // Add Product
    member this.AddProduct() =
        printfn "\n--- Add New Product ---"
        printf "SupplierId: "
        let supplierId = Int32.Parse(Console.ReadLine())
        printf "Name: "
        let name = Console.ReadLine()
        printf "Price: "
        let price = Decimal.Parse(Console.ReadLine())
        printf "Quantity: "
        let quantity = Int32.Parse(Console.ReadLine())

        if not (String.IsNullOrWhiteSpace(name)) then
            let product = { ProductId = 0; SupplierId = supplierId; Name = name; Price = price; Quantity = quantity }
            productRepo.AddProduct(product)
            printfn "Product added successfully."
        else
            printfn "Error: Product name cannot be empty."

    // Update Product
    member this.UpdateProduct() =
        printfn "\n--- Update Product ---"
        printf "Enter the Product ID to update: "
        let productId = Int32.Parse(Console.ReadLine())

        if productRepo.GetProductById(productId).IsSome then
            let existingProduct = productRepo.GetProductById(productId).Value

            printf "New Name (press enter to keep current - %s): " existingProduct.Name
            let name = Console.ReadLine()
            let newName = if String.IsNullOrWhiteSpace(name) then existingProduct.Name else name

            printf "New Price (press enter to keep current - %.2f): " existingProduct.Price
            let priceStr = Console.ReadLine()
            let newPrice = if String.IsNullOrWhiteSpace(priceStr) then existingProduct.Price else Decimal.Parse(priceStr)

            printf "New Quantity (press enter to keep current - %d): " existingProduct.Quantity
            let quantityStr = Console.ReadLine()
            let newQuantity = if String.IsNullOrWhiteSpace(quantityStr) then existingProduct.Quantity else Int32.Parse(quantityStr)

            let updatedProduct = { existingProduct with Name = newName; Price = newPrice; Quantity = newQuantity }
            productRepo.UpdateProduct(updatedProduct)
            printfn "Product updated successfully."
        else
            printfn "Error: Product with ID %d not found." productId

    // Delete Product
    member this.DeleteProduct() =
        printfn "\n--- Delete Product ---"
        printf "Enter the Product ID to delete: "
        let productId = Int32.Parse(Console.ReadLine())

        if productRepo.GetProductById(productId).IsSome then
            productRepo.DeleteProduct(productId)
            printfn "Product deleted successfully."
        else
            printfn "Error: Product with ID %d not found." productId

    // View All Products
    member this.ViewAllProducts() =
        let products = productRepo.GetAllProducts()
        printfn "\n--- List of All Products ---"
        if products.Length > 0 then
            products
            |> List.iter (fun p -> 
                printfn "ID: %d | Name: %s | Quantity: %d | Price: %.2f" p.ProductId p.Name p.Quantity p.Price)
        else
            printfn "No products available."
        printfn "--------------------------------"

    // Handle Menu Selection
    member this.HandleMenuSelection() =
        let mutable isRunning = true

        while isRunning do
            showMenu()
            let choice = Console.ReadLine()

            if choice = "1" then
                this.AddProduct()
            elif choice = "2" then
                this.UpdateProduct()
            elif choice = "3" then
                this.DeleteProduct()
            elif choice = "4" then
                this.ViewAllProducts()
            elif choice = "5" then
                printfn "Exiting Product Management."
                isRunning <- false
            else
                printfn "Invalid choice, please try again."
