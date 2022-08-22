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

        public static List<PostVenta> CargarVentas(List<PostVenta> DetalleVenta)
        {
            DataTable dtProductos = new DataTable();
            DataTable dtUsuarios = new DataTable();
            DataRow[] singlequery;
            DataTable dtIdVenta = new DataTable();
            string query = string.Empty;
            int registros_insertados = 0;
            int stock_producto = 0;
            int cont = -1;

            //Buscamos todos los ID de Producto y Stock para no hacer un select por cada item de venta
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter SqlAdapter = new SqlDataAdapter("select Id, Stock from Producto", sqlConnection);
                sqlConnection.Open();
                SqlAdapter.Fill(dtProductos);
                sqlConnection.Close();
            }
            //Buscamos todos los ID de Usuario para no hacer un select por cada item de venta
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter SqlAdapter = new SqlDataAdapter("SELECT Id FROM Usuario", sqlConnection);
                sqlConnection.Open();
                SqlAdapter.Fill(dtUsuarios);
                sqlConnection.Close();
            }
            //Se recorren los datos de venta recibidos por la API
            foreach (var line in DetalleVenta)
            {
                cont++;

                //Validar que el ID de Producto exista y que haya stock suficiente para registrar la venta
                query = "Id = " + line.IdProducto.ToString();
                singlequery = dtProductos.Select(query);

                if (singlequery.Length == 0)
                {
                    DetalleVenta[cont].Status = "Venta no Registrada - No existe el producto";
                    continue;
                }
                else
                {
                    if (line.Stock > Convert.ToInt32(singlequery[0].ItemArray[1]))
                    {
                        DetalleVenta[cont].Status = "Venta no Registrada - No hay Stock suficiente del producto";
                        continue;
                    }
                    else
                    {
                        stock_producto = Convert.ToInt32(singlequery[0].ItemArray[1]) - line.Stock;
                    }
                }

                //Validar que el ID de Usuario exista
                query = "Id = " + line.IdUsuario.ToString();
                singlequery = dtUsuarios.Select(query);

                if (singlequery.Length == 0)
                {
                    DetalleVenta[cont].Status = "Venta no Registrada - No existe el Usuario";
                    continue;
                }

                //Insertar en la tabla Venta (Id automatico y Comentarios: DateTime + IdUsuario vendedor)
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                    {
                        string QueryUpdate = "INSERT INTO Venta ( Comentarios ) VALUES ( @Comentarios )";

                        //Parámetros
                        SqlParameter param_Comentarios = new SqlParameter("Comentarios", SqlDbType.VarChar) { Value = "Venta Registrada: " + DateTime.Now };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_Comentarios);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                        }
                        if (registros_insertados == 1)
                        {
                            //Obtener IDVenta generado
                            using (SqlConnection sqlConnection_id = new SqlConnection(ConnectionString))
                            {
                                SqlDataAdapter SqlAdapter = new SqlDataAdapter("select max(Id) from Venta", sqlConnection);
                                SqlAdapter.Fill(dtIdVenta);
                            }

                            DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + dtIdVenta.Rows[0].ItemArray[0] + " - IdUsuario: " + line.IdUsuario;
                        }
                        else
                        {
                            DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
                }

                //Insertar en la tabla Producto vendido
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                    {
                        string QueryInsert = "INSERT INTO ProductoVendido ( Stock, IdProducto, IdVenta ) VALUES ( @Stock, @IdProducto, @IdVenta )";

                        //Parámetros
                        SqlParameter param_Stock = new SqlParameter("Stock", SqlDbType.Int) { Value = line.Stock };
                        SqlParameter param_IdProducto = new SqlParameter("IdProducto", SqlDbType.Int) { Value = line.IdProducto };
                        SqlParameter param_IdVenta = new SqlParameter("IdVenta", SqlDbType.Int) { Value = dtIdVenta.Rows[0].ItemArray[0] };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryInsert, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_Stock);
                            sqlCommand.Parameters.Add(param_IdProducto);
                            sqlCommand.Parameters.Add(param_IdVenta);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                            sqlCommand.Parameters.Clear();
                        }
                        if (registros_insertados == 1)
                        {
                            DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + dtIdVenta.Rows[0].ItemArray[0] + " - IdUsuario: " + line.IdUsuario;
                        }
                        else
                        {
                            DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
                }

                //Modificar la tabla producto (Descontar stock))
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                    {
                        string QueryUpdate = "UPDATE Producto SET Stock = " + stock_producto + " WHERE Id = @IdProducto";

                        //Parámetros
                        SqlParameter param_IdProducto = new SqlParameter("IdProducto", SqlDbType.Int) { Value = line.IdProducto };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_IdProducto);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                            sqlCommand.Parameters.Clear();
                        }
                        if (registros_insertados == 1)
                        {
                            DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + dtIdVenta.Rows[0].ItemArray[0] + " - IdUsuario: " + line.IdUsuario;
                        }
                        else
                        {
                            DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
                }
            }
            return DetalleVenta;
        }
    }
}
