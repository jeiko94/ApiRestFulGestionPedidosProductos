using ApiRestFulGestionPedidosProductos.Dominio.Models;

namespace ApiRestFulGestionPedidosProductos.Aplicacion.Repositorios
{
    //Define las operaciones de persistencia relacionadas con la entida cliente.
    public interface IClienteRepositorio
    {
        //Crea un cliente en la base de datos
        //Cliente = objeto de cliente a crear
        Task CrearAsync(Cliente cliente);

        //obtiene un cliente por su id
        //Id = del cliente
        //returns =  cliente encontrado o null si no existe.
        Task<Cliente> ObtenerPorIdAsync(int id);

        //obtener un cliente por su id
        //email = del cliente
        //returns = el cliente encontrado o null si no exite.
        Task<Cliente> ObtenerPorEmailAsync(string email);

        //Actualiza los datos de un cliente existente
        //Cliente = con los nuevos datos
        Task ActualizarAsync(Cliente cliente);

        //Eliminar un cliente por su id
        //Id = del cliente a eliminar
        Task EliminarPorIdAsync(int id);
    }
}
