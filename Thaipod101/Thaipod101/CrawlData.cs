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
        //public String Image { set; get; }

        public CrawlData()
        {
            this.MyExamples = new List<Examples>();
        }

        public void test()
        {
            Debug.WriteLine("Hello Test");
        }

        //public async void DownloadHomepage(String text)
        public async Task<int> DownloadHomepage(String text)
        {
            var httpClient = new HttpClient(); // Xamarin supports HttpClient!

            //Task<string> contentsTask = httpClient.GetStringAsync("http://www.thaipod101.com/widget/wotd/large.php"); // async method!
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("language", "Thai"),
                new KeyValuePair<string, string>("date", text)
            });

            var response = await httpClient.PostAsync("http://www.thaipod101.com/widget/wotd/large.php", formContent); // async method!

            // await! control returns to the caller and the task continues to run on another thread
            var stringContent = await response.Content.ReadAsStringAsync();
            //var byteArray = await response.Content.ReadAsByteArrayAsync();
            //var stringContent = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(stringContent);

            var imageBuff = doc.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-container-images-space"))
                .Single();

            var image = imageBuff.Descendants("img")
                .Select(img => img.Attributes["src"].Value)
                .Single();

            var main = doc.DocumentNode.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-container-up-inner"))
                .Single();

            //Debug.WriteLine(main.InnerHtml);

            var audio1 = main.Descendants("a")
                .Select(a => a.Attributes["href"].Value)
                .First();
            //Debug.WriteLine(audio1);

            var thai1 = main.Descendants("span")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-main-space-text"))
                .Single().InnerText.Trim();
            //Debug.WriteLine(thai1);

            var romanize1 = main.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space-text  romanization"))
                .Single().InnerText.Trim();
            //Debug.WriteLine(romanize1);

            var eng1 = main.Descendants("div")
                .Where(n => n.GetAttributeValue("class", "").Equals("wotd-widget-sentence-quizmode-space-text big english"))
                .Single().InnerText.Trim();
            //Debug.WriteLine(eng1);

            //List<Examples> MyExamples = new List<Examples>();
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
                Debug.WriteLine(audio2);
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


            //ResultEditText.Text += "DownloadHomepage method continues after async call. . . . .\n";

            // After contentTask completes, you can calculate the length of the string.
            //int exampleInt = stringContent.Length;

            //ResultEditText.Text += "Downloaded the html and found out the length.\n\n\n";

            //ResultEditText.Text += contents; // just dump the entire HTML

            //return exampleInt; // Task<TResult> returns an object of type TResult, in this case int
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
