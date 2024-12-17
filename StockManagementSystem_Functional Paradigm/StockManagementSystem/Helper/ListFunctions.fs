module ListFunctions

    let rec ListMap f lst =
        match lst with
        | [] -> []  
        | head :: tail -> (f head) :: ListMap f tail  

    let rec ListSum lst =
        match lst with
        | [] -> 0.0M  
        | head :: tail -> head + ListSum tail 

    let rec ListIter f lst =
        match lst with
        | [] -> () 
        | head :: tail -> 
            f head  
            ListIter f tail  

    let rec ListFilter f lst =
        match lst with
        | [] -> []  
        | head :: tail -> 
            if f head then
                head :: ListFilter f tail  
            else
                ListFilter f tail 

