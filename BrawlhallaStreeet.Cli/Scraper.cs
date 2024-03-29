﻿using AngleSharp;
using AngleSharp.Html.Dom;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BrawlhallaStreeet.Cli
{
    public class Scraper
    {
        // Example Url: https://www.brawlhalla.com/c/uploads/2020/07/Jaeyun_WebSplash.jpg

        /// <summary>
        /// The idea for this class is to scrape the valid image urls off the legends page
        /// </summary>

        private static string BaseUrl = @"https://brawlhalla.com/c/uploads/2018/11/";

        private static List<string> Legends = new List<string>()
        {
            "Ada",
            "Artemis",
            "Asuri",
            "Azoth",
            "Barraza",
            "Bodvar",
            "Brynn",
            "Caspian",
            "Cassidy",
            "Cross",
            "Diana",
            "Dusk",
            "Ember",
            "Fait",
            "Gnash",
            "Hattori",
            "Isaiah",
            "Jhala",
            "Jiro",
            "Kaya",
            "Koji",
            "Kor",
            "Lin Fei",
            "Lord Vraxx",
            "Lucien",
            "Mirage",
            "Mordex",
            "Nix",
            "Orion",
            "Petra",
            "Queen Nai",
            "Ragnir",
            "Rayman",
            "Scarlet",
            "Sentinel",
            "Sidra",
            "Sir Roland",
            "Teros",
            "Thatch",
            "Thor",
            "Ulgrim",
            "Val",
            "Vector",
            "Volkov",
            "Wu Shang",
            "Xull",
            "Yumiko",
            "Zariel"
        };

        public async Task ScrapeLegendImages()
        {
            Dictionary<string, string> legendImages = new Dictionary<string, string>();
            var config = Configuration.Default.WithDefaultLoader();
            var address = "https://www.brawlhalla.com/legends/";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            var images = document.QuerySelectorAll("img").ToList();

            using (WebClient client = new WebClient())
            {
                foreach (var legend in Legends)
                {
                    var legendImage = (IHtmlImageElement)images.Where(x => x.GetAttribute("src").ToLower().Contains(legend.ToLower())).FirstOrDefault();
                    // Also Search for each word in Legend Name if empty
                    if (legendImage == null)
                    {
                        var names = legend.ToLower().Split(' ').Reverse();
                        foreach (var name in names)
                        {
                            legendImage = (IHtmlImageElement)images.Where(x => x.GetAttribute("src").ToLower().Contains(name)).FirstOrDefault();
                            if (legendImage != null) break;
                        }
                    }

                    var source = legendImage?.Source;
                    // Download source
                    legendImages.Add(legend, source);
                }
            }
            var serialized = JsonConvert.SerializeObject(legendImages, Formatting.Indented);
            System.IO.File.WriteAllText(@"C:\Code\Repositories\BrawlhallaStreet\BrawlhallaStreet.Core\Images\LegendImageUrls.json", serialized);
        }
    }
}