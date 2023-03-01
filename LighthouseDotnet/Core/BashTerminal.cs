using System.Threading.Tasks;

namespace LighthouseDotnet.Core
{
    internal sealed class BashTerminal : TerminalBase
    {
        protected override string FileName => "which";

        internal async Task<string> GetNodePath()
        {
            return await this.Execute("node");
        }
    }
}
