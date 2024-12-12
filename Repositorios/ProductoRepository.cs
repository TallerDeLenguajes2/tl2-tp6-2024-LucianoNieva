using System;
using Microsoft.Data.Sqlite;
using producto;

namespace repoProduct
{


    public class RepoProduct
    {
        private string cadenaConexion = "Data Source=Tienda.db";
        public void CrearNuevo(Producto prod)
        {

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                var query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@Descripcion", prod.Descripcion));
                command.Parameters.Add(new SqliteParameter("@Precio", prod.Precio));
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ModificarProducto(int id, Producto prod)
        {
            var query = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @Id";

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@Descripcion", prod.Descripcion));
                command.Parameters.Add(new SqliteParameter("@Precio", prod.Precio));
                command.Parameters.Add(new SqliteParameter("@Id", id));
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public List<Producto> ListarProducto()
        {
            List<Producto> listaProd = new List<Producto>();
            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                string query = "SELECT * FROM Productos;";
                SqliteCommand command = new SqliteCommand(query, connection);
                connection.Open();
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var prod = new Producto();
                        prod.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        prod.Descripcion = reader["Descripcion"].ToString();
                        prod.Precio = Convert.ToInt32(reader["Precio"]);
                        listaProd.Add(prod);
                    }
                }
                connection.Close();

            }
            return listaProd;
        }

        public Producto ObtenerProductoPorId(int id)
        {
            using (SqliteConnection conexion = new SqliteConnection(cadenaConexion))
            {
                var consulta = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @Id";
                conexion.Open();
                var comando = new SqliteCommand(consulta, conexion);

                // Agregar el parámetro para el ID
                comando.Parameters.Add(new SqliteParameter("@Id", id));

                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        // Crear y devolver el objeto Producto con los datos obtenidos
                        return new Producto
                        {
                            IdProducto = lector.GetInt32(0),                // idProducto
                            Descripcion = lector.GetString(1),      // Descripcion
                            Precio = lector.GetInt32(2)           // Precio
                        };
                    }
                }
                conexion.Close();
            }
            return null; // Si no se encuentra el producto
        }

        public void EliminarProductoPorId(int id)
        {
            using (SqliteConnection conexion = new SqliteConnection(cadenaConexion))
            {
                var consulta = "DELETE FROM Productos WHERE idProducto = @Id";
                conexion.Open();
                var comando = new SqliteCommand(consulta, conexion);

                // Agregar el parámetro para el ID
                comando.Parameters.Add(new SqliteParameter("@Id", id));

                // Ejecutar la consulta de eliminación
                comando.ExecuteNonQuery();

                conexion.Close();
            }
        }
    }
}

