using System.ComponentModel.DataAnnotations;

namespace ApiRestFulGestionPedidosProductos.Dominio.Models
{
    //Representa un cliente del sistema, con sus datos basicos.
    public class Cliente
    {
        //Identificador unico del cliente
        public int Id { get; set; }

        //Nombre del cliente
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        //Email del cliente, debe ser unico
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email no valido.")]
        public string Email { get; set; }

        //Contraseña hasheada del cliente para su autenticación.
        //No se guarda la contraseña en texto plano.
        [Required(ErrorMessage = "La contrasela es obligatoria.")]
        public string Password { get; set; }

        //Fecha de registro del cliente en el sistema.
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
