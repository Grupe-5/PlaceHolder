using Firebase.Auth;
using System.Text.Json;

namespace GP3.Client.Services
{
    public class AuthService
    {
        private const string apiKey = "AIzaSyAhgtYgvvLST3hIb_SOdIQ8JHYMGYeX9Ns";
        private readonly FirebaseAuthProvider authProvider = new(new FirebaseConfig(apiKey));

        private FirebaseAuth auth = null;

        public string Token => auth.FirebaseToken;
        public string UserId => auth.User.LocalId;

        private async Task SaveAuth()
        {
            string authJson = null;
            if (auth != null)
            {
                authJson = JsonSerializer.Serialize(auth);
            }

            if (authJson != null)
            {
                await SecureStorage.SetAsync("auth_token", authJson);
            }
            else
            {
                SecureStorage.Remove("auth_token");
            }
        }

        public async Task LoadAuth()
        {
            string authJson = await SecureStorage.GetAsync("auth_token");
            if (authJson != null)
            {
                auth = JsonSerializer.Deserialize<FirebaseAuth>(authJson);
            }
        }

        public bool IsSignedIn()
        {
            return auth != null && !auth.IsExpired();
        }

        public async Task RegisterAsync(string email, string password)
        {
            try
            {
                auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                await SaveAuth();
            }
            catch { throw; }
        }

        public async Task LoginAsync(string email, string password)
        {
            try
            {
                auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
                await SaveAuth();
            }
            catch { throw; }
        }

        public async Task ResetPassword(string email)
        {
            try
            {
                await authProvider.SendPasswordResetEmailAsync(email);
            }
            catch { throw; }
        }

        public async Task RefreshTokenAsync()
        {
            if (auth != null && auth.IsExpired())
            {
                Console.WriteLine("Refreshing expired auth token");
                auth = await authProvider.RefreshAuthAsync(auth);
            }
        }

        public static string ParseErrorToString(FirebaseAuthException ex)
        {

            string errorText = "";

            switch (ex.Reason)
            {
                case AuthErrorReason.InvalidEmailAddress:
                    errorText = "Email address and/or password is invalid";
                    break;

                case AuthErrorReason.UserNotFound:
                    errorText = "This user does not exist";
                    break;

                default:
                    errorText = ex.Message;
                    break;
            }

            return errorText;
        }
    }
}
