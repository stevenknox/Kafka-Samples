using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cient
{
    public class SuggestionService
    {
        private readonly Database db;

        public SuggestionService(Database db)
        {
            this.db = db;
        }
        public void Suggest(string userId, string product)
        {
            string[] str = product.Split(',');
            string type = str[0];
            string color = str[1];
            string design = str[2];

            Console.WriteLine($"User with ID {userId} showed interest in {type} of colour {color} with design {design}");

            User user = db.Find(userId);

            var suggestion = GenerateSuggestions(new List<Product> { new Product { Type = type, Color = Color.FromName(color), DesignType = design }});

            Console.WriteLine($"Suggested {user.Username} with product {suggestion.Type} of colour {suggestion.Color} with design {suggestion.DesignType}");

            user.Preferences.Add(suggestion);

            db.Save(user);
        }

        public Product GenerateSuggestions(List<Product> preferences)
        {
            var all = new List<Product> { 
                new Product { Type = "TSHIRT", Color = Color.Blue, DesignType = "Logo" },
                new Product { Type = "SHORTS", Color = Color.Pink, DesignType = "STRIPED" },
             };

            var random = new Random();
            int index = random.Next(all.Count);
            return all[index];
        }
    }
}
