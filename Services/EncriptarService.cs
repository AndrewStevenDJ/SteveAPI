using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SteveAPI.Data;
using SteveAPI.Models;

namespace SteveAPI.Services
{
    /// <summary>
    /// Lógica de negocio para crear, leer, actualizar y eliminar
    /// mensajes encriptados con AES-256-CBC.
    /// </summary>
    public class EncriptarService
    {
        private readonly ApplicationDbContext _db;
        private readonly byte[] _key;   // 32 bytes  = 256 bits
        private readonly byte[] _iv;    // 16 bytes  = 128 bits

        public EncriptarService(ApplicationDbContext db, IConfiguration cfg)
        {
            _db = db;

            _key = Encoding.UTF8.GetBytes(
                cfg["Encryption:Key"] ?? "0123456789ABCDEF0123456789ABCDEF");

            _iv = Encoding.UTF8.GetBytes(
                cfg["Encryption:IV"] ?? "ABCDEF0123456789");
        }

        /*--------------------------------------------------------------------
         * CRUD
         *------------------------------------------------------------------*/
        public async Task<Encriptar> CreateAsync(string textoPlano)
        {
            var entidad = new Encriptar
            {
                TextoCifrado = AesEncrypt(textoPlano)
            };

            _db.MensajesEncriptados.Add(entidad);
            await _db.SaveChangesAsync();
            return entidad;
        }

        public Task<List<Encriptar>> GetAllAsync() =>
            _db.MensajesEncriptados.AsNoTracking().ToListAsync();

        public Task<Encriptar?> GetByIdAsync(int id) =>
            _db.MensajesEncriptados.FindAsync(id).AsTask();

        /// <summary>Reemplaza el registro completo (ya modificado) y guarda.</summary>
        public async Task ReplaceAsync(Encriptar entidad)
        {
            _db.MensajesEncriptados.Update(entidad);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entidad = await _db.MensajesEncriptados.FindAsync(id);
            if (entidad is null) return false;

            _db.MensajesEncriptados.Remove(entidad);
            await _db.SaveChangesAsync();
            return true;
        }

        /*--------------------------------------------------------------------
         * Utilidades internas
         *------------------------------------------------------------------*/
        private string AesEncrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Mode    = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key     = _key;
            aes.IV      = _iv;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes   = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes  = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(cipherBytes);
        }
    }
}
