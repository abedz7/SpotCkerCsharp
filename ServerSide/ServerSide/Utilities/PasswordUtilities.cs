using System.Text;
using System.Security.Cryptography;

namespace ServerSide.Utilities
{
    public static class PasswordUtilities
    {
        // Hash the password using SHA256
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Hash the entered password
            string hashedEnteredPassword = HashPassword(enteredPassword);

            // Compare the entered hashed password with the stored hashed password
            return hashedEnteredPassword == storedPasswordHash;
        }

    }
}
