using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thaipod101;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using Nito.AsyncEx.UnitTests;

namespace UnitTestProject1
{
    [TestClass]
    public class CrawlDataTests
    {
        [TestMethod]
        public async Task testTests()
        {
            CrawlData crawlData = new CrawlData();
            crawlData.test();
            int resultTsak = await crawlData.DownloadHomepage("2016-11-20");
            Debug.WriteLine(crawlData.MyExamples[0].Thai.Text);
            Assert.AreEqual<int>(1, resultTsak);
        }
    }
}
