using System.Collections.Generic;
using System.IO;
using System.Threading;
using automateit.Configs;
using automateit.Configs.Models;
using ActiveUp.Net.Mail;
using NUnit.Framework;
using selenium.core.Auxiliary;

namespace automateit.Auxiliary
{
    public class MailHelper
    {
        static MailHelper()
        {
            _letters = new Dictionary<string, Message>();
        }

        public static void DeleteMessages(GenericAccount emailAccount)
        {
            DeleteMessages(emailAccount.Login, emailAccount.Password);
        }

        public static void DeleteMessages(string email, string password)
        {
            var client = GMailClient.Connect(email, password);
            client.DeleteAll();
        }

        public static Message GetMessage(GenericAccount mailAccount, string titlePattern)
        {
            return GetMessage(mailAccount.Login, mailAccount.Password, titlePattern);
        }

        private static readonly Dictionary<string, Message> _letters;

        public static void CleanUp()
        {
            _letters.Clear();
        }

        public static Message GetMessage(string email, string password, string titlePattern)
        {
            var client = CreateClient(email, password);
            Message letter = null;
            for (var i = 0; i < 90; i++)
            {
                letter = client.GetLetter(titlePattern);
                if (letter != null)
                    break;
                Thread.Sleep(1000);
            }

            if (letter != null)
            {
                _letters[letter.Subject] = letter;
            }

            return letter;
        }

        private static GMailClient CreateClient(string email, string password)
        {
            var client = GMailClient.Connect(email, password);
            return client;
        }

    }

    [TestFixture]
    public class MailHelperTests
    {
        [Test]
        public void DeleteAllTest()
        {
            MailHelper.GetMessage("s.westkm.mail@gmail.com", "Diana88@", "Email from West KM");
        }
    }
}