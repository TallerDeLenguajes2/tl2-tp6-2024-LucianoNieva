namespace clientes
{
    public class Clientes{

        private int clienteId;
        private string nombre;
        private string email;
        private string telefono;

        public Clientes(){}

        public Clientes(int clienteId, string nombre, string email, string telefono){
            this.clienteId=clienteId;
            this.nombre=nombre;
            this.email=email;
            this.telefono=telefono;
        }

        public int ClienteId { get => clienteId; set => clienteId = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Email { get => email; set => email = value; }
        public string Telefono { get => telefono; set => telefono = value; }
    }
}