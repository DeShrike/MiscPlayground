namespace Sovoreg.Crypto
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    public class StringEncryptionService : BaseEncryptionService
    {
        /// <summary>
        /// Encrypt a string with the given password.
        /// </summary>
        /// <param name="data">The string to encrypt.</param>
        /// <param name="password">The password to use.</param>
        /// <returns>A Base64 encoded string.</returns>
        public string Encrypt(string data, string password)
        {
            using (var aesManaged = new AesManaged())
            {
                // Set ciphermode for the AES algoritm (CBC, cipher block chaining, by default)
                aesManaged.Mode = CipherMode.CBC;

                // Generate initialization vector, IV is 16 bytes (128 bits) long
                aesManaged.GenerateIV();

                // Generate a random salt
                byte[] salt = GenerateRandomSalt();

                // Generate a 256 bits key using the password and the salt
                aesManaged.Key = GenerateKey(password, salt);

                byte[] buffer = new byte[1024 * 1024];
                int read, totalRead = 0;

                var dataBytes = System.Text.Encoding.Unicode.GetBytes(data);

                using (var inputStream = new MemoryStream(dataBytes))
                using (var outputStream = new MemoryStream())
                {
                    // Append salt to filestream
                    outputStream.Write(salt, 0, salt.Length);

                    // Append initialization vector to filestream
                    outputStream.Write(aesManaged.IV, 0, aesManaged.IV.Length);

                    try
                    {
                        using (var cryptoStream = new CryptoStream(outputStream, aesManaged.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                totalRead += read;
                                Logger.Trace("    {0} bytes", totalRead);
                                cryptoStream.Write(buffer, 0, read);
                            }

                            Logger.Info("    Encrypt Done - {0} bytes", totalRead);
                        }

                        var result = outputStream.ToArray();
                        return Convert.ToBase64String(result);
                    }
                    catch (CryptographicException ex_CryptographicException)
                    {
                        Logger.Error(ex_CryptographicException, "CryptographicException error");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }
                }
            }
        }

        public string Decrypt(string data, string password)
        {
            var encryptedBytes = Convert.FromBase64String(data);

            using (var aesManaged = new AesManaged())
            using (var inputStream = new MemoryStream(encryptedBytes))
            {
                byte[] salt = GetSaltFromCiphertext(inputStream);
                byte[] initializationVector = GetInitializationVectorFromCiphertext(inputStream);

                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Key = GenerateKey(password, salt);
                aesManaged.IV = initializationVector;

                using (var cryptoStream = new CryptoStream(inputStream, aesManaged.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        int read, totalRead = 0;
                        var buffer = new byte[1024 * 1024];

                        try
                        {
                            while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                totalRead += read;
                                Logger.Trace("    {0} bytes", totalRead);
                                outputStream.Write(buffer, 0, read);
                            }

                            Logger.Info("    Decrypt Done - {0} bytes", totalRead);

                            var result = outputStream.ToArray();
                            return System.Text.Encoding.Unicode.GetString(result);
                        }
                        catch (CryptographicException ex_CryptographicException)
                        {
                            Logger.Error(ex_CryptographicException, "CryptographicException error");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            throw;
                        }
                    }
                }
            }
        }
    }
}