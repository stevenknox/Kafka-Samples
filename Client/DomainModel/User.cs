using System;
using System.Collections.Generic;

namespace Cient
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Product> Preferences { get; set; } = new List<Product>();

        public override string ToString()
        {
            return $"{UserId} - {Username} - {DateOfBirth.ToShortDateString()}";
        }
    }
}
