using System.Security.Cryptography;

namespace AngularAuthAPI.Helpers
{
    public class PasswordHasser
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iteration = 10000;

        public static string HashPassword(string password)
        {
            
            return ""; //NOT DONE
        }
    }
}
