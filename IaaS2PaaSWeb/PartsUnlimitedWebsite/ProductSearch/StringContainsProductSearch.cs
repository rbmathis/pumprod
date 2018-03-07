using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using PartsUnlimited.Models;

namespace PartsUnlimited.ProductSearch
{
    public class StringContainsProductSearch : IProductSearch
    {
        private readonly IPartsUnlimitedContext _context;

        public StringContainsProductSearch(IPartsUnlimitedContext context)
        {
            _context = context;
        }

		// TODO: [EF] Change this to return List of ProductViewModel?
        public async Task<IEnumerable<Product>> Search(string query)
        {
            List<Product> products = new List<Product>();
            try
			{
                SearchParameters parameters;  
                DocumentSearchResult<Product> searchResults;

               parameters = new SearchParameters()
                {
                    Select = new[] { "ProductId", "SkuNumber", "CategoryId", "Title", "Price", "SalePrice", "ProductArtUrl", "Description", "ProductDetails", "Inventory", "LeadTime" }
                };

                //returns Azure search objects containing list of <T>
                searchResults = CreateSearchIndexClient().Documents.Search<Product>(query, parameters);


                
                foreach (var r in searchResults.Results)
                {
                    products.Add(r.Document);
                }
                return products;

			}
			catch(Exception ex)
			{
				//eat it
			}
            return products;
        }

		public string Depluralize(string query)
		{
			if (query.EndsWith("ies"))
			{
				query = query.Substring(0, query.Length - 3) + "y";
			}
			else if (query.EndsWith("es"))
			{
				query = query.Substring(0, query.Length - 1);
			}
			else if (query.EndsWith("s"))
			{
				query = query.Substring(1, query.Length);
			}
			return query.ToLowerInvariant();
		}

        private static SearchIndexClient CreateSearchIndexClient()
        {
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string queryApiKey = ConfigurationManager.AppSettings["SearchServiceQueryApiKey"];
            string indexName = ConfigurationManager.AppSettings["SearchServiceIndexName"];

            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));
            return indexClient;
        }
    }
}


