using System;
using System.Collections.Generic;

namespace Cient
{
    public static class EventGenerator
    {
        public static List<Event> Generate(Database db)
        {
            var tshirt = new Product { Color = System.Drawing.Color.Red, DesignType = "Logo", Type="Polo" };
            var shorts = new Product { Color = System.Drawing.Color.Blue, DesignType = "None", Type="Shorts" };
            var events = new List<Event>();

            var products = new List<Product> { 
                new Product { Color = System.Drawing.Color.Red, DesignType = "Logo", Type="Polo" },
                new Product { Color = System.Drawing.Color.Blue, DesignType = "None", Type="Shorts" },
                new Product { Color = System.Drawing.Color.Blue, DesignType = "Striped", Type="Tshirt" },
                new Product { Color = System.Drawing.Color.Blue, DesignType = "Checked", Type="Trousers" },
                new Product { Color = System.Drawing.Color.Blue, DesignType = "Plain", Type="Shirt" }
             };

            var random = new Random();

            foreach (var user in db.Users)
            {
                for (int i = 0; i < random. Next(1, 3); i++)
                {
                     int index = random.Next(products.Count);
                    events.Add(new Event { User = user, Product = products[index] });
                }
            }

            return events;

        }
    }
}