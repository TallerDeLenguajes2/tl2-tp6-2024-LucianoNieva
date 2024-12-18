using Microsoft.Data.Sqlite;
using clientes;
using SQLitePCL;

namespace clientesRepository
{
    public class ClientesRepository
    {
        private string cadenaConexion = "Data Source=Tienda.db";

        public void CrearCliente(Clientes clientes)
        {
            string query = @"INSERT INTO Clientes (Nombre, Email, Telefono) VALUES (@nombre, @email, @telefono)";

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@nombre", clientes.Nombre);
                command.Parameters.AddWithValue("@email", clientes.Email);
                command.Parameters.AddWithValue("@telefono", clientes.Telefono);
                command.ExecuteNonQuery();
                connection.Close();

            }
        }

        public List<Clientes> obtenerClientes()
        {

            var clientes = new List<Clientes>();

            string query = @"SELECT * FROM Clientes";

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {

                connection.Open();
                SqliteCommand command = new SqliteCommand(query, connection);

                using (SqliteDataReader leer = command.ExecuteReader())
                {

                    while (leer.Read())
                    {

                        Clientes nuevo = new Clientes();
                        nuevo.ClienteId = Convert.ToInt32(leer["ClienteId"]);
                        nuevo.Nombre = leer["Nombre"].ToString();
                        nuevo.Email = leer["Email"].ToString();
                        nuevo.Telefono = leer["Telefono"].ToString();
                        clientes.Add(nuevo);
                    }
                }
                connection.Close();
            }
            return clientes;
        }

        public void modificarCliente(int id, Clientes cliente)
        {

            string query = @"UPDATE Clientes SET Nombre = @nombre, Email = @email, Telefono = @telefono WHERE ClienteId = @id";

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@email", cliente.Email);
                command.Parameters.AddWithValue("@telefono", cliente.Telefono);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }


        public Clientes ObtenerCliente(int id)
        {
            Clientes cliente = null; //Uso el null para devolver en caso de no encontrar nada

            string query = @"SELECT * FROM Clientes WHERE ClienteId = @id ";

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cliente = new Clientes();
                        cliente.ClienteId = Convert.ToInt32(reader["ClienteId"]);
                        cliente.Nombre = reader["Nombre"].ToString();
                        cliente.Email = reader["Email"].ToString();
                        cliente.Telefono = reader["Telefono"].ToString();
                    }
                }
                connection.Close();
            }
            return cliente;
        }

        public void EliminarCliente(int id)
        {
            string query = @"DELETE FROM Clientes WHERE ClienteId = @Id;";

            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}