using clientes;
using Microsoft.Data.Sqlite;
using presupuestos;
using presupuestosDetalles;
using producto;

namespace presupuestosRepository;
public class PresupuestosRepository
{
    private string cadenaConexion = "Data Source=Tienda.db";

    // Método para crear un nuevo presupuesto
    public void CrearPre(Presupuestos presupuesto)
    {
        if (presupuesto == null)
            throw new ArgumentNullException(nameof(presupuesto), "El presupuesto no puede ser nulo.");

        // Validar el presupuesto y asignar valores por defecto si es necesario

        string query1 = @"INSERT INTO Presupuestos (FechaCreacion, ClienteId) VALUES (@fechaCre, @idCli)";

        using (var connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();

            using (var command = new SqliteCommand(query1, connection))
            {
                command.Parameters.AddWithValue("@fechaCre", presupuesto.FechaCreacion);
                command.Parameters.AddWithValue("@idCli", presupuesto.Cliente.ClienteId);
                command.ExecuteNonQuery();
            }

            var GetId = "SELECT last_insert_rowid()";
            using (SqliteCommand getIdCommand = new SqliteCommand(GetId, connection))
            {
                presupuesto.IdPresupuesto = Convert.ToInt32(getIdCommand.ExecuteScalar());
            }

            if (presupuesto.Detalle != null && presupuesto.Detalle.Count > 0)
            {
                foreach (var item in presupuesto.Detalle)
                {
                    if (item.Producto == null)
                        throw new InvalidOperationException("El producto en el detalle no puede ser nulo.");

                    string query2 = @"
                    INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) 
                    VALUES (@idPre, @idProdu, @canti)";

                    using (var detalleCommand = new SqliteCommand(query2, connection))
                    {
                        detalleCommand.Parameters.AddWithValue("@idPre", presupuesto.IdPresupuesto);
                        detalleCommand.Parameters.AddWithValue("@idProdu", item.Producto.IdProducto);
                        detalleCommand.Parameters.AddWithValue("@canti", item.Cantidad);

                        detalleCommand.ExecuteNonQuery();
                    }
                }
            }

            connection.Close();
        }
    }


    public List<PresupuestosDetalles> ObtenerDetalle(int id)
    {
        string query = @"SELECT p.idProducto, p.Descripcion, p.Precio, pd.Cantidad 
                         FROM Productos AS p
                         INNER JOIN PresupuestosDetalle AS pd USING (idProducto)
                         WHERE pd.idPresupuesto = @idquery";

        List<PresupuestosDetalles> lista = new List<PresupuestosDetalles>();

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@idquery", id));

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PresupuestosDetalles Pd = new PresupuestosDetalles();
                        Producto nuevoProducto = new Producto();

                        nuevoProducto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        nuevoProducto.Descripcion = Convert.ToString(reader["Descripcion"]);
                        nuevoProducto.Precio = Convert.ToInt32(reader["Precio"]);
                        Pd.Cantidad = Convert.ToInt32(reader["Cantidad"]);

                        Pd.Producto = nuevoProducto;

                        lista.Add(Pd);
                    }
                }
            }
        }
        return lista;
    }

    // Método para listar todos los presupuestos
    public List<Presupuestos> ListarPresupuestos()
    {
        string queryDetalle = @"SELECT * FROM Presupuestos LEFT JOIN Clientes USING(ClienteId)";
        List<Presupuestos> ListaDepresupuestos = new List<Presupuestos>();

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            using (SqliteCommand command = new SqliteCommand(queryDetalle, connection))
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int idpres = Convert.ToInt32(reader["idPresupuesto"]);
                    string fecha = Convert.ToString(reader["FechaCreacion"]);
                    int idC = Convert.ToInt32(reader["ClienteId"]);
                    string nombreC = Convert.ToString(reader["Nombre"]);
                    string emailC = Convert.ToString(reader["Email"]);
                    string telC = Convert.ToString(reader["Telefono"]);

                    Clientes nuevo = new Clientes(idC, nombreC, emailC, telC);
                    Presupuestos presup = new Presupuestos(idpres,nuevo, fecha);
                    ListaDepresupuestos.Add(presup);
                }
            }
        }
        return ListaDepresupuestos;
    }

    // Método para obtener los detalles de un presupuesto por su ID
    public Presupuestos ObtenerPresupuestoPorId(int id)
    {
        Presupuestos presupuesto = null;

        string query = @"SELECT 
            P.idPresupuesto,
            P.FechaCreacion,
            C.ClienteId,
            C.Nombre,
            C.Email,
            C.Telefono
            PR.idProducto,
            PR.Descripcion AS Producto,
            PR.Precio,
            PD.Cantidad
        FROM 
            Presupuestos P
        JOIN
            Clientes C USING(ClienteId)
        JOIN 
            PresupuestosDetalle PD ON P.idPresupuesto = PD.idPresupuesto
        JOIN 
            Productos PR ON PD.idProducto = PR.idProducto
        WHERE 
            P.idPresupuesto = @id;";

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            int cont = 1;
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (cont == 1)
                    {
                        Clientes cliente = new Clientes(Convert.ToInt32(reader["ClienteId"]), reader["Nombre"].ToString(), reader["Email"].ToString(), reader["Telefono"].ToString());
                        presupuesto = new Presupuestos(Convert.ToInt32(reader["idPresupuesto"]), cliente, reader["FechaCreacion"].ToString());
                    }
                    Producto producto = new Producto(Convert.ToInt32(reader["idProducto"]), reader["Producto"].ToString(), Convert.ToInt32(reader["Precio"]));
                    PresupuestosDetalles detalle = new PresupuestosDetalles(producto, Convert.ToInt32(reader["Cantidad"]));
                    presupuesto.Detalle.Add(detalle);
                    cont++;
                }
            }
            connection.Close();
        }
        return presupuesto;
    }

    // Método para eliminar un presupuesto por su ID
    public void EliminarPresupuesto(int idPresupuesto)
    {
        string query = @"DELETE FROM Presupuestos WHERE idPresupuesto = @IdP;";
        string query2 = @"DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @Id;";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query, connection);
            SqliteCommand command2 = new SqliteCommand(query2, connection);
            command.Parameters.AddWithValue("@IdP", idPresupuesto);
            command2.Parameters.AddWithValue("@Id", idPresupuesto);
            command2.ExecuteNonQuery();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }



    // Método para modificar un presupuesto existente
    public void ModificarPresupuesto(Presupuestos presupuesto)
    {
        using (var connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction()) //Preguntar como funciona esto
            {

                // Actualiza el presupuesto en la tabla Presupuestos
                string query = @"UPDATE Presupuestos 
                                 SET FechaCreacion = @fecha, ClienteId = @clienteId
                                 WHERE idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion);
                     command.Parameters.AddWithValue("@clienteId", presupuesto.Cliente.ClienteId);
                    command.Parameters.AddWithValue("@id", presupuesto.IdPresupuesto);
                    command.ExecuteNonQuery();
                }

                // Actualiza la tabla PresupuestosDetalle
                if (presupuesto.Detalle != null)
                {
                    foreach (var detalle in presupuesto.Detalle)
                    {

                        string updateDetalleQuery = @"UPDATE PresupuestosDetalle 
                                                       SET Cantidad = @cant
                                                       WHERE idPresupuesto = @idPr AND idProducto = @idP";

                        using (var command = new SqliteCommand(updateDetalleQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@cant", detalle.Cantidad);
                            command.Parameters.AddWithValue("@idP", detalle.Producto.IdProducto);
                            command.Parameters.AddWithValue("@idPr", presupuesto.IdPresupuesto);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                // Si todo sale bien, se confirma la transacción
                transaction.Commit();
            }
        }


    }

    public void AgregarProductoAPresupuesto(int idPresupuesto, Producto producto, int cantidad)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "INSERT INTO PresupuestosDetalle (IdPresupuesto, IdProducto, Cantidad) VALUES (@IdPresupuesto, @IdProducto, @Cantidad)";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
            command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
            command.Parameters.AddWithValue("@Cantidad", cantidad);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }


}