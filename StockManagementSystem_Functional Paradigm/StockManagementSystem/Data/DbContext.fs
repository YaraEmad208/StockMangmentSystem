module DbContext
open System
open System.Data.SqlClient

let connectionString = "Server=DESKTOP-V0VDHTV\SQLEXPRESS; Database=StockMangmentSystem; Trusted_Connection=true;"
let getDbConnection () = new SqlConnection(connectionString)
