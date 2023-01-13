using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace Grocery.Models
{
    public class Product
    {
        public string? Id { get; set; }
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public double? PriceRegular { get; set; }
        public double? PricePromo { get; set; }
        public string? PriceRegularString { get; set; }
        public string? PricePromoString { get; set; }
        public string? Size { get; set; }
        public string? ImageURL { get; set; }
        public string? PricePerGal { get; set; }
        public double? PricePerUnit { get; set; }
        public string? PricePerUnitString { get; set; }
        private bool isLiquid { get; set; }
        private string? Unit { get; set; }

        public Product(dynamic? item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Id = item["productId"];
            Brand = item["brand"];
            Description = item["description"];
            PriceRegular = item["items"][0]["price"]["regular"];
            PricePromo = item["items"][0]["price"]["promo"];
            Size = item["items"][0]["size"];
            ImageURL = "https://www.kroger.com/product/images/medium/front/" + Id;
            PriceRegularString = string.Format("${0:N2}", PriceRegular);
            if (PricePromo != 0) PricePromoString = string.Format("${0:N2}", PricePromo);
            else PricePromoString = null;

            ///// Get Price Per Unit /////
            if (Size == null) return;

            // Delimit size string by spaces
            List<string> words = Size.Split(' ').ToList();
            List<string> fraction = words[0].Split('/').ToList();

            if (words.Count < 2) return;

            // Handle special cases where size is a count such as "10 ct / 35.2 oz"
            // The true size is either 10*35.2 or just 35.2
            // There is no standard for this so it's impossible to know which is accurate
            if (words[1] == "ct") return;

            // Handle special cases where amount may be a fraction such as 1/2 instead of 0.5
            double amount = 0;
            if (fraction.Count == 1)
            {
                amount = Convert.ToDouble(words[0]);
            }
            else
            {
                amount = Convert.ToDouble(fraction[0]) / Convert.ToDouble(fraction[1]);
            }

            // Handle special cases where the last word is not the unit
            int last = words.Count - 1;
            if (words[last] == "pouch" || words[last] == "cans" || words[last] == "can")
            {
                last--;
            }

            string unit = "";
            if (words[last - 1] == "fl")
            {
                unit = "fl oz";
            }
            else
            {
                unit = words[last].Trim();
            }

            double conversion = 0;
            switch (unit)
            {
                case "fl oz":
                    conversion = 0.0078125;
                    isLiquid = true;
                    break;
                case "gal":
                    conversion = 1;
                    isLiquid = true;
                    break;
                case "qt":
                    conversion = 0.25;
                    isLiquid = true;
                    break;
                case "pt":
                    conversion = 0.125;
                    isLiquid = true;
                    break;

                case "lb":
                    conversion = 1;
                    isLiquid = false;
                    break;
                case "lbs":
                    conversion = 1;
                    isLiquid = false;
                    break;
                case "oz":
                    conversion = 0.0625;
                    isLiquid = false;
                    break;

                default:
                    return;
            }

            if (isLiquid)
            {
                Unit = "gal";
            }
            else
            {
                Unit = "lb";
            }

            amount *= conversion;

            double? price = 0;
            if (PricePromo != 0)
            {
                price = PricePromo;
            }
            else
            {
                price = PriceRegular;
            }

            PricePerUnit = price / amount;

            PricePerUnitString = string.Format("${0:N2}/{1}", price / amount, Unit);
        }
    }
}