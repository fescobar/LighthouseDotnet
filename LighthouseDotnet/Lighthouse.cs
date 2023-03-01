using LighthouseDotnet.Core;
using LighthouseDotnet.Objects;
using NLog;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Logger = NLog.Logger;

namespace LighthouseDotnet
{
    public sealed class Lighthouse
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<AuditResult> Run(string urlWithProtocol)
        {
            return await Run(new AuditRequest(urlWithProtocol)).ConfigureAwait(false);
        }
        public Task<AuditResult> Run(AuditRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return RunAfterCheck(request);
        }

        public string SaveReportJson(string reportsDir, string fileName, AuditResult result)
        {
            return SaveReport(reportsDir, fileName, "json", result.ReportJson);
        }

        public string SaveReportHtml(string reportsDir, string fileName, AuditResult result)
        {
            return SaveReport(reportsDir, fileName, "html", result.ReportHtml);
        }

        public string SaveReportCsv(string reportsDir, string fileName, AuditResult result)
        {
            return SaveReport(reportsDir, fileName, "csv", result.ReportCsv);
        }

        private string SaveReport(string reportsDir, string fileName, string extension, string content)
        {
            Directory.CreateDirectory(reportsDir);
            var fullFileName = $"{reportsDir}/{fileName}.{extension}";
            File.WriteAllText(fullFileName, content);
            return Path.GetFullPath(fullFileName);
        }

        private async Task<AuditResult> RunAfterCheck(AuditRequest request)
        {
            var nodePath = "";
            if (OSystem.IsLinux)
            {
                var bashTestminal = new BashTerminal();
                bashTestminal.EnableDebugging = request.EnableLogging;
                nodePath = await bashTestminal.GetNodePath().ConfigureAwait(false);
            }
            else
            {
                var whereCmd = new WhereCmd();
                whereCmd.EnableDebugging = request.EnableLogging;
                nodePath = await whereCmd.GetNodePath().ConfigureAwait(false);
            }

            if (String.IsNullOrEmpty(nodePath) || !File.Exists(nodePath)) throw new Exception("Couldn't find NodeJs. Please, install NodeJs and make sure than PATH variable defined.");

            var npm = new Npm(nodePath)
            {
                EnableDebugging = request.EnableLogging
            };
            var npmPath = await npm.GetNpmPath().ConfigureAwait(false);

            var version = await npm.GetLighthouseVersion().ConfigureAwait(false);
            await npm.InstallLighthouseDependencies(version.FullVersion);

            var sm = new ScriptMaker();
            var content = sm.Produce(request, npmPath, version);

            if (!sm.Save(content)) throw new Exception($"Couldn't save JS script to %temp% directory. Path: {sm.TempFileName}");

            try
            {
                var node = new Node()
                {
                    EnableDebugging = request.EnableLogging
                };
                var stdoutJson = await node.Run(sm.TempFileName).ConfigureAwait(false);
                return AuditResult.Parse(stdoutJson);
            }
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(ex.Message) && Regex.IsMatch(ex.Message, @"Cannot find module[\s\S]+?node_modules\\lighthouse'"))
                {
                    throw new Exception("Lighthouse is not installed. Please, execute `npm install -g lighthouse` in console.");
                }
                throw;
            }
            finally
            {
                if (!npm.EnableDebugging) sm.Delete();
            }
        }

    }
}
