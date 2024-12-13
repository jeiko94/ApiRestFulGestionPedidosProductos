using ApiRestFulGestionPedidosProductos.Aplicacion.Repositorios;
using ApiRestFulGestionPedidosProductos.Dominio.Models;
using System.Security.Cryptography;
using System.Text;


namespace ApiRestFulGestionPedidosProductos.Aplicacion.Servicios
{
    //Servio de aplicacion para manejar la logica relacionada con clientes.
    public class ClienteServicio
    {
        private readonly IClienteRepositorio _clienteRepositorio;

        //Constructor que recibe el repositorio de cliente (puerto)
        //clienteRepositorio = Repositorio de clientes
        public ClienteServicio(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        //Registra un nuevo cliente en el sistema, validando duplicados y encriptando la contraseña.
        //nombre del cliente
        //email del cliente
        //contraseña en texto plano
        public async Task RegistrarClienteAsync(string nombre, string email, string password)
        {
            //verificar si el email ya existe
            var clienteExiste = await _clienteRepositorio.ObtenerPorEmailAsync(email);

            if (clienteExiste != null)
            {
                throw new InvalidOperationException("El email ya esta registrado.");
            }

            //Hashear la contraseña
            string passwordHashed = HashPassword(password);

            //Crear objeto cliente
            var nuevoCliente = new Cliente
            {
                Nombre = nombre,
                Email = email,
                Password = passwordHashed,
                FechaRegistro = DateTime.UtcNow
            };

            //Guardar en repositorio
            await _clienteRepositorio.CrearAsync(nuevoCliente);
           
        }

        //Metodo auxiliar para encriptar / hashear la contraseña.
        //Aqui usamos un SHA256 de manera sencilla.
        //En produccion preferible manejar librerias espeficicas como BCrypt.
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        //Obtiene un cliente por su Id
        public async Task<Cliente> ObtenerClienteAsync(int id)
        {
            return await _clienteRepositorio.ObtenerPorIdAsync(id);
        }

        //Actualiza los datos de un cliente
        //Puede considerar validaciones adicionales
        public async Task ActualizarClienteAsync(int id, string nuevoNombre, string nuevoEmail)
        {
            var cliente = await _clienteRepositorio.ObtenerPorIdAsync(id);

            if (cliente == null)
                throw new InvalidOperationException("Cliente no encontrado.");

            //Verificar si el nuevo email ya esta en uso por otro cliente
            var clienteOtro = await _clienteRepositorio.ObtenerPorEmailAsync(nuevoEmail);

            if (clienteOtro != null && clienteOtro.Id != id)
            {
                throw new InvalidOperationException("El email ya esta en uso por otro cliente.");
            }

            cliente.Nombre = nuevoNombre;
            cliente.Email = nuevoEmail;

            await _clienteRepositorio.ActualizarAsync(cliente);
        }

        //Eliminar un cliente por su Id
        public async Task EliminarClienteAsync(int id)
        {
            await _clienteRepositorio.EliminarPorIdAsync(id);
        }
    }
}
