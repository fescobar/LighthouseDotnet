# LighthouseDotnet
 [![Nuget](https://img.shields.io/nuget/v/lighthouse.net.svg)](https://www.nuget.org/packages/lighthouse.net)
This is a .net (c#) library for [Google Lighthouse](https://github.com/GoogleChrome/lighthouse) tool.

Lighthouse.NET analyzes web apps and web pages, collecting modern performance metrics and insights on developer best practices from your code.

*Auditing, performance metrics, and best practices for Progressive Web Apps in .NET tests.*

Compatible with newest version from dotnet5 and working from `Docker` containers.

- Note: Project based on https://github.com/dima-horror/lighthouse.net

## PRE-REQUISITES
- Install Google CHrome https://www.google.com/chrome/
- Install node from 18x version https://nodejs.org/en/download/
- Install lighthouse `10.0.2` globally
```sh
npm i lighthouse@10.0.2 -g
```
- Install lighthouse.net into your project via NuGet
```
PM> Install-Package lighthouse.net
```

## USAGE

[LighthouseDotnetTests.cs](https://github.com/fescobar/LighthouseDotnet/blob/main/LighthouseDotnetTests/Tests/LighthouseDotnetTests.cs)

```c#
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
```

```c#
        [Test]
        public async Task GetHtmlReport()
        {
            var lh = new Lighthouse();
            var res = await lh.Run("http://example.com");

            var reportsDir = "./lighthouse-reports";
            var filename = $"{Guid.NewGuid()}--{DateTime.Now:dd-MM-yyyy--HH-mm-ss}";

            var reportPath = lh.SaveReportHtml(reportsDir, filename, res);
            Console.WriteLine(reportPath);

            var report = File.ReadAllText(reportPath);
            Console.WriteLine(report);
            Assert.NotNull(report);
        }
```