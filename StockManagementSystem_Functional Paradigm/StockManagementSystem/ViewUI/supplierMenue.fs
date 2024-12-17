module supplierMenue
open System
open ProductService
open ProductRepository
open SupplierRepository
 let rec supplierMenu () =
    printfn "\nSupplier Menu:"
    printfn "1. Add Supplier"
    printfn "2. List Suppliers"
    printfn "3. Order Product by Supplier"
    printfn "4. List Products by Supplier"
    printfn "5. Back to Main Menu"

    match Console.ReadLine() with
    | "1" ->
        printfn "Enter supplier name:"
        let name = Console.ReadLine()
        printfn "Enter supplier contact:"
        let contact = Console.ReadLine()
        printfn "Enter supplier email:"
        let email = Console.ReadLine()
        printfn "Enter supplier address:"
        let address = Console.ReadLine()
        SupplierRepository.addSupplier name contact email address
        printfn "Supplier added successfully."
        supplierMenu()
    | "2" -> 
        printfn "\nList of Suppliers:"
        SupplierRepository.listSuppliers()
        supplierMenu()
    | "3" ->
        printfn "Enter supplier ID:"
        let supplierId = int (Console.ReadLine())
        printfn "Enter product name:"
        let name = Console.ReadLine()
        printfn "Enter product quantity:"
        let quantity = int (Console.ReadLine())
        printfn "Enter product price:"
        let price = decimal (Console.ReadLine())
        SupplierRepository.addOrUpdateProduct supplierId name quantity price |> ignore
        printfn "Product added or updated successfully for supplier."
        supplierMenu()
    | "4" ->
        printfn "Enter supplier ID:"
        let supplierId = int (Console.ReadLine())
        printfn "\nProducts by Supplier ID %d:" supplierId
        ProductRepository.listProductsBySupplier supplierId
        supplierMenu()
    | "5" -> 
        printfn "Returning to Main Menu..."
        ()

    | _ -> 
        printfn "Invalid choice, please try again."
        supplierMenu()