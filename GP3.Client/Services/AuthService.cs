using Plugin.Firebase.Auth;
using System.Text.Json;

namespace GP3.Client.Services
{
    public class AuthService
    {
        /*
        private const string apiKey = "AIzaSyAhgtYgvvLST3hIb_SOdIQ8JHYMGYeX9Ns";
        private readonly FirebaseAuthProvider authProvider = new(new FirebaseConfig(apiKey));
        */
        private readonly IFirebaseAuth _auth;
        public AuthService(IFirebaseAuth auth)
        {
            _auth = auth;
        }

        public string UserId => _auth.CurrentUser.Uid;

        public async Task<string> GetToken()
        {
            return (await _auth.CurrentUser.GetIdTokenResultAsync()).Token;
        }

        public bool IsSignedIn() => _auth.CurrentUser != null;

        public async Task RegisterAsync(string email, string password)
            => await _auth.CreateUserAsync(email, password);

        public async Task LoginAsync(string email, string password)
            => await _auth.SignInWithEmailAndPasswordAsync(email, password);

        public async Task ResetPassword(string email)
            => await _auth.SendPasswordResetEmailAsync(email);

        public async Task Signout()
            => await _auth.SignOutAsync();
    }
}
