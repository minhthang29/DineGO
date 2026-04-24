using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Client.Custom
{
  public class RestaurantWithFoodsViewModel
  {
    public int ResId { get; set; }
    public string ResName { get; set; }
    public string ResAddress { get; set; }
    public string ResImage { get; set; }
    [NotMapped]
    public List<string> ResImages
    {
      get
      {
        if (string.IsNullOrEmpty(ResImage))
          return new();
        try
        {
          return ResImage.Trim().StartsWith("[")
              ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(ResImage) ?? new()
              : ResImage.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        catch
        {
          return new();
        }
      }
    }

    public double? ResLatitude { get; set; }
    public double? ResLongitude { get; set; }

    [NotMapped]
    public double DistanceKm { get; set; }

    public List<FoodBasicViewModel> Foods { get; set; }
  }
}