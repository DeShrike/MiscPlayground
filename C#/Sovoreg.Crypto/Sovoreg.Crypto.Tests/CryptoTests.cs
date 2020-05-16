namespace Sovoreg.Crypto.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CryptoTests
    {
        [TestMethod]
        public void TestStringEncryption()
        {
            string pw = "password";
            string data = "The Is Very Sensitive Data !";
            var se = new StringEncryptionService();
            var encrypted = se.Encrypt(data, pw);
            var plain = se.Decrypt(encrypted, pw);

            Assert.AreEqual(data, plain);
        }
    }
}
