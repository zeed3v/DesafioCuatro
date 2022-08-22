using ProyectoFinal.Controllers.DTOS;
using ProyectoFinal.Model;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoFinal.Repository
{
    public class VentaHandler
    {
        public const string ConnectionString = "Server=DESKTOP-NKR56VE;Initial Catalog=SistemaGestion;Trusted_Connection=true";

        public static List<Venta> GetVentas()
        {
            List<Venta> resultados = new List<Venta>();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Venta", sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                Venta venta = new Venta();

                                venta.Id = Convert.ToInt32(dataReader["Id"]);
                                venta.Comentarios = dataReader["Comentarios"].ToString();

                                resultados.Add(venta);
                            }
                        }
                    }
                }
            }

            return resultados;
        }

        internal static List<PostVenta> CargarVentas()
        {
            throw new NotImplementedException();
        }

        public static void CargarVenta(List<Producto> productos, int IdUsuario)
        {
            PostVenta venta = new PostVenta();

            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.Connection.Open();
            sqlCommand.CommandText = @"INSERT INTO Venta
                                ([Comentarios]
                                ,[IdUsuario])
                                VALUES
                                (@Comentarios,
                                    @IdUsuario)";

            sqlCommand.Parameters.AddWithValue("@Comentarios", "");
            sqlCommand.Parameters.AddWithValue("@IdUsuario", IdUsuario);

            sqlCommand.ExecuteNonQuery(); //Se ejecuta realmente el INSERT INTO
            venta.IdUsuario = IdUsuario;

            foreach (Producto producto in productos)
            {
                sqlCommand.CommandText = @"INSERT INTO ProductoVendido
                                ([Stock]
                                ,[IdProducto]
                                ,[IdVenta])
                                VALUES
                                (@Stock,
                                @IdProducto,
                                @IdVenta)";

                sqlCommand.Parameters.AddWithValue("@Stock", producto.Stock);
                sqlCommand.Parameters.AddWithValue("@IdProducto", producto.Id);
                sqlCommand.Parameters.AddWithValue("@IdVenta", venta.Id);

                sqlCommand.ExecuteNonQuery(); //Se ejecuta realmente el INSERT INTO
                sqlCommand.Parameters.Clear();

                sqlCommand.CommandText = @" UPDATE Producto
                                                SET 
                                                Stock = Stock - @Stock
                                                WHERE id = @IdProducto";

                sqlCommand.Parameters.AddWithValue("@Stock", producto.Stock);
                sqlCommand.Parameters.AddWithValue("@IdProducto", producto.Id);

                sqlCommand.ExecuteNonQuery(); //Se ejecuta realmente el INSERT INTO
                sqlCommand.Parameters.Clear();
            }
            sqlCommand.Connection.Close();
        }

        //public static bool CargaVenta(List<Producto> productos, int IdUsuario)
        //{
        //    bool resultado = false;
        //    PostVenta venta = new PostVenta();
        //    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
        //    {
        //        string queryInsert = "INSERT INTO Venta " +
        //            "([Comentarios], [IdUsuario]) VALUES " +
        //            "(@comentarios, @idUsuario) ";

        //        SqlParameter comentarios = new SqlParameter("@comentarios", SqlDbType.VarChar) { Value = venta.Comentarios };
        //        SqlParameter idUsuario = new SqlParameter("@idUsuario", SqlDbType.Int) { Value = venta.IdUsuario };

        //        sqlConnection.Open();

        //        using (SqlCommand sqlCommand = new SqlCommand(queryInsert, sqlConnection))
        //        {
        //            sqlCommand.CommandText = "INSERT INTO ProductoVendido " +
        //            "([Stock], [IdProducto], [IdVenta]) VALUES " +
        //            "(@stock, @idProducto, @idVenta) ";

        //            SqlParameter stock = new SqlParameter("@stock", SqlDbType.Int) { Value = productos.Stock };
        //            SqlParameter idProducto = new SqlParameter("@idProducto", SqlDbType.Int) { Value = venta.Id };
        //            SqlParameter idVenta = new SqlParameter("@idVenta", SqlDbType.Int) { Value = venta.IdVenta };


        //            using (SqlCommand sqlCommand = new SqlCommand(queryInsert, sqlConnection))
        //            {
        //                sqlCommand.Parameters.Add(Stock);
        //                sqlCommand.Parameters.Add(Id);
        //                sqlCommand.Parameters.Add(precioVentaParameter);
        //                sqlCommand.Parameters.Add(stockParameter);
        //                sqlCommand.Parameters.Add(idUsuarioParameter);

        //                int numberOfRows = sqlCommand.ExecuteNonQuery(); // Se ejecuta la sentencia sql

        //                if (numberOfRows > 0)
        //                {
        //                    resultado = true;
        //                }
        //            }
        //        }

        //        sqlConnection.Close();
        //    }

        //    return resultado;
        //}
            
    }
    
}
