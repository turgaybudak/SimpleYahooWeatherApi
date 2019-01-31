using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace SimpleYahooWeatherForecast
{
    public class WeatherForecast
    {
        // http://woeid.rosselliot.co.nz/lookup/ankara buradan konum id öğrenilebilir.//                                                                                                                                                    havadurumulokasyonu
        public List<WeatherForecast_Type> GetWeatherForecast(string cAppID, string cConsumerKey, string cConsumerSecret, string cWeatherID)
        {
            string cURL = "https://weather-ydn-yql.media.yahoo.com/forecastrss";
            string cOAuthVersion = "1.0";
            string cOAuthSignMethod = "HMAC-SHA1";
            string cUnitID = "u=c";
            string cFormat = "json";
            cWeatherID = "woeid=" + cWeatherID;
            string lURL = cURL + "?" + cWeatherID + "&" + cUnitID + "&format=" + cFormat;

            var lClt = new WebClient();

            lClt.Headers.Set("Content-Type", "application/" + cFormat);
            lClt.Headers.Add("Yahoo-App-Id", cAppID);
            lClt.Headers.Add("Authorization", _get_auth(cURL, cAppID, cConsumerKey, cConsumerSecret, cOAuthVersion, cOAuthSignMethod, cWeatherID, cUnitID, cFormat));

            byte[] lDataBuffer = lClt.DownloadData(lURL);

            var lOut = Encoding.ASCII.GetString(lDataBuffer);

            string json = lOut;
            json = json.Substring(json.IndexOf("["));
            json = json.Substring(0, json.IndexOf("]") + 1);

            JavaScriptSerializer js = new JavaScriptSerializer();
            WeatherForecast_Type[] persons = js.Deserialize<WeatherForecast_Type[]>(json);

            List<WeatherForecast_Type> DuzenliListe = new List<WeatherForecast_Type>();

            foreach (var item in persons)
            {
                WeatherForecast_Type Yeni = new WeatherForecast_Type();
                string veri = @"/Date(" + item.date + ")/";
                DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                Double milliseconds = Convert.ToDouble(item.date);
                DateTime dateTime = unixEpoch.AddSeconds(milliseconds).ToLocalTime();
                Yeni.day = item.day;
                Yeni.date = dateTime.ToString("MM-dd-yyyy");
                Yeni.low = item.low;
                Yeni.high = item.high;
                Yeni.text = item.text;
                Yeni.code = item.code;
                DuzenliListe.Add(Yeni);
            }

            return DuzenliListe;
        }

        private static string _get_timestamp()
        {
            TimeSpan lTS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(lTS.TotalSeconds).ToString();
        }

        private static string _get_nonce()
        {
            return Convert.ToBase64String(
             new ASCIIEncoding().GetBytes(
              DateTime.Now.Ticks.ToString()
             )
            );
        }

        private static string _get_auth(string cURL, string cAppID, string cConsumerKey, string cConsumerSecret, string cOAuthVersion, string cOAuthSignMethod, string cWeatherID, string cUnitID, string cFormat)
        {
            string retVal;
            string lNonce = _get_nonce();
            string lTimes = _get_timestamp();
            string lCKey = string.Concat(cConsumerSecret, "&");
            string lSign = string.Format(  // note the sort order !!!
             "format={0}&" +
             "oauth_consumer_key={1}&" +
             "oauth_nonce={2}&" +
             "oauth_signature_method={3}&" +
             "oauth_timestamp={4}&" +
             "oauth_version={5}&" +
             "{6}&{7}",
             cFormat,
             cConsumerKey,
             lNonce,
             cOAuthSignMethod,
             lTimes,
             cOAuthVersion,
             cUnitID,
             cWeatherID
            );

            lSign = string.Concat(
             "GET&", Uri.EscapeDataString(cURL), "&", Uri.EscapeDataString(lSign)
            );

            using (var lHasher = new HMACSHA1(Encoding.ASCII.GetBytes(lCKey)))
            {
                lSign = Convert.ToBase64String(
                 lHasher.ComputeHash(Encoding.ASCII.GetBytes(lSign))
                );
            }  // end using

            return "OAuth " +
                   "oauth_consumer_key=\"" + cConsumerKey + "\", " +
                   "oauth_nonce=\"" + lNonce + "\", " +
                   "oauth_timestamp=\"" + lTimes + "\", " +
                   "oauth_signature_method=\"" + cOAuthSignMethod + "\", " +
                   "oauth_signature=\"" + lSign + "\", " +
                   "oauth_version=\"" + cOAuthVersion + "\"";
        }  // end _get_auth

        public class WeatherForecast_Type
        {
            public string day { get; set; }
            public string date { get; set; }
            public string low { get; set; }
            public string high { get; set; }
            public string text { get; set; }
            public string code { get; set; }
        }
    }
}