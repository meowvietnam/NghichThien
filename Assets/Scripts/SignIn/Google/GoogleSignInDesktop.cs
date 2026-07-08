using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SignIn.Google
{
    [CreateAssetMenu(fileName = "GoogleSignInDesktop", menuName = "SignIn/Google/GoogleSignInDesktop")]
    public class GoogleSignInDesktop : GoogleSignInPlatform
    {
        [SerializeField] private string clientId = "YOUR_CLIENT_ID.apps.googleusercontent.com";
        [SerializeField] private string clientSecret = "YOUR_CLIENT_SECRET";
        [SerializeField] private ushort listenPort = 3221;
        [SerializeField] private string handlerRoute = "/auth/handler";

        private const string PrefKeyIdToken = "google_id_token";
        private const string PrefKeyAccessToken = "google_access_token";
        private const string PrefKeyRefreshToken = "google_refresh_token";
        private const string PrefKeyExpiry = "google_token_expiry";
        private const string PrefKeyName = "google_user_name";
        private const string PrefKeyEmail = "google_user_email";
        private const string PrefKeyPicture = "google_user_picture";
        private const int ExpiryBufferSeconds = 60;

        private string WebUrl => $"http://localhost:{listenPort}";

        private static readonly HttpClient httpClient = new();
        private HttpListener listener;
        private TaskCompletionSource<GoogleCredential> tcs;
        [NonSerialized] private bool isServerStarted;


        public override async Task<GoogleCredential> SignInAsync()
        {
            var cached = await TryGetCachedUser();
            return cached ?? await SignInWithBrowser();
        }


        [ContextMenu("Sign Out")]
        public override async Task SignOutAsync()
        {
            if (PlayerPrefs.HasKey(PrefKeyAccessToken))
                await RevokeTokenOnServer(PlayerPrefs.GetString(PrefKeyAccessToken));
            ClearCache();
        }


        private async Task<GoogleCredential> TryGetCachedUser()
        {
            if (!PlayerPrefs.HasKey(PrefKeyRefreshToken)) return null;

            if (IsAccessTokenValid()) return BuildUserFromPrefs();

            var refreshed = await RefreshAccessToken(PlayerPrefs.GetString(PrefKeyRefreshToken));
            if (refreshed != null)
            {
                SaveTokenToPrefs(refreshed, existingPicture: PlayerPrefs.GetString(PrefKeyPicture));
                return BuildUserFromPrefs();
            }

            ClearCache();
            return null;
        }

        private bool IsAccessTokenValid()
        {
            if (!PlayerPrefs.HasKey(PrefKeyExpiry)) return false;
            var expiry = long.Parse(PlayerPrefs.GetString(PrefKeyExpiry));
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() < expiry - ExpiryBufferSeconds;
        }

        private GoogleCredential BuildUserFromPrefs()
        {
            var idToken = PlayerPrefs.GetString(PrefKeyIdToken);
            var name = PlayerPrefs.GetString(PrefKeyName);
            var email = PlayerPrefs.GetString(PrefKeyEmail);
            var imageUrl = Uri.TryCreate(PlayerPrefs.GetString(PrefKeyPicture), UriKind.Absolute, out var uri) ? uri : null;
            return new GoogleCredential(email, name, idToken, imageUrl);
        }

        private void SaveTokenToPrefs(TokenResponse token, string existingPicture = null)
        {
            var payload = GetPayloadFromIdToken(token.id_token);
            var expiryUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + token.expires_in;

            PlayerPrefs.SetString(PrefKeyIdToken, token.id_token);
            PlayerPrefs.SetString(PrefKeyAccessToken, token.access_token);
            PlayerPrefs.SetString(PrefKeyExpiry, expiryUnix.ToString());

            if (!string.IsNullOrEmpty(token.refresh_token))
                PlayerPrefs.SetString(PrefKeyRefreshToken, token.refresh_token);

            if (payload != null)
            {
                PlayerPrefs.SetString(PrefKeyName, payload.name);
                PlayerPrefs.SetString(PrefKeyEmail, payload.email);
                PlayerPrefs.SetString(PrefKeyPicture, payload.picture);
            }
            else if (!string.IsNullOrEmpty(existingPicture))
                PlayerPrefs.SetString(PrefKeyPicture, existingPicture);

            PlayerPrefs.Save();
        }

        public void ClearCache()
        {
            foreach (var key in new[]
                     {
                         PrefKeyIdToken, PrefKeyAccessToken, PrefKeyRefreshToken,
                         PrefKeyExpiry, PrefKeyName, PrefKeyEmail, PrefKeyPicture
                     })
                PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }


        private async Task<string> ExchangeCodeForToken(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", $"{WebUrl}{handlerRoute}"),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });

            try
            {
                var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", form);
                var body = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode ? body : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<TokenResponse> RefreshAccessToken(string refreshToken)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
            });

            try
            {
                var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", form);
                var body = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode ? JsonUtility.FromJson<TokenResponse>(body) : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task RevokeTokenOnServer(string token)
        {
            try
            {
                await httpClient.PostAsync(
                    $"https://oauth2.googleapis.com/revoke?token={Uri.EscapeDataString(token)}",
                    new StringContent(""));
            }
            catch
            {
                /* best-effort */
            }
        }

        private void SetResult(GoogleCredential credential)
        {
            if (tcs == null) return;
            if(!tcs.Task.IsCompleted) 
            {
                tcs.SetResult(credential);
                if (credential != null)
                {
                    Stop();
                }
            } 
        }
    
        private void TryThrowNewSession()
        {
            if(tcs == null) return;
            if (!tcs.Task.IsCompleted)
            {
                tcs.SetException(new NewGoogleSignInSessionException());
            }
        }
        private async Task<GoogleCredential> SignInWithBrowser()
        {
            if (!isServerStarted)
            {
                _ = StartServer();
                isServerStarted = true;
            }
            TryThrowNewSession();
            tcs = new TaskCompletionSource<GoogleCredential>();
            Application.OpenURL(
                "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=" + Uri.EscapeDataString(clientId) +
                "&redirect_uri=" + Uri.EscapeDataString($"{WebUrl}{handlerRoute}") +
                "&response_type=code" +
                "&scope=" + Uri.EscapeDataString("openid email profile") +
                "&access_type=offline" +
                "&prompt=consent");

            return await tcs.Task;
        }

        private async Task StartServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(WebUrl + "/");
            listener.Start();

            while (listener.IsListening)
            {
                try
                {
                    _ = HandleRequest(await listener.GetContextAsync());
                }
                catch (Exception ex)
                {
                  //  Debug.LogException(ex);
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            res.StatusCode = 200;
            res.ContentType = "text/plain; charset=utf-8";
      

            if (req.RawUrl.StartsWith(handlerRoute))
            {
                var tokenJson = await ExchangeCodeForToken(req.QueryString["code"]);

                if (!string.IsNullOrEmpty(tokenJson))
                {
                    var tokenResponse = JsonUtility.FromJson<TokenResponse>(tokenJson);
                    var payload = GetPayloadFromIdToken(tokenResponse.id_token);
                    SaveTokenToPrefs(tokenResponse);
                    var buffer = Encoding.UTF8.GetBytes("Đăng nhập thành công! Bạn có thể đóng tab này.");
                    res.ContentLength64 = buffer.Length;
                    await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    res.Close();
                    var user = new GoogleCredential(idToken: tokenResponse.id_token, name: payload?.name,
                        email: payload?.email, imageUrl: payload?.picture != null ? new Uri(payload.picture) : null);
                    SetResult(user);
                }
                else
                {
                    var buffer = Encoding.UTF8.GetBytes("Đăng nhập không thành công! Bạn có thể đóng tab này.");
                    res.ContentLength64 = buffer.Length;
                    await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    res.Close();
                    SetResult(null);
                }
            }

       
        }


        private IdTokenPayload GetPayloadFromIdToken(string idToken)
        {
            try
            {
                var parts = idToken.Split('.');
                if (parts.Length < 2) return null;

                var payload = parts[1]
                    .PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')
                    .Replace('-', '+')
                    .Replace('_', '/');

                return JsonUtility.FromJson<IdTokenPayload>(
                    Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
            }
            catch
            {
                return null;
            }
        }

        private void Stop()
        {
            listener?.Stop();
            listener?.Close();
            isServerStarted = false;
        }


        [Serializable]
        private class TokenResponse
        {
            public string id_token;
            public string access_token;
            public string refresh_token;
            public string token_type;
            public int expires_in;
        }

        [Serializable]
        private class IdTokenPayload
        {
            public string email;
            public string sub;
            public string name;
            public string picture;
        }
    }
}