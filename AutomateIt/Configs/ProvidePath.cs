using System;
using System.IO;

namespace automateit.Configs {
    public class ProvidePath {
        public static string ForConfig(string configFileName) => InOutputFolder(Path.Combine("configuration", configFileName));

        public static string InOutputFolder(string relativePath) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        public static string ForEmails() => InOutputFolder("Emails");
    }
}
