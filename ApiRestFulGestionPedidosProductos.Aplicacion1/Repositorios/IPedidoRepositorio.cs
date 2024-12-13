using ApiRestFulGestionPedidosProductos.Dominio.Models;

namespace ApiRestFulGestionPedidosProductos.Aplicacion.Repositorios
{
    
    //Define las operaciobes de persistencia relacionadas con la entidad Pedido y sus detalles
    public interface IPedidoRepositorio
    {
        Task CrearAsync(Pedido pedido);

        Task<Pedido> ObtenerPorIdAsync(int id);

        Task<IEnumerable<Pedido>> ObtenerTodosAsync();

        Task ActualizarAsync(Pedido pedido);

        Task EliminarAsync(int id);
    }
}
