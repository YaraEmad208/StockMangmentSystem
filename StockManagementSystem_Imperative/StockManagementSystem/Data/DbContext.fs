module DbContext

open System
open System.Data.SqlClient

type DbContext() =

    let connectionString = "Server=LAPTOP-MLVV73RA; Database=StockMangmentSystem; Trusted_Connection=true;"

    member this.GetDbConnection() : SqlConnection =
        new SqlConnection(connectionString)

    member this.OpenConnection() : SqlConnection =
        let connection = this.GetDbConnection()
        connection.Open() |> ignore
        connection

    member this.CloseConnection(connection: SqlConnection) =
        if connection.State = System.Data.ConnectionState.Open then
            connection.Close()
