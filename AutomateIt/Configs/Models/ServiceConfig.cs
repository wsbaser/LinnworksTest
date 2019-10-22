using System;
using System.Collections.Generic;
using automateit.Configs.Enums;
using Natu.Utils.Exceptions;

namespace automateit.Configs.Models {
    public class ServiceConfig {
        public string Id;
        public string DefaultServer;
        public GenericAccount DefaultIWAAccount;
        public GenericAccount DefaultFormsAccount;
        public GenericAccount DefaultWindowsAccount;
        public Dictionary<string, ServerConfig> Servers;

        public void Activate() {
            if (string.IsNullOrEmpty(DefaultServer)) {
                Throw.FrameworkException("DefaultServer is not defined in config file");
            }
            if (!Servers.ContainsKey(DefaultServer)) {
                Throw.FrameworkException($"Invalid DefaultServer property. Server '{DefaultServer}' is not defined in config file.");
            }
            foreach (var serverId in Servers.Keys) {
                Servers[serverId].Id = serverId;
                if (Servers[serverId].IWAAccount == null)
                    Servers[serverId].IWAAccount = DefaultIWAAccount;
                if (Servers[serverId].FormsAccount == null)
                    Servers[serverId].FormsAccount = DefaultFormsAccount;
                if (Servers[serverId].WindowsAccount == null)
                    Servers[serverId].WindowsAccount = DefaultWindowsAccount;
            }
        }

        public ServerConfig GetServer(string serverId) {
            if (!Servers.ContainsKey(serverId))
                Throw.FrameworkException($"Server '{serverId}' is not defined in config file.");
            return Servers[serverId];
        }

        public GenericAccount GetDefaultAccount(AuthType authType) {
            switch (authType) {
                case AuthType.IWA:
                    return DefaultIWAAccount;
                case AuthType.Forms:
                    return DefaultFormsAccount;
                default:
                    throw new ArgumentOutOfRangeException(nameof(authType), authType, null);
            }
        }

        public ServerConfig GetDefaultServer() => Servers[DefaultServer];
    }
}