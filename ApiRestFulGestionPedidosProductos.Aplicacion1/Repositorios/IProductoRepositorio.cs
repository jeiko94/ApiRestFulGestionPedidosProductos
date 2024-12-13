using ApiRestFulGestionPedidosProductos.Dominio.Models;

namespace ApiRestFulGestionPedidosProductos.Aplicacion.Repositorios
{
    //Define las operaciones de persistencia relacionadas con la entida productos
    public interface IProductoRepositorio
    {
        Task CrearAsync(Producto producto);

        Task<Producto> ObtenerPorIdAsync(int id);

        Task<IEnumerable<Producto>> ObtenerTodosAsync();

        Task ActualizarAsync(Producto producto);

        Task EliminarAsync(int id);
    }
}
