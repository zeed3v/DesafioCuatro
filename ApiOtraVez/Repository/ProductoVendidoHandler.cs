using ProyectoFinal.Model;
using System.Data.SqlClient;

namespace ProyectoFinal.Repository
{
    public class ProductoVendidoHandler
    {
        public const string ConnectionString = "Server=DESKTOP-NKR56VE;Initial Catalog=SistemaGestion;Trusted_Connection=true";

        public static List<ProductoVendido> GetProductoVendido()
        {
            List<ProductoVendido> resultados = new List<ProductoVendido>();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM ProductoVendido", sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ProductoVendido productoVendido = new ProductoVendido();

                                productoVendido.Id = Convert.ToInt32(dataReader["Id"]);
                                productoVendido.Stock = Convert.ToInt32(dataReader["Id"]);
                                productoVendido.IdProducto = Convert.ToInt32(dataReader["Id"]);
                                productoVendido.IdVenta = Convert.ToInt32(dataReader["Id"]);
                                

                                resultados.Add(productoVendido);
                            }
                        }
                    }
                }
            }

            return resultados;
        }
    }
}
