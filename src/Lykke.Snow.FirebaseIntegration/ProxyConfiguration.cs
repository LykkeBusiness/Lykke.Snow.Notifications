using System;

namespace Lykke.Snow.FirebaseIntegration
{
    public readonly struct ProxyConfiguration
    {
        public string Address { get; }
        public string? Username { get; }
        public string? Password { get; }

        public bool CanUseCredentials => !string.IsNullOrEmpty(Username); 
        
        public ProxyConfiguration(string address, string? username = null, string? password = null)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address));

            // check if both username and password are specified or none of them
            if (string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be specified without username");
            
            if (!string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username cannot be specified without password");
            
            Address = address;
            Username = username;
            Password = password;
        }
    }
}
