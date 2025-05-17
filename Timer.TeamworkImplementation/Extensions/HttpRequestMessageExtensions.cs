using System.Text;

namespace Timer.Shared.Extensions
{
    public static class HttpRequestMessageExtensions
    {

        public static void AddAuthenticationHeader(this HttpRequestMessage httpRequestMessage, bool isBasic, string authValue)
        {
            var auth = isBasic ? "Basic" : "Bearer";
            var token = isBasic ? Convert.ToBase64String(Encoding.ASCII.GetBytes($"{authValue}:")) : authValue;

            httpRequestMessage.Headers.Add("Authorization", $"{auth} {token}");
        }

    }

}
