using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FitbitClient.Models;
using FitbitClient.OAuth2;

namespace FitbitClient
{
    public class Program
    {
        public static readonly string FitbitOauthPostUrl = "https://api.fitbit.com/oauth2/token";
        private static string ClientId = "My Client ID";
        private static string ClientSecret = "My Client Secret";

        private static string RedirectUri = "https://api.mdstation.eu/auth/oauth/callback";

        //authCode : The authorization code received in the redirect as a URI parameter. 
        //More information - https://dev.fitbit.com/build/reference/web-api/oauth2/
        private static string authCode = "My authCode";

        public static string encodedClientID = "3TTGVV";

        //accessToken : Valid until 9/5/2018
        private static string accessToken =
                "My accessToken";

        //A refresh token can only be used once, as a new refresh token is returned with the new access token.
        private static string refreshToken = "my refreshToken";
        static void Main()
        {           
            RunClient();
            Console.ReadLine();
        }

        static void RunClient()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.fitbit.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Exchange Authorization Code with Access Token  -  https://github.com/aarondcoleman/Fitbit.NET
                //Information about Authorization Code Grant Flow - https://dev.fitbit.com/build/reference/web-api/oauth2/
                //OAuth2AccessToken accessToken = ExchangeAuthCodeForAccessTokenAsync(authCode).GetAwaiter().GetResult();
                //OAuth2AccessToken refreshedToken = RefreshTokenAsync(refreshToken).GetAwaiter().GetResult();

                //Requests
                //UserProfile profile = GetUserProfile(client, accessToken, encodedClientID).GetAwaiter().GetResult();
                //Activity activity = GetDayActivityAsync(client, DateTime.Today, encodedClientID).GetAwaiter().GetResult();
            }
        }
        
        public static async Task<OAuth2AccessToken> ExchangeAuthCodeForAccessTokenAsync(string code)
        {
            HttpClient httpClient = new HttpClient();

            string postUrl = OAuth2Helper.FitbitOauthPostUrl;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", ClientId),
                //new KeyValuePair<string, string>("client_secret", AppSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });


            string clientIdConcatSecret = OAuth2Helper.Base64Encode(ClientId + ":" + ClientSecret);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", clientIdConcatSecret);

            HttpResponseMessage response = await httpClient.PostAsync(postUrl, content);
            string responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
            OAuth2AccessToken accessToken = OAuth2Helper.ParseAccessTokenResponse(responseString);

            
            return accessToken;
        }

        //Refresh AccessToken when expired.
        public static async Task<OAuth2AccessToken> RefreshTokenAsync(string expiredToken)
        {
            HttpClient httpClient = new HttpClient();

            string postUrl = OAuth2Helper.FitbitOauthPostUrl;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", expiredToken),
            });


            string clientIdConcatSecret = OAuth2Helper.Base64Encode(ClientId + ":" + ClientSecret);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", clientIdConcatSecret);

            HttpResponseMessage response = await httpClient.PostAsync(postUrl, content);
            string responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
            OAuth2AccessToken accessToken = OAuth2Helper.ParseAccessTokenResponse(responseString);


            return accessToken;
        }

        public static async Task<UserProfile> GetUserProfile(HttpClient client, string accessToken, string encodedUserId)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //encodedUserId : The encoded ID of the user. Use "-" (dash) for current logged-in user.
            HttpResponseMessage response = await client.GetAsync("/1/user/" + encodedUserId + "/profile.json");

            if (CheckStatusCode(response.StatusCode))
            {
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var serializer = new JsonDotNetSerializer { RootProperty = "user" };
                UserProfile profile = serializer.Deserialize<UserProfile>(responseBody);

                //Console.WriteLine("Profile "+ profile.DisplayName);
                return profile;
            }
            else
            {
                return null;
            }
        }
        public static async Task<Activity> GetDayActivityAsync(HttpClient client, DateTime activityDate, string encodedUserId )
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //encodedUserId : The encoded ID of the user. Use "-" (dash) for current logged-in user.
            HttpResponseMessage response = await client.GetAsync("/1/user/" + encodedUserId + "/activities/date/" + activityDate.ToFitbitFormat() + ".json");
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            Activity activity = serializer.Deserialize<Activity>(responseBody);

            //Console.WriteLine("Activity daily steps " + activity.Summary.Steps);
            return activity;
        }
        private static bool CheckStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("You are unauthorized");
                return false;
            }

            if (statusCode == HttpStatusCode.BadRequest)
            {
                Console.WriteLine("Bad Request");
                return false;
            }
            return true;
        }
    }
}