module User




type User(id: int, name: string, email: string, password: string, phone: string, role: string) =

    member val ID = id with get, set
    member val Name = name with get, set
    member val Email = email with get, set
    member val Password = password with get, set
    member val Phone = phone with get, set
    member val Role = role with get, set