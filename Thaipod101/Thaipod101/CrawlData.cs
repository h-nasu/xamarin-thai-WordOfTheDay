using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Thaipod101
{
    public class CrawlData
    {
        public List<Examples> MyExamples;

        public CrawlData()
        {
            this.MyExamples = new List<Examples>();
        }

        public void test()
        {
            Debug.WriteLine("Hello Test");
        }

        public async Task<int> DownloadHomepage(String text)
        {
            var httpClient = new HttpClient(); // Xamarin supports HttpClient!

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("language", "Thai"),
                new KeyValuePair<string, string>("date", text)
            });

            var response = await httpClient.PostAsync("http://www.thaipod101.com/widget/wotd/large.php", formContent); // async method!

            var stringContent = await response.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(stringContent);

            var noSentence = doc.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("no-sentences"));
            if (noSentence.Any()) {
                return 0;
            }

            var imageBuff = doc.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-container-images-space"))
                .Single();

            var image = imageBuff.Descendants("img")
                .Select(img => img.Attributes["src"].Value)
                .Single();

            var main = doc.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-container-up-inner"))
                .Single();
            
            var audio1 = main.Descendants("a")
                .Select(a => a.Attributes["href"].Value)
                .First();

            var thai1 = main.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-main-space-text"))
                .Single().InnerText.Trim();

            var romanize1 = main.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space-text  romanization"))
                .Single().InnerText.Trim();

            var eng1 = main.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space-text big english"))
                .Single().InnerText.Trim();
            
            this.MyExamples.Add(new Examples
            {
                Thai = new Content
                {
                    Audio = audio1,
                    Text = thai1
                },
                Eng = new Content
                {
                    Romanize = romanize1,
                    Text = eng1
                },
                Img = image
            });


            var examples = doc.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-container-down-inner"))
                .Single();

            var exampleThais = examples.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-down-space"));

            
            foreach (HtmlNode exampleThai in exampleThais)
            {
                var audio2 = exampleThai.Descendants("a")
                    .Select(a => a.Attributes["href"].Value)
                    .First();
                var thai2 = exampleThai.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-main-space-text"))
                .Single().InnerText.Trim();

                this.MyExamples.Add(new Examples
                {
                    Thai = new Content
                    {
                        Audio = audio2,
                        Text = thai2
                    }
                });
            }

            var exampleEngs = examples.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space"));

            int i = 1;
            foreach (HtmlNode exampleEng in exampleEngs)
            {
                var romanize2 = exampleEng.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space-text  big romanization"))
                .Single().InnerText.Trim();
                var audio3 = exampleEng.Descendants("a")
                    .Select(a => a.Attributes["href"].Value)
                    .First();
                var eng2 = exampleEng.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space-text big english"))
                .Single().InnerText.Trim();

                this.MyExamples[i].Eng = new Content
                {
                    Romanize = romanize2,
                    Audio = audio3,
                    Text = eng2
                };
                i++;
            }
            
            if (!string.IsNullOrEmpty(this.MyExamples[0].Thai.Text))
            {
                return 1;
            }
            else {
                return 0;
            }
        }

        public struct Content
        {
            public string Audio { get; set; }
            public string Text { get; set; }
            public string Romanize { get; set; }
        }

        public class Examples
        {
            public Content Thai;
            public Content Eng;
            public String Img;
        }

    }
    
}
