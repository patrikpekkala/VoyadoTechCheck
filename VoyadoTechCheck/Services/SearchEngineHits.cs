using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
namespace VoyadoTechCheck.Services
{
    public class SearchEngineHits
    {
        public double TotalHits { get; set; }
        static string bingSubscriptionKey = Environment.GetEnvironmentVariable("BING_SEARCH_V7_SUBSCRIPTION_KEY");
        static string bingEndpoint = Environment.GetEnvironmentVariable("BING_SEARCH_V7_ENDPOINT") + "/v7.0/search";
        static string rapidKey = Environment.GetEnvironmentVariable("RAPID_KEY");
        static string rapidEndpoint = Environment.GetEnvironmentVariable("RAPID_ENDPOINT") + "/api/Search/WebSearchAPI";

        public void SearchForHits(string[] words)
        {
            TotalHits = 0;
            //call search engines with all words in array
            foreach (string word in words)
            {
                SearchRapid(word);
                SearchBing(word);
            }
        }
        private async void SearchBing(string searchWord)
        {
            var uriQuery = bingEndpoint + "?q=" + Uri.EscapeDataString(searchWord);


            WebRequest request = HttpWebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = bingSubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            var webpages = parsedJson.webPages;

            TotalHits = TotalHits + double.Parse((webpages.totalEstimatedMatches).ToString());
            //Console.WriteLine(JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
        }

        private async void SearchRapid(string searchWord)
        {
            var uriQuery = rapidEndpoint + "?q=" + Uri.EscapeDataString(searchWord);


            WebRequest request = HttpWebRequest.Create(uriQuery);
            request.Headers["x-rapidapi-key"] = rapidKey;
            request.Headers["x-rapidapi-host"] = "contextualwebsearch-websearch-v1.p.rapidapi.com";
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();


            dynamic parsedJson = JsonConvert.DeserializeObject(json);

            TotalHits = TotalHits + double.Parse((parsedJson.totalCount).ToString());
            //Console.WriteLine(JsonConvert.SerializeObject(parsedJson, Formatting.Indented));
        }
    }
}
