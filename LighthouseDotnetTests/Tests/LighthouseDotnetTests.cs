using LighthouseDotnet;
using LighthouseDotnet.Objects;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.IO;
using static LighthouseDotnet.Objects.AuditRequest;

namespace LighthouseDotnetTests.Tests
{
    public class LighthouseDonetTests
    {

        [Test]
        public async Task NpmExistTest()
        {
            var lh = new Lighthouse();
            var res = await lh.Run("http://example.com");

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Performance);
            Assert.IsTrue(res.Performance > 0.5m);

            Assert.IsNotNull(res.Accessibility);
            Assert.IsTrue(res.Accessibility > 0.5m);

            Assert.IsNotNull(res.BestPractices);
            Assert.IsTrue(res.BestPractices > 0.5m);

            Assert.IsNotNull(res.Pwa);
            Assert.IsTrue(res.Pwa > 0.2m);

            Assert.IsNotNull(res.Seo);
            Assert.IsTrue(res.Seo > 0.2m);
        }

        [Test]
        public async Task GetJsonReport()
        {
            var lh = new Lighthouse();
            var res = await lh.Run("http://example.com");

            var reportsDir = "./lighthouse-reports";
            var filename = $"{Guid.NewGuid()}--{DateTime.Now:dd-MM-yyyy--HH-mm-ss}";
            
            var reportPath = lh.SaveReportJson(reportsDir, filename, res);
            Console.WriteLine(reportPath);

            var reportContent = File.ReadAllText(reportPath);
            Assert.False(String.IsNullOrEmpty(reportContent.Trim()));
        }

        [Test]
        public async Task GetHtmlReport()
        {
            var lh = new Lighthouse();
            var res = await lh.Run("http://example.com");

            var reportsDir = "./lighthouse-reports";
            var filename = $"{Guid.NewGuid()}--{DateTime.Now:dd-MM-yyyy--HH-mm-ss}";

            var reportPath = lh.SaveReportHtml(reportsDir, filename, res);
            Console.WriteLine(reportPath);

            var reportContent = File.ReadAllText(reportPath);
            Assert.False(String.IsNullOrEmpty(reportContent.Trim()));
        }

        [Test]
        public async Task GetCsvReport()
        {
            var lh = new Lighthouse();
            var res = await lh.Run("http://example.com");

            var reportsDir = "./lighthouse-reports";
            var filename = $"{Guid.NewGuid()}--{DateTime.Now:dd-MM-yyyy--HH-mm-ss}";

            var reportPath = lh.SaveReportCsv(reportsDir, filename, res);
            Console.WriteLine(reportPath);

            var reportContent = File.ReadAllText(reportPath);
            Assert.False(String.IsNullOrEmpty(reportContent.Trim()));
        }

        [Test]
        public async Task ScreenShots()
        {
            var lh = new Lighthouse();
            var res = await lh.Run("http://example.com");

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.FinalScreenshot);
            Assert.IsFalse(String.IsNullOrWhiteSpace(res.FinalScreenshot.Base64Data));

            Assert.IsNotNull(res.Thumbnails);
            Assert.IsFalse(res.Thumbnails.Count == 0);
            Assert.IsFalse(String.IsNullOrWhiteSpace(res.Thumbnails[0].Base64Data));
        }

        [Test]
        public async Task FormFactorTest()
        {
            var lh = new Lighthouse();
            var ar = new AuditRequest("http://example.com")
            {
                EmulatedFormFactor = FormFactor.Desktop
            };

            var res = await lh.Run(ar);

            Assert.IsNotNull(res);
        }

        [Test]
        [Ignore("TODO")]
        public async Task OnlyCategoriesTest()
        {
            var lh = new Lighthouse();
            var ar = new AuditRequest("http://example.com")
            {
                OnlyCategories = new[]
                {
                    Category.Performance,
                },
                EnableLogging = true
            };
            var res = await lh.Run(ar);

            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Performance);
            Assert.IsTrue(res.Performance > 0.5m);
            Assert.IsNull(res.Accessibility);
        }
    }
}
