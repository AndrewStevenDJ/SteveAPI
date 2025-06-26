using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SteveAPI.Data;

namespace SteveAPI.Services
{
    /// <summary>
    /// Lógica de negocio para desencriptar mensajes almacenados.
    /// </summary>
    public class DesencriptarService
    {
        private readonly ApplicationDbContext _db;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public DesencriptarService(ApplicationDbContext db, IConfiguration cfg)
        {
            _db = db;

            _key = Encoding.UTF8.GetBytes(
                cfg["Encryption:Key"] ?? "0123456789ABCDEF0123456789ABCDEF");

            _iv = Encoding.UTF8.GetBytes(
                cfg["Encryption:IV"] ?? "ABCDEF0123456789");
        }

        /// <summary>
        /// Devuelve el texto plano correspondiente al registro <paramref name="id"/>.
        /// </summary>
        public async Task<string?> DesencriptarPorIdAsync(int id)
        {
            var entidad = await _db.MensajesEncriptados
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(e => e.Id == id);

            return entidad is null ? null : AesDecrypt(entidad.TextoCifrado);
        }

        /*--------------------------------------------------------------------
         * Utilidades internas
         *------------------------------------------------------------------*/
        private string AesDecrypt(string cipherBase64)
        {
            using var aes = Aes.Create();
            aes.Mode    = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key     = _key;
            aes.IV      = _iv;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherBase64);
            var plainBytes  = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
