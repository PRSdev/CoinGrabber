using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BinanceBotLib
{
    public class EmailHelper
    {
        public string Advice { get; private set; }
        public int UnreadEmails { get; private set; }

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

                int.TryParse(doc.SelectSingleNode(@"/feed/fullcount").InnerText, out int nr);
                UnreadEmails = nr;

                Advice = GetSubject(doc);
            }
            catch (Exception ex)
            {
                Bot.WriteLog(ex.Message);
            }
        }

        private string GetSubject(XmlDocument doc)
        {
            foreach (XmlNode node in doc.SelectNodes(@"/feed/entry"))
            {
                string title = node.SelectSingleNode("title").InnerText;
                if (!string.IsNullOrEmpty(title))
                    return title;
            }

            return null;
        }
    }
}