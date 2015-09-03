using System;

namespace VendingDataExtractor.Structures
{
    public class Auth
    {
        public String Id { get; set; }
        public String Password { get; set; }

        public Auth(string id, string password)
        {
            this.Id = id;
            this.Password = password;
        }
    }
}