using PartsUnlimited.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using PartsUnlimited.Utils;
using PartsUnlimited.ViewModels;
using System.Collections.Generic;

namespace PartsUnlimited.Controllers
{
    public class StoreController : Controller
    {
        private readonly IPartsUnlimitedContext db;

        public StoreController(IPartsUnlimitedContext context)
        {
            db = context;
        }

        //
        // GET: /Store/
        public ActionResult Index()
        {
            List<Category> genres = new List<Category>();
            if (RedisHelper.GetList<Category>("categories") == null)
            {
                genres = db.Categories.ToList();
                RedisHelper.SetList<Category>("categories", genres);

            }


            return View(RedisHelper.GetList<Category>("categories"));
            //return View(genres);
        }

        [OutputCache(Duration=500, VaryByParam="categoryId")]
        public ActionResult Browse(int categoryId)
        {
            Category category = new Category();
            category = RedisHelper.Get<Category>($"category-{categoryId}");
            if (category == null)
            {
                category = db.Categories.Include("Products").Single(g => g.CategoryId == categoryId);
                RedisHelper.Set($"category-{categoryId}", category);
            }

            return View(category);
        }
        [OutputCache(Duration = 500, VaryByParam = "id")]
        public ActionResult Details(int id)
        {

            var productCacheKey = $"product_{id}";
            var product = RedisHelper.Get<Product>(productCacheKey);
            if (product == null)
            {
                product = db.Products.Single(a => a.ProductId == id);
                RedisHelper.Set(productCacheKey, product);
            }
            var viewModel = new ProductViewModel
            {
                Product = product,
                ShowRecommendations = ConfigurationHelpers.GetBool("ShowRecommendations")
            };

            return View(viewModel);
        }
    }
}
