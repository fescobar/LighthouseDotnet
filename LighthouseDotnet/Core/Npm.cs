using LighthouseDotnet.Objects;
using System;
using System.Threading.Tasks;
using System.IO;

namespace LighthouseDotnet.Core
{
    internal sealed class Npm : TerminalBase
    {
        protected override string FileName { get; }

        private Npm()
        {
        }

        public Npm(string nodePath)
        {
            if (OSystem.IsLinux)
            {
                this.FileName = nodePath.Replace("node", "npm");
            }
            else
            {
                this.FileName = nodePath.Replace("node.exe", "npm.cmd");
            }
        }

        internal async Task<string> GetNpmPath()
        {
            var rsp = await this.Execute("config get prefix");
            if (String.IsNullOrEmpty(rsp)) throw new Exception("Couldn't detect global node_modules path.");
            return rsp.Trim();
        }

        internal async Task<NpmPackageVersion> GetLighthouseVersion()
        {
            var command = "list -g lighthouse";
            var rsp = await this.Execute(command);
            var index = !String.IsNullOrEmpty(rsp) ? rsp.LastIndexOf("@") : -1;
            if (rsp == null || index == -1) throw new Exception("Couldn't detect lighthouse version.");
            return new NpmPackageVersion(rsp.Substring(index + 1).Trim());
        }

        internal async Task<bool> InstallLighthouseDependencies(string fullVersion)
        {
            string tempPath = Path.GetTempPath();
            var command = $"i lighthouse@{fullVersion} --save-dev --loglevel=error --prefix {tempPath}/lh";
            var rsp = await this.Execute(command);
            if (rsp == null) throw new Exception("Couldn't install lighthouse version.");
            return true;
        }

        protected override void OnError(string message)
        {
            throw new Exception(message);
        }

    }
}
