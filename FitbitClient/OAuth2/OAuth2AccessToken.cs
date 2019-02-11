using System;

namespace FitbitClient.OAuth2
{
    public class OAuth2AccessToken
    {
       
        public string Token { get; set; }

       
        public string TokenType { get; set; } // "Bearer" is expected

      
        public string Scope { get; set; }
        
    
        public int ExpiresIn { get; set; } //maybe convert this to a DateTime ?

       
        public string RefreshToken { get; set; }

       
        public string UserId { get; set; }

        /// <summary>
        /// This property is NOT set by the library. It is simply provided as a covenience placeholder. The library consumer is responsible for setting up this field.
        /// The library assums this DateTime is UTC for token validation purposes.
        /// </summary>
        public DateTime UtcExpirationDate { get; set; }

        public bool IsFresh()
        {
            if (DateTime.MinValue == UtcExpirationDate)
                throw new InvalidOperationException(
                    $"The {nameof(UtcExpirationDate)} property needs to be set before using this method.");
            return DateTime.Compare(DateTime.UtcNow, UtcExpirationDate) < 0;
        }
    }
}
