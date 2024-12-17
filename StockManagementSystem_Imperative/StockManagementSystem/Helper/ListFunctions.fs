module ListFunctions

type ListOps() =

    member this.ListMap (f: 'T -> 'U) (lst: 'T list) : 'U list =
        let result = ref []
        for item in lst do
            result := (f item) :: !result
        List.rev !result

    member this.ListSum (lst: decimal list) : decimal =
        let mutable sum = 0.0M
        for item in lst do
            sum <- sum + item
        sum

    member this.ListIter (f: 'T -> unit) (lst: 'T list) : unit =
        for item in lst do
            f item

    member this.ListFilter (f: 'T -> bool) (lst: 'T list) : 'T list =
        let result = ref []
        for item in lst do
            if f item then
                result := item :: !result
        List.rev !result
