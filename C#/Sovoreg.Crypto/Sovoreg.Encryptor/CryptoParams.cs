namespace Sovoreg.Encryptor
{
    using ConsoleCommon;

    public class CryptoParams : ParamsObject
    {
        public CryptoParams(string[] args)
            : base(args)
        {
        }

        [Switch("F", true)]
        public string Folder { get; set; }

        [Switch("P", true)]
        public string Password { get; set; }
    }
}
