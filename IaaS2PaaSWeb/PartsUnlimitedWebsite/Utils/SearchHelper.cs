using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PartsUnlimited.Utils
{
    public class SearchHelper
    {
        /// <summary>
        /// static object to use for search interaction, new-up only when required
        /// </summary>
        private static Lazy<SearchIndexClient> lazySearcher = new Lazy<SearchIndexClient>(() =>
        {
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string queryApiKey = ConfigurationManager.AppSettings["SearchServiceQueryApiKey"];
            string indexName = ConfigurationManager.AppSettings["SearchServiceIndexName"];

            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));
            return indexClient;
        });

        /// <summary>
        /// SearchIndexClient object
        /// </summary>
        public static SearchIndexClient SearchClient
        {
            get
            {
                return lazySearcher.Value;
            }
        }
    }
}