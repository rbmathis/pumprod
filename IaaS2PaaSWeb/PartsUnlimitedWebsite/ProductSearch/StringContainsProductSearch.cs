using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using PartsUnlimited.Models;
using PartsUnlimited.Utils;

namespace PartsUnlimited.ProductSearch
{
    public class StringContainsProductSearch : IProductSearch
    {
        private readonly IPartsUnlimitedContext _context;

        public StringContainsProductSearch(IPartsUnlimitedContext context)
        {
            _context = context;
        }



     /** REPLACED BY AZURE SEARCH
        public async Task<IEnumerable<Product>> Search(string query)

        {
            try
            {
                var cleanQuery = Depluralize(query);

                var q = _context.Products
                    .Where(p => p.Title.ToLower().Contains(cleanQuery));

                return await q.ToListAsync();
            }
            catch
            {
                return new List<Product>();
            }
        }
    **/


        //use Azure Search to return search results
        public async Task<IEnumerable<Product>> Search(string query)
        {
            List<Product> products = new List<Product>();
            try
            {
                SearchParameters parameters;
                DocumentSearchResult<Product> searchResults;

                //declare the object properties that we want to be returned by our search
                //each property must be delcared as "Retrievable" in search, or an error will be thrown
                parameters = new SearchParameters()
                {
                    Select = new[] { "ProductId", "SkuNumber", "CategoryId", "Title", "Price", "SalePrice", "ProductArtUrl", "Description", "ProductDetails", "Inventory", "LeadTime" }
                };

                //returns Azure search objects containing list of <T>
                searchResults = await SearchHelper.SearchClient.Documents.SearchAsync<Product>(query, parameters);


                //convert the List<Result> to List<Product>
                foreach (var r in searchResults.Results)
                {
                    products.Add(r.Document);
                }
                return products;

            }
            catch (Exception ex)
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
    }
}


