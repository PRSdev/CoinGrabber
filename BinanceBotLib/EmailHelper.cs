using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BinanceBotLib
{
    public class EmailHelper
    {
        public string Subject { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        public bool NewMail { get; private set; }

        public EmailHelper(string email, string password)
        {
            try
            {
                System.Net.WebClient objClient = new System.Net.WebClient();
                objClient.Credentials = new System.Net.NetworkCredential(email, password);

                string response; response = Encoding.UTF8.GetString(objClient.DownloadData(@"https://mail.google.com/mail/feed/atom"));

                response = response.Replace(@"<feed version=""0.3"" xmlns=""http://purl.org/atom/ns#"">", @"<feed>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);

                ReadLatestEmail(doc);
            }
            catch (Exception ex)
            {
                Bot.WriteLog(ex.Message);
            }
        }

        private void ReadLatestEmail(XmlDocument doc)
        {
            foreach (XmlNode node in doc.SelectNodes(@"/feed/entry"))
            {
                string title = node.SelectSingleNode("title").InnerText;
                if (!string.IsNullOrEmpty(title))
                    Subject = title;

                string modified = node.SelectSingleNode("modified").InnerText;
                if (!string.IsNullOrEmpty(modified))
                {
                    DateTime dateTime;
                    DateTime.TryParse(modified, out dateTime);
                    ModifiedDate = dateTime;

                    NewMail = ModifiedDate > Bot.Settings.LastEmailDateTime;
                    if (NewMail)
                        Bot.Settings.LastEmailDateTime = ModifiedDate;
                    return;
                }
            }
        }
    }
}