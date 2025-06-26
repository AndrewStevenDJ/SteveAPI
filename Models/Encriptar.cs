using System.ComponentModel.DataAnnotations;

namespace SteveAPI.Models
{
    /// <summary>
    /// Representa un mensaje cifrado con AES-256-CBC.
    /// </summary>
    public class Encriptar
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Texto cifrado en Base64.
        /// </summary>
        [Required]
        public string TextoCifrado { get; set; } = string.Empty;

        /// <summary>
        /// Marca de fecha para auditoría.
        /// </summary>
        public DateTime Creado { get; set; } = DateTime.UtcNow;
    }
}
