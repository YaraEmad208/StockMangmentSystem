module order
open System

type Order(orderId: int, userId: int, totalPrice: decimal) =
   
    member val OrderId = orderId with get, set
    member val UserId = userId with get, set
    member val TotalPrice = totalPrice with get, set

type OrderDetails(orderDetailId: int, orderId: int, productId: int, quantity: int, pricePerUnit: decimal) =

    member val OrderDetailId = orderDetailId with get, set
    member val OrderId = orderId with get, set
    member val ProductId = productId with get, set
    member val Quantity = quantity with get, set
    member val PricePerUnit = pricePerUnit with get, set