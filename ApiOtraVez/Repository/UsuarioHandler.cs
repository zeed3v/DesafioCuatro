using ProyectoFinal.Model;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoFinal.Repository
{
    public class UsuarioHandler
    {
        public const string ConnectionString = "Server=DESKTOP-NKR56VE;Initial Catalog=SistemaGestion;Trusted_Connection=true";

        public static List<Usuario> GetUsuarios()
        {
            List<Usuario> resultados = new List<Usuario>();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario", sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                Usuario usuario = new Usuario();

                                usuario.Id = Convert.ToInt32(dataReader["Id"]);
                                usuario.NombreUsuario = dataReader["NombreUsuario"].ToString();
                                usuario.Nombre = dataReader["Nombre"].ToString();
                                usuario.Apellido = dataReader["Apellido"].ToString();
                                usuario.Contraseña = dataReader["Contraseña"].ToString();
                                usuario.Mail = dataReader["Mail"].ToString();

                                resultados.Add(usuario);
                            }
                        }
                    }
                }
            }

            return resultados;
        }

        public static bool EliminarUsuario(int id)
        {
            bool resultado = false;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryDelete = "DELETE FROM Usuario WHERE Id = @id";

                SqlParameter sqlParameter = new SqlParameter("id", System.Data.SqlDbType.BigInt);
                sqlParameter.Value = id;

                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(queryDelete, sqlConnection))
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                    int numberOfRows = sqlCommand.ExecuteNonQuery();
                    if (numberOfRows > 0)
                    {
                        resultado = true;
                    }
                }

                sqlConnection.Close();
            }

            return resultado;
        }

        public static bool CrearUsuario(Usuario usuario)
        {
            bool resultado = false;
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryInsert = "INSERT INTO [SistemaGestion].[dbo].[Usuario] " +
                    "(Nombre, Apellido, NombreUsuario, Contraseña, Mail) VALUES " +
                    "(@nombreParameter, @apellidoParameter, @nombreUsuarioParameter, @contraseñaParameter, @mailParameter);";

                SqlParameter nombreParameter = new SqlParameter("nombreParameter", SqlDbType.VarChar) { Value = usuario.Nombre };
                SqlParameter apellidoParameter = new SqlParameter("apellidoParameter", SqlDbType.VarChar) { Value = usuario.Apellido };
                SqlParameter nombreUsuarioParameter = new SqlParameter("nombreUsuarioParameter", SqlDbType.VarChar) { Value = usuario.NombreUsuario };
                SqlParameter contraseñaParameter = new SqlParameter("contraseñaParameter", SqlDbType.VarChar) { Value = usuario.Contraseña };
                SqlParameter mailParameter = new SqlParameter("mailParameter", SqlDbType.VarChar) { Value = usuario.Mail };

                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(queryInsert, sqlConnection))
                {
                    sqlCommand.Parameters.Add(nombreParameter);
                    sqlCommand.Parameters.Add(apellidoParameter);
                    sqlCommand.Parameters.Add(nombreUsuarioParameter);
                    sqlCommand.Parameters.Add(contraseñaParameter);
                    sqlCommand.Parameters.Add(mailParameter);

                    int numberOfRows = sqlCommand.ExecuteNonQuery(); // Se ejecuta la sentencia sql

                    if (numberOfRows > 0)
                    {
                        resultado = true;
                    }
                }

                sqlConnection.Close();
            }

            return resultado;
        }

        public static bool ModificarUsuario(Usuario usuario)
        {
            bool resultado = false;
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryInsert = "UPDATE [SistemaGestion].[dbo].[Usuario] " +
                    "SET Nombre = @nombreParameter, " +
                    "Apellido = @apellidoParameter, " +
                    "NombreUsuario = @nombreUsuarioParameter, " +
                    "Contraseña = @contraseñaParameter, " +
                    "Mail = @mailParameter " +
                    "WHERE Id = @id ";

                SqlParameter nombreParameter = new SqlParameter("nombreParameter", SqlDbType.VarChar) { Value = usuario.Nombre };
                SqlParameter apellidoParameter = new SqlParameter("apellidoParameter", SqlDbType.VarChar) { Value = usuario.Apellido };
                SqlParameter nombreUsuarioParameter = new SqlParameter("nombreUsuarioParameter", SqlDbType.VarChar) { Value = usuario.NombreUsuario };
                SqlParameter contraseñaParameter = new SqlParameter("contraseñaParameter", SqlDbType.VarChar) { Value = usuario.Contraseña };
                SqlParameter mailParameter = new SqlParameter("mailParameter", SqlDbType.VarChar) { Value = usuario.Mail };
                SqlParameter idParameter = new SqlParameter("id", SqlDbType.BigInt) { Value = usuario.Id };

                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(queryInsert, sqlConnection))
                {
                    sqlCommand.Parameters.Add(nombreParameter);
                    sqlCommand.Parameters.Add(apellidoParameter);
                    sqlCommand.Parameters.Add(nombreUsuarioParameter);
                    sqlCommand.Parameters.Add(contraseñaParameter);
                    sqlCommand.Parameters.Add(mailParameter);
                    sqlCommand.Parameters.Add(idParameter);

                    int numberOfRows = sqlCommand.ExecuteNonQuery(); // Se ejecuta la sentencia sql

                    if (numberOfRows > 0)
                    {
                        resultado = true;
                    }
                }

                sqlConnection.Close();
            }

            return resultado;
        }
        public static Usuario IniciarSesion(string nombreUsuario, string contraseña)
        {
            Usuario resultado = new Usuario();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario " +
                    "WHERE NombreUsuario = @nombreUsuario " +
                    "AND contraseña = @contraseña ", sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("nombreUsuario", nombreUsuario));
                    sqlCommand.Parameters.Add(new SqlParameter("contraseña", contraseña));
                    sqlConnection.Open();

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                Usuario usuario = new Usuario();

                                usuario.Id = Convert.ToInt32(dataReader["Id"]);
                                usuario.NombreUsuario = dataReader["NombreUsuario"].ToString();
                                usuario.Nombre = dataReader["Nombre"].ToString();
                                usuario.Apellido = dataReader["Apellido"].ToString();
                                usuario.Contraseña = dataReader["Contraseña"].ToString();
                                usuario.Mail = dataReader["Mail"].ToString();

                                resultado = usuario;
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }

            return resultado;
        }
        public static List<Usuario> TraerUsuarioNombre(string nombreUsuario)
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario " +
                    "WHERE NombreUsuario = @nombreUsuario ", sqlConnection))
                {
                    sqlCommand.Parameters.Add(new SqlParameter("nombreUsuario", nombreUsuario));
                    sqlConnection.Open();

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                Usuario usuario = new Usuario();

                                usuario.Id = Convert.ToInt32(dataReader["Id"]);
                                usuario.NombreUsuario = dataReader["NombreUsuario"].ToString();
                                usuario.Nombre = dataReader["Nombre"].ToString();
                                usuario.Apellido = dataReader["Apellido"].ToString();
                                usuario.Contraseña = dataReader["Contraseña"].ToString();
                                usuario.Mail = dataReader["Mail"].ToString();

                                usuarios.Add(usuario);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }

            return usuarios;
        }
    }
}
