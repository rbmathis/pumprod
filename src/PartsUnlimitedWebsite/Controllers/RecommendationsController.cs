using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PartsUnlimited.Models;
using PartsUnlimited.Recommendations;
using PartsUnlimited.Utils;
using System.Collections.Generic;
using System;

namespace PartsUnlimited.Controllers
{
    public class RecommendationsController : Controller
    {
        private readonly IPartsUnlimitedContext db;
        private readonly IRecommendationEngine recommendation;

        public RecommendationsController(IPartsUnlimitedContext context, IRecommendationEngine recommendationEngine)
        {
            db = context;
            recommendation = recommendationEngine;
        }

        public async Task<ActionResult> GetRecommendations(string productId)
        {

            var recommendedProducts = new List<Product>();
            if (!ConfigurationHelpers.GetBool("ShowRecommendations"))
            {
                return new EmptyResult();
            }
            try
            {
                var recommendedProductIds = await recommendation.GetRecommendationsAsync(productId);

                recommendedProducts = await db.Products.Where(x => recommendedProductIds.Contains(x.ProductId.ToString())).ToListAsync();
            }
            catch (Exception ex)
            {
                //eat it
            }
            return PartialView("_Recommendations", recommendedProducts);
        }
    }
}
