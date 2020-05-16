namespace Sovoreg.Encryptor
{
    using NLog;
    using Sovoreg.Crypto;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly string[] ExtensionToEncrypt = new[] { ".doc", ".docx", ".pdf", ".txt" };

        static void Main(string[] args)
        {
            var key1 = "6efbnlqGPRW3NlbWNqW+klBwjQxsXemzwmKepZXBPd8dL0l86cMvX9q60hy26Zzlj5xzT7rwPenQM4bXt74ZY1DD+M4MhE7PhnYm+Y4OS30GzCxBtpaoYDXGFgOe1rv0xVhUoR5TsJkEhdwQbQq4WZLqhplBAMxzw1OSMzi55Do=";
            var key2 = "UwBvAHYAbwByAGUAZwAuAFcAZQBiAA==";

            var key2Bytes = Convert.FromBase64String(key2);
            var password2 = System.Text.Encoding.Unicode.GetString(key2Bytes);

            Sovoreg.Crypto.StringEncryptionService ses = new Crypto.StringEncryptionService();
            var password1 = ses.Decrypt(key1, password2);

            //// EncryptFolders(args);

            string pw = "a";
            string data = "This is a test";

            Console.WriteLine(pw);
            Console.WriteLine("Original");
            Console.WriteLine(data);
            var se = new StringEncryptionService();
            var encrypted = se.Encrypt(data, pw);
            Console.WriteLine("Encrypted");
            Console.WriteLine(encrypted);
            var plain = se.Decrypt(encrypted, pw);
            Console.WriteLine("Decrypted");
            Console.WriteLine(plain);


            // var pwbytes = System.Text.Encoding.Unicode.GetBytes(pw);
            // Console.WriteLine(Convert.ToBase64String(pwbytes));
        }

        static void EncryptFolders(string[] args)
        {
            try
            {
                //This step will automatically cast the string args to a strongly typed object:
                var parameters = new CryptoParams(args);
                //This step will do type checking and validation
                parameters.CheckParams();

                Logger.Info("Folder: " + parameters.Folder);

                if (System.IO.Directory.Exists(parameters.Folder) == false)
                {
                    Console.WriteLine("Folder not found");
                    return;
                }

                EncryptFolder(parameters.Folder, parameters.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void EncryptFolder(string folder, string password)
        {
            Logger.Info(folder);

            var subFolders = System.IO.Directory.GetDirectories(folder);
            foreach (var subFolder in subFolders)
            {
                EncryptFolder(subFolder, password);
            }

            var fe = new FileEncryptionService();
            var gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            var files = System.IO.Directory.GetFiles(folder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fi = new System.IO.FileInfo(file);
                if (ExtensionToEncrypt.Contains(fi.Extension) == false)
                {
                    continue;
                }

                Logger.Info("  " + file);
                fe.EncryptFile(file, password);
            }

            FileEncryptionService.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
        }

        static void SimpleTest()
        {
            var fe = new FileEncryptionService();
            var password = "The Secret Password";
            Console.WriteLine("Password: {0}", password);

            Console.WriteLine("Press Enter to start encrypting");
            Console.ReadLine();

            // For additional security Pin the password of your files
            var gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Encrypt the file
            fe.EncryptFile(@"C:\TFSProjects\Playground\C#\Sovor.Crypto\Sovor.Crypto\testfile.zip", password);

            Console.WriteLine("Press Enter to start decrypting");
            Console.ReadLine();

            fe.DecryptFile(@"C:\TFSProjects\Playground\C#\Sovor.Crypto\Sovor.Crypto\testfile.zip.aes", password);

            // To increase the security of the encryption, delete the given password from the memory !
            FileEncryptionService.ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();

            Console.WriteLine(password);
        }
    }
}
