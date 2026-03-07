using System.Security.Cryptography;

namespace WebApplication1
{
    public static class PasswordHasher
    {
        
        private const int SaltSize = 16;          
        private const int KeySize = 32;           
        private const int IterationsV1 = 100_000; 
        private const byte Version1 = 1;          

        public static string Hash(string password)
        {
            
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                IterationsV1,
                HashAlgorithmName.SHA256,
                KeySize); 
            

            
            byte[] result = new byte[1 + SaltSize + KeySize];
            result[0] = Version1;
            Buffer.BlockCopy(salt, 0, result, 1, SaltSize);
            Buffer.BlockCopy(key, 0, result, 1 + SaltSize, KeySize);

            
            return Convert.ToBase64String(result);
        }

        public static bool Verify(string password, string storedHash)
        {
            byte[] decoded;
            try
            {
                decoded = Convert.FromBase64String(storedHash);
            }
            catch
            {
                return false; 
            }

            if (decoded.Length < 1 + SaltSize + KeySize)
                return false;

            byte version = decoded[0];

            switch (version)
            {
                case Version1:
                    return VerifyV1(password, decoded);
                default:
                    
                    return false;
            }
        }

        private static bool VerifyV1(string password, byte[] decoded)
        {
            byte[] salt = new byte[SaltSize];
            byte[] storedKey = new byte[KeySize];

            Buffer.BlockCopy(decoded, 1, salt, 0, SaltSize);
            Buffer.BlockCopy(decoded, 1 + SaltSize, storedKey, 0, KeySize);

            byte[] computedKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                IterationsV1,
                HashAlgorithmName.SHA256,
                KeySize); 

            
            return CryptographicOperations.FixedTimeEquals(storedKey, computedKey); 
        }
    }
}
