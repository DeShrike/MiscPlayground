namespace Sovoreg.Crypto
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using NLog;

    /// <summary>
    /// Methods to encrypt and decrypt files.
    /// </summary>
    public class FileEncryptionService : BaseEncryptionService
    {
        private const string _extension = ".aes";

        /// <summary>
        /// Encrypt a file.
        /// </summary>
        /// <param name="inputFile">The name of the file to encrypt.</param>
        /// <param name="password">The password to use.</param>
        /// <param name="overwrite">If the resultfile should be overwritten if it already exists.</param>
        /// <param name="deleteOriginal">If the original file should be deleted after encryption.</param>
        public void EncryptFile(string inputFile, string password, bool overwrite = true, bool deleteOriginal = false)
        {
            var outputFile = inputFile + _extension;
            if (File.Exists(inputFile) == false)
            {
                throw new FileNotFoundException("Inputfile not found");
            }

            if (File.Exists(outputFile))
            {
                if (overwrite)
                {
                    try
                    {
                        File.Delete(outputFile);
                        Logger.Info("    Deleted " + outputFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }
                }
                else
                {
                    throw new ApplicationException("Outputfile exists");
                }
            }

            Logger.Info("Encrypting " + inputFile);
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

                using (var inputStream = new FileStream(inputFile, FileMode.Open))
                using (var outputStream = new FileStream(outputFile, FileMode.Create))
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

                if (deleteOriginal)
                {
                    try
                    {
                        File.Delete(inputFile);
                        Logger.Info("    Deleted " + inputFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts an encrypted file with the EncryptFile method through its path and the plain password.
        /// </summary>
        /// <param name="inputFile">The name of the file to decrypt.</param>
        /// <param name="password">The password to use.</param>
        /// <param name="overwrite">If the resultfile should be overwritten if it already exists.</param>
        public void DecryptFile(string inputFile, string password, bool overwrite = false)
        {
            string outputFile;
            if (inputFile.EndsWith(_extension, StringComparison.InvariantCulture))
            {
                outputFile = inputFile.Substring(0, inputFile.Length - _extension.Length);
            }
            else
            {
                outputFile = inputFile + ".plain";
            }

            if (File.Exists(inputFile) == false)
            {
                throw new FileNotFoundException("Inputfile not found");
            }

            if (File.Exists(outputFile))
            {
                if (overwrite)
                {
                    try
                    {
                        File.Delete(outputFile);
                        Logger.Info("    Deleted " + outputFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }
                }
                else
                {
                    throw new ApplicationException("Outputfile exists");
                }
            }

            Logger.Info("Decrypting " + inputFile);
            using (var aesManaged = new AesManaged())
            using (var inputStream = new FileStream(inputFile, FileMode.Open))
            {
                byte[] salt = GetSaltFromCiphertext(inputStream);
                byte[] initializationVector = GetInitializationVectorFromCiphertext(inputStream);

                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Key = GenerateKey(password, salt);
                aesManaged.IV = initializationVector;

                using (var cryptoStream = new CryptoStream(inputStream, aesManaged.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var outputStream = new FileStream(outputFile, FileMode.Create))
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

        /// <summary>
        /// Decrypt a file and returns it as a bayte array.
        /// </summary>
        /// <param name="inputFile">The name of the file to decrypt.</param>
        /// <param name="password">The password to use.</param>
        /// <returns>A byte array, or NULL.</returns>
        public byte[] DecryptFileToByteArray(string inputFile, string password)
        {
            if (inputFile.EndsWith(_extension, StringComparison.OrdinalIgnoreCase) == false)
            {
                inputFile += _extension;
            }

            if (File.Exists(inputFile) == false)
            {
                throw new FileNotFoundException("Inputfile not found");
            }

            Logger.Info("Decrypting " + inputFile);
            using (var aesManaged = new AesManaged())
            using (var inputStream = new FileStream(inputFile, FileMode.Open))
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

                            return outputStream.ToArray();
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
