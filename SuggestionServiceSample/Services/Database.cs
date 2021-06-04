using System;
using System.Collections.Generic;

namespace Cient
{
    public class Database
    {
        public Database()
        {
            var bob = new User { UserId = Guid.NewGuid().ToString(), Username = "Bob", DateOfBirth = DateTime.Now.AddYears(-18) };
            var sam = new User { UserId = Guid.NewGuid().ToString(), Username = "Sam", DateOfBirth = DateTime.Now.AddYears(-40) };

            Users = new List<User> { bob, sam };
        }
        public List<User> Users { get; set; }

        public User Find(string id)
        {
            return Users.Find(f=> f.UserId == id);
        }

        public void Save(User user)
        {
            var u = Users.Find(f=> f.UserId == user.UserId);
            u = user;
        }
    }
}
