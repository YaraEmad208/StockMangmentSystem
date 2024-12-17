module ProductService

open ProductRepository

let getAllProducts () =
    ProductRepository.getAllProducts ()

let getProductById id =
    ProductRepository.getProductById id

let addProduct product =
    ProductRepository.addProduct product

let updateProduct product =
    ProductRepository.updateProduct product

let deleteProduct id =
    ProductRepository.deleteProduct id
