using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using HtmlAgilityPack;
using System.Globalization;
using TaskSoloution;


class Program
{
    static async Task Main(string[] args)
    {
        // URL of the e-commerce product page (example site for scraping practice)
        string url = "http://books.toscrape.com/";

        // Use HttpClient to get the HTML content
        HttpClient client = new HttpClient();
        var response = await client.GetStringAsync(url);

        // Load the HTML document
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(response);

        // Create a list to hold the extracted product details
        var products = new List<Product>();

        // Extract book names, prices, and ratings
        var bookTitles = doc.DocumentNode.SelectNodes("//h3/a");
        var bookPrices = doc.DocumentNode.SelectNodes("//div[@class='product_price']/p[@class='price_color']");
        var bookRatings = doc.DocumentNode.SelectNodes("//p[contains(@class, 'star-rating')]");

        if (bookTitles != null && bookPrices != null && bookRatings != null)
        {
            for (int i = 0; i < bookTitles.Count; i++)
            {
                string name = bookTitles[i].Attributes["title"].Value.Trim();
                string price = bookPrices[i].InnerText.Trim();
                string ratingClass = bookRatings[i].Attributes["class"].Value;

                string rating = ratingClass.Replace("star-rating", "").Trim();

                products.Add(new Product { Name = name, Price = price, Rating = rating });
            }
        }

        // Write the data to a CSV file using CsvHelper
        using (var writer = new StreamWriter("products.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(products);
        }

        Console.WriteLine("Product information successfully saved to products.csv");
    }
}
