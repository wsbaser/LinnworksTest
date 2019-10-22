using System;
using System.Net;

namespace automateit.Configs.Models
{
    public class GenericAccount
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public int Role { get; set; }

        public NetworkCredential NetworkCredentials => new NetworkCredential(Login, Password, Domain);

        public string NetworkLogin
        {
            get
            {
                if (string.IsNullOrEmpty(Domain))
                    throw new InvalidOperationException($"Account {Login} can not be used as IWA account. Property 'Domain' is not initialized.");
                return $"{Domain}\\{Login}";
            }
        }

        public string FullName => string.IsNullOrWhiteSpace(Domain) ? $"{Domain}\\{Login}" : Login;
        // TODO: get rid of this
        public string DmsUserId { get; set; }
    }
}