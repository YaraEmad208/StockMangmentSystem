module SupplierMenu

open System
open SupplierRepository

type SupplierMenu(supplierRepo: SupplierRepository) =
    let mutable running = true

    member private this.DisplayMenu() =
        printfn "\nSupplier Menu:"
        printfn "1. Add Supplier"
        printfn "2. List Suppliers"
        printfn "3. Order Product by Supplier"
        printfn "4. List Products by Supplier"
        printfn "5. Back to Main Menu"

    member private this.AddSupplier() =
        printf "Enter supplier name: "
        let name = Console.ReadLine()
        printf "Enter supplier contact: "
        let contact = Console.ReadLine()
        printf "Enter supplier email: "
        let email = Console.ReadLine()
        printf "Enter supplier address: "
        let address = Console.ReadLine()

        if not (String.IsNullOrWhiteSpace(name)) then
            supplierRepo.AddSupplier(name, contact, email, address)
            printfn "Supplier added successfully."
        else
            printfn "Error: Supplier name cannot be empty."

   
    member private this.ListSuppliers() =
        supplierRepo.ListSuppliers()

    member private this.OrderProduct() =
        printf "Enter supplier ID: "
        let supplierId = Console.ReadLine() |> Int32.Parse
        if supplierRepo.SupplierExists(supplierId) then
            printf "Enter product name: "
            let name = Console.ReadLine()
            printf "Enter quantity: "
            let quantity = Console.ReadLine() |> Int32.Parse
            printf "Enter price: "
            let price = Console.ReadLine() |> Decimal.Parse

            supplierRepo.AddOrUpdateProduct(supplierId, name, quantity, price) |> ignore
            printfn "Product ordered successfully."
        else
            printfn "Supplier ID %d does not exist." supplierId

    member private this.ListProductsBySupplier() =
        printf "Enter supplier ID: "
        let supplierId = Console.ReadLine() |> Int32.Parse
        if supplierRepo.SupplierExists(supplierId) then
            supplierRepo.ListProductsBySupplier(supplierId)
        else
            printfn "Supplier ID %d does not exist." supplierId


    member this.StartMenu() =
        while running do
            this.DisplayMenu()
            printf "Enter your choice: "
            let choice = Console.ReadLine()
            if choice = "1" then
                this.AddSupplier()
            elif choice = "2" then
                this.ListSuppliers()
            elif choice = "3" then
                this.OrderProduct()
            elif choice = "4" then
                this.ListProductsBySupplier()
            elif choice = "5" then
                printfn "Returning to Main Menu..."
                running <- false
            else
                printfn "Invalid choice, please try again."

        printfn "Exiting Supplier Menu..."
