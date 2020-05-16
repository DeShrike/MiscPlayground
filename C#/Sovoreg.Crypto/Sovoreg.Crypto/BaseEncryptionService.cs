namespace Sovoreg.Crypto
{
    using NLog;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public class BaseEncryptionService
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected const int _saltSizeBytes = 32;
        protected const int _IVSizeBytes = 16;
        protected const int _PBKDF2Iterations = 10000;

        /// <summary>
        /// Call this function to remove the key from memory after use for security.
        /// </summary>
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        /// <summary>
        /// Creates a random salt that will be used to encrypt your file. This method is required on EncryptFile.
        /// </summary>
        /// <returns></returns>
        protected static byte[] GenerateRandomSalt()
        {
            var data = new byte[32];

            var rng = new RNGCryptoServiceProvider();
            for (var i = 0; i < 10; i++)
            {
                // Fill the buffer with the generated data
                rng.GetBytes(data);
            }

            return data;
        }

        /// <summary>
        /// Generates a 256 bits symmetric encryption key using the PBKDF2 algorithm
        /// </summary>
        /// <param name="password">Password used to lock and unlock te file</param>
        /// <param name="salt">Random salt to prevent rainbow table hash cracking</param>
        /// <returns>An array of bytes representing the 256 bits key</returns>
        protected byte[] GenerateKey(string password, byte[] salt)
        {
            // Use password derivation function PBKDF2 with 10.000 iterations (1000 is default)
            // And a salt.
            var rfc = new Rfc2898DeriveBytes(password, salt, _PBKDF2Iterations);

            // Get 32 bytes (256 bits) from the derived key. A 256 bits key is required for AES.
            byte[] key = rfc.GetBytes(32);

            return key;
        }

        protected byte[] GetSaltFromCiphertext(Stream stream)
        {
            var salt = new byte[_saltSizeBytes];
            stream.Read(salt, 0, _saltSizeBytes);
            return salt;
        }

        protected byte[] GetInitializationVectorFromCiphertext(Stream stream)
        {
            var IV = new byte[_IVSizeBytes];
            stream.Read(IV, 0, _IVSizeBytes);
            return IV;
        }
    }
}
