using LighthouseDotnet.Objects;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LighthouseDotnet.Core
{
    internal sealed class ScriptMaker
    {
        internal string TempFileName { get; private set; }
        internal ScriptMaker()
        {
        }

        internal string Produce(AuditRequest request, string npmPath, NpmPackageVersion lighthouseVersion)
        {
            if (request == null) return null;
            var data = this.getTemplate();

            var jsOptions = new LighthouseJsOptions()
            {
                chromeFlags = new[]
                {
                    "--show-paint-rects",
                    "--headless",
                    "--no-sandbox",
                    "--disable-gpu"
                },
                maxWaitForLoad = request.MaxWaitForLoad,
                blockedUrlPatterns = request.BlockedUrlPatterns,
                disableStorageReset = request.DisableStorageReset,
                disableDeviceEmulation = request.DisableDeviceEmulation,
                OnlyCategories = request.OnlyCategories,

                emulatedFormFactor = request.EmulatedFormFactor?.ToString().ToLower()
            };
            // https://github.com/GoogleChrome/lighthouse/blob/master/docs/emulation.md
            if (lighthouseVersion.MajorVersion >= 7)
            {
                jsOptions.preset = jsOptions.emulatedFormFactor;
                jsOptions.emulatedFormFactor = null;
            }

            var optionsAsJson = JsonConvert.SerializeObject(jsOptions,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            data = data.Replace("{OPTIONS}", optionsAsJson).Replace("{URL}", request.Url);
            return data;
        }
        public bool Save(string content)
        {
            this.TempFileName = null;

            string tempPath = Path.GetTempPath();

            var fullPath = $"{tempPath}/lh/lighthouse-net-{Guid.NewGuid():N}.mjs";
            try
            {
                File.WriteAllText(fullPath, content);
                this.TempFileName = fullPath;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete()
        {
            if (String.IsNullOrEmpty(this.TempFileName)) return false;
            try
            {
                File.Delete(this.TempFileName);
                return true;
            }
            catch (Exception)
            {
                // ignore
            }
            return false;
        }

        private string getTemplate()
        {
            return @"
import lighthouse from 'lighthouse';
import chromeLauncher from 'chrome-launcher';

const chrome = await chromeLauncher.launch({OPTIONS});
const options = { output: ['json', 'html', 'csv'], port: chrome.port};

try {
    const runnerResult = await lighthouse('{URL}', options);
    const reports = {
        json: runnerResult.report[0],
        html: runnerResult.report[1],
        csv: runnerResult.report[2],
    }
    console.log(JSON.stringify(reports))
} finally {
    await chrome.kill();
}
";
        }
    }
}
