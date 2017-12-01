using System;
namespace Drone.API.Twitter.OAuth
{
    [Serializable]
    public class OAuthTokens
    {
        public string AccessToken { internal get; set; }
        public string AccessTokenSecret { internal get; set; }
        public string ConsumerKey { internal get; set; }
        public string ConsumerSecret { internal get; set; }
        public bool HasConsumerToken
        {
            get
            {
                return !string.IsNullOrEmpty(this.ConsumerKey) && !string.IsNullOrEmpty(this.ConsumerSecret);
            }
        }
        public bool HasAccessToken
        {
            get
            {
                return !string.IsNullOrEmpty(this.AccessToken) && !string.IsNullOrEmpty(this.AccessTokenSecret);
            }
        }
        public bool HasBothTokens
        {
            get
            {
                return this.HasAccessToken && this.HasConsumerToken;
            }
        }
    }
}
