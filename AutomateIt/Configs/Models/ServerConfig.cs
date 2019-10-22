using System;
using automateit.Configs.Enums;

namespace automateit.Configs.Models
{
    public class ServerConfig {
        public string Id;
        public string ConnectionString;
        public AuthType AuthType;
        public string Host;
        public GenericAccount FormsAccount;
        public GenericAccount IWAAccount;
        public GenericAccount WindowsAccount;
        public string Version;

        public GenericAccount GetAuthAccount() {
            switch (AuthType) {
                case AuthType.IWA:
                    return IWAAccount;
                case AuthType.Forms:
                    return FormsAccount;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Version GetVersion() => new Version(Version);
    }
}
