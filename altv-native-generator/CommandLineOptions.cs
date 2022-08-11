using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AltV.Native.Generator
{
    public class CommandLineOptions
    {
        [Option("extract-current-version", Required = false, HelpText = "Extracting current version of script files.")]
        public bool ExtractCurrentVersion { get; set; } = false;

        [Option("gta-path", Required = false, HelpText = "Set GTA 5 path manually.")]
        public string GtaPath { get; set; } = "";

        [Option("source-version", Required = false, HelpText = "Set GTA 5 build version where to build crossmap from.")]
        public uint BuildVersionSource { get; set; } = 0;

        [Option("target-version", Required = false, HelpText = "Set GTA 5 build version where to build crossmap to.")]
        public uint BuildVersionTarget { get; set; } = 0;
    }
}
