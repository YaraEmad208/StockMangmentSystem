module order
open System

type Order = {
    OrderId: int
    UserId: int
    TotalPrice: decimal
    OrderDate : DateTime
    Status : string
}

type OrderDetails = {
    OrderDetailId: int
    OrderId: int
    ProductId: int
    Quantity: int
    PricePerUnit: decimal
}
