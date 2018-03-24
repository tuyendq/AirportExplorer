using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using CsvHelper;
using CsvHelper.Configuration;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace AirportExplorer.Pages
{
    public class IndexModel : PageModel
    {
        // property: MapboxAccessToken 
        private readonly IHostingEnvironment _hostingEnvironment;
        public string MapboxAccessToken { get; }

        // constructor: IndexModel
        public IndexModel(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            MapboxAccessToken = configuration["Mapbox:AccessToken"];
        }
        public void OnGet()
        {

        }

        public IActionResult OnGetAirports()
        {
            var configuration = new Configuration
            {
                BadDataFound = ContextBoundObject =>
                {

                }
            };

            using (var sr = new StreamReader(Path.Combine(_hostingEnvironment.WebRootPath, "airports.dat")))
            using (var reader = new CsvReader(sr, configuration))
            {
                FeatureCollection featureCollection = new FeatureCollection();

                while(reader.Read())
                {
                    string name = reader.GetField<string>(1);
                    string iataCode = reader.GetField<string>(4);
                    double latitude = reader.GetField<double>(6);
                    double longitude = reader.GetField<double>(7);

                    featureCollection.Features.Add(new Feature(
                        new Point(new Position(latitude, longitude)), 
                        new Dictionary<string, object>
                        {
                            {"name", name},
                            {"iataCode", iataCode}
                        }
                    ));
                }
            }

            //return new JsonResult(featureCollection);
            return featureCollection;
        }
    }
}
