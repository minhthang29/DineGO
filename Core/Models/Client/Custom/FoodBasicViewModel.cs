using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
namespace Core.Models.Client.Custom
{
    public class FoodBasicViewModel
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal FoodPrice { get; set; }
        public string FoodImage { get; set; }
        public int? FoodStatus { get; set; }

        [NotMapped]
        public List<string> FoodImages
        {
            get
            {
                if (string.IsNullOrEmpty(FoodImage))
                    return new List<string>();
                try
                {
                    if (FoodImage.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(FoodImage) ?? new List<string>();
                    return FoodImage.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }


    }
}