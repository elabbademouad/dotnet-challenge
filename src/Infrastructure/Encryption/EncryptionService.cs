using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Cds.DroidManagement.Infrastructure.Encryption
{
    /// <summary>
    /// Provides encryption/decryption features for standard strings
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionConfiguration _configuration;

        public EncryptionService(IOptions<EncryptionConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        /// <summary>
        /// Encrypt the given string using the Rijndael Algorithm
        /// </summary>
        /// <param name="clearText">The text to encrypt</param>
        /// <returns></returns>
        public string Encrypt(string clearText)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(clearText);

            var rijndaelEncryptor = InitializeEncryptor();
            var aesEncryptor = rijndaelEncryptor.CreateEncryptor();

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aesEncryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainText, 0, plainText.Length);
                cs.FlushFinalBlock();
            
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Decrypts the given string
        /// </summary>
        /// <param name="cipherText">The text to decrypt</param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            byte[] cipheredData = Convert.FromBase64String(cipherText);

            var rijndaelEncryptor = InitializeEncryptor();
            var decryptor = rijndaelEncryptor.CreateDecryptor();

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                byte[] plainTextData = new byte[cipheredData.Length];
                int decryptedByteCount = cs.Read(plainTextData, 0, plainTextData.Length);

                return Encoding.UTF8.GetString(plainTextData, 0, decryptedByteCount);
            }
        }

        private RijndaelManaged InitializeEncryptor()
        {
            const int rijndaelKey = 32;
            const int rijndaelIv = 16;
            var passwordBytes = new Rfc2898DeriveBytes(_configuration.Password, _configuration.Salt);

            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Key = passwordBytes.GetBytes(rijndaelKey),
                IV = passwordBytes.GetBytes(rijndaelIv)
            };
        }
    }
}
