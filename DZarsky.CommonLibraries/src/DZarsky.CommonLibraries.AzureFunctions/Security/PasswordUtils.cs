using DZarsky.CommonLibraries.AzureFunctions.Configuration;
using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Text;

namespace DZarsky.CommonLibraries.AzureFunctions.Security
{
    public class PasswordUtils
    {
        private readonly AuthConfiguration _configuration;

        public PasswordUtils(AuthConfiguration configuration) => _configuration = configuration;

        private Argon2Config GetHasherConfiguration(string password)
        {
            var salt = new byte[_configuration.SaltBytes];
            RandomNumberGenerator.Create().GetBytes(salt);

            return new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 10,
                MemoryCost = 32768,
                Lanes = 5,
                Threads = Environment.ProcessorCount,
                Password = Encoding.UTF8.GetBytes(password),
                Salt = salt,
                Secret = Encoding.UTF8.GetBytes(_configuration.ArgonSecret),
                HashLength = _configuration.HashLength
            };
        }

        public string HashPassword(string password)
        {
            var config = GetHasherConfiguration(password);

            return Argon2.Hash(config);
        }

        public bool ValidatePassword(string password, string hashedPassword)
        {
            var config = GetHasherConfiguration(password);

            if (Argon2.Verify(hashedPassword, config))
            {
                return true;
            }

            return false;
        }
    }
}
