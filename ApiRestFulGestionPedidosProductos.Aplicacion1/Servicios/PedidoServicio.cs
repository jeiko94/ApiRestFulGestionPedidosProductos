using ApiRestFulGestionPedidosProductos.Aplicacion.Repositorios;
using ApiRestFulGestionPedidosProductos.Dominio.Models;

namespace ApiRestFulGestionPedidosProductos.Aplicacion.Servicios
{
    public class PedidoServicio
    {
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly IProductoRepositorio _productoRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio;

        public PedidoServicio(IPedidoRepositorio pedidoRepositorio, IProductoRepositorio productoRepositorio, IClienteRepositorio clienteRepositorio)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _productoRepositorio = productoRepositorio;
            _clienteRepositorio = clienteRepositorio;
        }

        //Crea un pedido vacio para un cliente especifico (estado pendiente)
        public async Task<int> CrearPedidoAsync(int clienteId)
        {
            var cliente = await _clienteRepositorio.ObtenerPorIdAsync(clienteId);

            if (cliente == null)
                throw new InvalidOperationException("Cliente no existe.");

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                FechaCreacion = DateTime.UtcNow,
                Estado = EstadoPedido.Pendiente
            };

            await _pedidoRepositorio.CrearAsync(pedido);
            return pedido.Id;
        }

        //Agregar un producto al pedido con la cantidad especifica.
        //Asume que el pedido esta en estado pendiente
        public async Task AgregarProductoAsync(int pedidoId, int productoId, int cantidad)
        {
            //Verificar pedido existente
            var pedido = await _pedidoRepositorio.ObtenerPorIdAsync(pedidoId);

            if (pedido == null)
                throw new InvalidOperationException("Pedido no encontrado.");

            if (pedido.Estado != EstadoPedido.Pendiente)
                throw new InvalidOperationException("No se pueden agregar productos a un pedido no pendiente.");

            //Verificar producto
            var producto = await _productoRepositorio.ObtenerPorIdAsync(productoId);

            if (producto == null)
                throw new InvalidOperationException("Producto no encontrado.");

            //Crear detalle pedido
            var detallePedido = new DetallePedido
            {
                PedidoId = pedidoId,
                ProductoId = productoId,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            };

            pedido.Detalles.Add(detallePedido);

            //Actualizar pedido en repositorio
            await _pedidoRepositorio.ActualizarAsync(pedido);
        }
    }
}
