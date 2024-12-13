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

            if (cantidad < 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

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

        //Cambiar el estado del pedido a confirmado y ajusta el stock de los productos
        public async Task ConfirmarPedidoAsync(int id)
        {
            var pedido = await _pedidoRepositorio.ObtenerPorIdAsync(id);

            if (pedido == null)
                throw new InvalidOperationException("Pedido no encontrado.");

            if (pedido.Estado != EstadoPedido.Pendiente)
                throw new InvalidOperationException("Solo un pedido pendiente se puede confirmar.");

            //verificar stock de cada producto
            foreach(var detalle in pedido.Detalles)
            {
                var producto = await _productoRepositorio.ObtenerPorIdAsync(detalle.ProductoId);
                if(producto.Stock < detalle.Cantidad)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre}");
                }
            }

            //Descontar stock
            foreach(var detalles in pedido.Detalles)
            {
                var producto = await _productoRepositorio.ObtenerPorIdAsync(detalles.ProductoId);
                producto.Stock -= detalles.Cantidad;
                await _productoRepositorio.ActualizarAsync(producto);
            }

            //Cambiar estado
            pedido.Estado = EstadoPedido.Confirmado;
            await _pedidoRepositorio.ActualizarAsync(pedido);    
        }

        //Cambiar el estado de un pedido a entregado
        public async Task EntregarPedidoAsync(int pedidoId)
        {
            var pedido = await _pedidoRepositorio.ObtenerPorIdAsync(pedidoId);
            if (pedido == null)
                throw new InvalidOperationException("Pedido no encontrado.");

            if (pedido.Estado != EstadoPedido.Confirmado)
                throw new InvalidOperationException("Solo un producto confirmado se puede marcar como entregado.");

            pedido.Estado = EstadoPedido.Entregado;
            await _pedidoRepositorio.ActualizarAsync(pedido);
        }

        //Obtener los pedidos de un cliente especifico
        public async Task<IEnumerable<Pedido>> ObtenerPedidosDeClienteAsync(int clienteId)
        {
            return await _pedidoRepositorio.ObtenerPorClienteAsync(clienteId);
        }

        //Obtener un pedido por id
        public async Task<Pedido> ObtenerPedidoAsync(int pedidoId)
        {
            return await _pedidoRepositorio.ObtenerPorIdAsync(pedidoId);
        }

        public async Task EliminarPedidoAsync(int pedidoId)
        {
            await _pedidoRepositorio.EliminarAsync(pedidoId);
        }
    }
}
