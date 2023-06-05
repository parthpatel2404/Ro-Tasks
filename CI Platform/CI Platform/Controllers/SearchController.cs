using CIPlatform.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
namespace CI_Platform.Controllers
{
    public class SearchController : Controller
    {

        private const string ApiKey = "AIzaSyDMrgEhZivUaX2TrgB_N7rFxXsNXG5jt44";
        private const string SearchEngineId = "53d298e86b5694353";

        public ActionResult CustomGoogle(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return View();
            }

            // Perform the search
            var searchResults = PerformSearch(search);

            return View(searchResults);
        }

        private List<SearchResult> PerformSearch(string search)
        {
            var searchResults = new List<SearchResult>();

            // Create the URL for the API request
            var apiUrl = $"https://www.googleapis.com/customsearch/v1?key={ApiKey}&cx={SearchEngineId}&q={WebUtility.UrlEncode(search)}";

            // Make the API request
            using (var client = new WebClient())
            {
                var json = client.DownloadString(apiUrl);

                // Deserialize the JSON response
                dynamic response = JsonConvert.DeserializeObject(json);

                // Extract the search results
                if (response.items != null)
                {
                    foreach (var item in response.items)
                    {
                        var result = new SearchResult
                        {
                            Title = item.title,
                            Url = item.link,
                            Snippet = item.snippet
                        };

                        searchResults.Add(result);
                    }
                }
            }

            return searchResults;
        }


    }
}

