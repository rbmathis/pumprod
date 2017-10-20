using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Models;
using PartsUnlimited.ViewModels;

namespace PartsUnlimited.ProductSearch
{
    public class StringContainsProductSearch : IProductSearch
    {
        private readonly IPartsUnlimitedContext _context;

        public StringContainsProductSearch(IPartsUnlimitedContext context)
        {
            _context = context;
        }



		// TODO: Change <Product> to return List of <ProductViewModel>
		// What wilil happen? curious..
		// How long will it take for me to find this violation?
		public async Task<IEnumerable<Product>> Search(string query)
        {
            var cleanQuery = Depluralize(query);

            try
			{
                var q = _context.Products
					.Where(p => p.Title.ToLower().Contains(cleanQuery));

				return await q.ToListAsync();
			}
			catch
			{
				return new List<Product>();
			}
        }




		//What code logic is covered here by tests?
		//Wouldn't it be nice to know this without having to build/deploy/test?
		public string Depluralize(string query)
		{
			if (String.IsNullOrEmpty(query)) return string.Empty;

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
                 query= query.Substring(0, query.Length - 1);
                //query = query.Substring(1, query.Length);

            }
			return query.ToLowerInvariant();
		}


	}


}
