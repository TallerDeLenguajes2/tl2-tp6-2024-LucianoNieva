
using Microsoft.Data.Sqlite;
using presupuestos;
using presupuestosDetalles;
using producto;

namespace presupuestosRepository;
public class PresupuestosRepository
{
    private string cadenaConexion = "Data Source=Tienda.db";

    // Método para crear un nuevo presupuesto
    public void CrearNuevo(Presupuestos presupuesto)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
            command.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion);
            command.ExecuteNonQuery();

            // Comando para obtener el ID del último registro insertado
            command.CommandText = "SELECT last_insert_rowid();";
            presupuesto.IdPresupuesto = (int)(long)command.ExecuteScalar();

            connection.Close();
        }

        // Agregar detalles del presupuesto en la tabla intermedia
        foreach (var detalle in presupuesto.Detalle)
        {
            AgregarProductoAPresupuesto(presupuesto.IdPresupuesto, detalle.Producto, detalle.Cantidad);
        }
    }

    // Método para listar todos los presupuestos
    public List<Presupuestos> ListarPresupuestos()
    {
        List<Presupuestos> listaPresupuestos = new List<Presupuestos>();

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "SELECT * FROM Presupuestos";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var presupuesto = new Presupuestos
                    {
                        IdPresupuesto = reader.GetInt32(reader.GetOrdinal("IdPresupuesto")),
                        NombreDestinatario = reader.GetString(reader.GetOrdinal("NombreDestinatario")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        Detalle = ObtenerDetallesPresupuesto(reader.GetInt32(reader.GetOrdinal("IdPresupuesto")))
                    };
                    listaPresupuestos.Add(presupuesto);
                }
            }
            connection.Close();
        }

        return listaPresupuestos;
    }

    // Método para obtener los detalles de un presupuesto por su ID
    public Presupuestos ObtenerPresupuestoPorId(int id)
    {
        Presupuestos presupuesto = null;

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "SELECT * FROM Presupuestos WHERE IdPresupuesto = @Id";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    presupuesto = new Presupuestos
                    {
                        IdPresupuesto = reader.GetInt32(reader.GetOrdinal("IdPresupuesto")),
                        NombreDestinatario = reader.GetString(reader.GetOrdinal("NombreDestinatario")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        Detalle = ObtenerDetallesPresupuesto(id)
                    };
                }
            }
            connection.Close();
        }

        return presupuesto;
    }

    // Método para agregar un producto y cantidad a un presupuesto existente
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

    // Método para eliminar un presupuesto por su ID
    public void EliminarPresupuesto(int id)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "DELETE FROM Presupuestos WHERE IdPresupuesto = @Id";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    // Método auxiliar para obtener los detalles de un presupuesto
    private List<PresupuestosDetalles> ObtenerDetallesPresupuesto(int idPresupuesto)
    {
        List<PresupuestosDetalles> detalles = new List<PresupuestosDetalles>();

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = @"
                SELECT p.IdProducto, p.Descripcion, p.Precio, pp.Cantidad 
                FROM PresupuestosDetalle pp
                INNER JOIN Productos p ON pp.IdProducto = p.IdProducto
                WHERE pp.IdPresupuesto = @IdPresupuesto";

            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var detalle = new PresupuestosDetalles
                    {
                        Producto = new Producto
                        {
                            IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            Precio = reader.GetInt32(reader.GetOrdinal("Precio"))
                        },
                        Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"))
                    };
                    detalles.Add(detalle);
                }
            }
            connection.Close();
        }

        return detalles;
    }


    // Método para modificar un presupuesto existente
    public void ModificarPresupuesto(Presupuestos presupuesto)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "UPDATE Presupuestos SET NombreDestinatario = @NombreDestinatario, FechaCreacion = @FechaCreacion WHERE IdPresupuesto = @IdPresupuesto";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
            command.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion);
            command.Parameters.AddWithValue("@IdPresupuesto", presupuesto.IdPresupuesto);
            command.ExecuteNonQuery();
            connection.Close();
        }

        // Actualizar los detalles del presupuesto
        foreach (var detalle in presupuesto.Detalle)
        {
            // Llamar al método modificado con los parámetros necesarios
            ModificarDetallePresupuesto(presupuesto.IdPresupuesto, detalle.Producto, detalle.Cantidad);
        }
    }

    // Método auxiliar para modificar los detalles de un presupuesto
    private void ModificarDetallePresupuesto(int idPresupuesto, Producto producto, int cantidad)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "UPDATE PresupuestosDetalle SET Cantidad = @Cantidad WHERE IdPresupuesto = @IdPresupuesto AND IdProducto = @IdProducto";
            connection.Open();
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Cantidad", cantidad);
            command.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);
            command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }


}
