using System.Text.RegularExpressions;

namespace OnlineBank.Auth.Application.Common
{
    public static class Validation
    {
        // Simple, practical email check (not perfect RFC, but good for training)
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static bool IsValidEmail(string email) =>
            !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

        public static bool IsValidPassword(string password) =>
            !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
    }
}
