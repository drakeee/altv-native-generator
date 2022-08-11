using RageKit;
using RageKit.GameFiles;
using AltV.Native.Generator;
using AltV.Native.Generator.Resources;
using System.Diagnostics;
using CommandLine;
using CommandLine.Text;
using System.Net;

ParserResult<CommandLineOptions> parseResult = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args);
if (parseResult.Tag == ParserResultType.NotParsed)
    return;

List<uint> GtaBuildVersions = new List<uint>()
{
    323, 350, 372, 393, 463, 505, 573, 617,
    678, 757, 791, 877, 944, 1011, 1103, 1180,
    1290, 1365, 1493, 1604, 1734, 1868, 2060, 2189,
    2245, 2372, 2545, 2612, 2699
};

CommandLineOptions cli = parseResult.Value;
#region Extract current version
if (cli.ExtractCurrentVersion)
{
    Utils.Log.Info("Loading GTA 5 crypt keys");
    GTA5Keys.PC_AES_KEY = Keys.gtav_aes_key;
    GTA5Keys.PC_NG_KEYS = CryptoIO.ReadNgKeys(Keys.gtav_ng_key);
    GTA5Keys.PC_NG_DECRYPT_TABLES = CryptoIO.ReadNgTables(Keys.gtav_ng_decrypt_tables);
    GTA5Keys.PC_NG_ENCRYPT_TABLES = CryptoIO.ReadNgTables(Keys.gtav_ng_encrypt_tables);
    GTA5Keys.PC_NG_ENCRYPT_LUTs = CryptoIO.ReadNgLuts(Keys.gtav_ng_encrypt_luts);
    GTA5Keys.PC_LUT = Keys.gtav_hash_lut;

    cli.GtaPath = String.IsNullOrEmpty(cli.GtaPath) ? "C:\\StemDl\\steamapps\\common\\Grand Theft Auto V" : cli.GtaPath;
    DirectoryInfo gtaPathInfo = new DirectoryInfo(cli.GtaPath);
    string gtaExePath = Path.Combine(gtaPathInfo.ToString(), "GTA5.exe");
    if (File.Exists(gtaExePath))
        Utils.Log.Info("GTA5.exe was found in GTA V path");

    FileVersionInfo gtaVersionInfo = FileVersionInfo.GetVersionInfo(gtaExePath);
    string GtaBuildVersion = "<unknown>";
    string GtaPatchVersion = "<unknown>";

    if (gtaVersionInfo.FileVersion is not null)
    {
        List<string> GtaVersions = gtaVersionInfo.FileVersion.Split('.').ToList();
        GtaBuildVersion = GtaVersions.ElementAt(2);
        GtaPatchVersion = GtaVersions.ElementAt(3);

        Utils.Log.Info("GTA 5 exe version: {0}", gtaVersionInfo.FileVersion);
        Utils.Log.Info("Build version: {0}", GtaBuildVersion);
        Utils.Log.Info("Patch version: {0}", GtaPatchVersion);
    }

    Utils.Log.Info("Creating scripts directory for further extracting");
    DirectoryInfo scriptsDirectory = Directory.CreateDirectory($"scripts_{GtaBuildVersion}");

    Utils.Log.Info("Download GTA 5 background scripts");
    string bgBaseUrl = "https://prod.cloud.rockstargames.com/titles/gta5/pcros/bgscripts/bg_ng_{0}_{1}.rpf";
    string bgFileUrl = String.Format(bgBaseUrl, GtaBuildVersion, GtaPatchVersion);

    if (Utils.CheckFileURLExists(bgFileUrl))
    {
        Utils.Log.Info("Background scripts found at \"{0}\"", bgFileUrl);
        DirectoryInfo bgScriptDir = Directory.CreateDirectory(Path.Combine("bgscripts", GtaBuildVersion.ToString(), GtaPatchVersion.ToString()));

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(bgFileUrl);

            string bgFilePath = Path.Combine(bgScriptDir.FullName, "bgscript.rpf");
            using (var fs = File.OpenWrite(bgFilePath))
            {
                await response.Content.CopyToAsync(fs);
                fs.Close();
            }

            RpfFile bgFile = new RpfFile(bgFilePath, Path.GetPathRoot(bgFilePath));
            bgFile.ScanStructure((string status) => { }, (string errorLog) => { });

            foreach (var fileEntry in bgFile.AllEntries)
            {
                if (fileEntry.Name.EndsWith(".ysc"))
                {
                    string fileOutput = Path.Combine(scriptsDirectory.FullName, String.Format("{0}_{1}_{2}.ysc", fileEntry.GetShortName(), GtaBuildVersion, GtaPatchVersion));
                    byte[] scriptData = fileEntry.File.ExtractFile(fileEntry as RpfResourceFileEntry);

                    using (var scriptWriter = new BinaryWriter(File.OpenWrite(fileOutput)))
                    {
                        scriptWriter.Write(scriptData);
                        scriptWriter.Close();
                    }
                }
            }
        }
    }

    Utils.Log.Info("Scanning GTA 5 path for file infos");
    RpfManager rpfManager = new RpfManager();
    rpfManager.Init(cli.GtaPath, (string status) => { Utils.Log.Status(status); }, (string error) => { }, false, false);

    Utils.Log.Info("Search for *.ysc files");
    foreach (var entry in rpfManager.SearchFile(".ysc"))
    {
        string extractDirectory = Path.Combine(scriptsDirectory.FullName, entry.Name.ToString().Split('.')[0]);
        Utils.Log.Info("Extracting \"{0}\" script into {1}", entry.Name, Path.GetRelativePath(Directory.GetCurrentDirectory(), extractDirectory));

        DirectoryInfo extractDirectoryInfo = Directory.CreateDirectory(extractDirectory);
        string extractPath = Path.Combine(extractDirectoryInfo.FullName, entry.Name);

        //entry.File.ExtractScripts(extractPath, (string _) => { });
        byte[] scriptData = entry.File.ExtractFile(entry as RpfResourceFileEntry);
        using var scriptWriter = new BinaryWriter(File.OpenWrite(extractPath));
        scriptWriter.Write(scriptData);
        scriptWriter.Close();
    }

    return;
}
#endregion

#region Building crossmap
cli.BuildVersionSource = 2545;
cli.BuildVersionTarget = 2699;

if (cli.BuildVersionSource == 0 || cli.BuildVersionTarget == 0)
{
    Utils.Log.Error("Source or Target version can't be zero! Exiting...");
    return;
}

if (!GtaBuildVersions.Contains(cli.BuildVersionSource) || !GtaBuildVersions.Contains(cli.BuildVersionTarget))
{
    Utils.Log.Error("Source or Target version doesn't exists in GtaBuildVersions array! Exiting...");
    return;
}

if (cli.BuildVersionSource > cli.BuildVersionTarget)
{
    Utils.Log.Error("Target version is lower then the Source version! Exiting...");
    return;
}

string SourcePath = Path.Combine(Directory.GetCurrentDirectory(), String.Format("scripts_{0}", cli.BuildVersionSource.ToString()));
if (!Directory.Exists(SourcePath))
{
    Utils.Log.Error("Source path: {0}", SourcePath);
    Utils.Log.Error($"Source folder \"scripts_{cli.BuildVersionSource}\" was not found! Exiting...");
    return;
}

string TargetPath = Path.Combine(Directory.GetCurrentDirectory(), String.Format("scripts_{0}", cli.BuildVersionTarget.ToString()));
if (!Directory.Exists(TargetPath))
{
    Utils.Log.Error($"Source folder \"scripts_{cli.BuildVersionTarget}\" was not found! Exiting...");
    return;
}

YSCContainer sourceContainer = new YSCContainer(SourcePath);
YSCContainer targetContainer = new YSCContainer(TargetPath);
#endregion
/*

//buildVersion = "2545";

Utils.Log.Info("Process \"*.ysc\" script files");
Dictionary<ulong, bool> nativeHashDict = new Dictionary<ulong, bool>();

//foreach(var file in scriptsDirectory.GetFiles("*.ysc", SearchOption.AllDirectories))
//{

//    YSCFile yscFile = new YSCFile(file.FullName);

//    Utils.Log.Debug("Native counter: {0} - {1}", file.Name, yscFile.Header.NativesCount);

//    foreach(var hash in yscFile.AllHashes)
//    {
//        nativeHashDict[hash] = true;
//    }

//    //Release memory
//    yscFile.Close();

//    break;

//    //GC.Collect();
//    //GC.WaitForPendingFinalizers();
//}

Utils.Log.Info("Processing done. Collected {0} natives", nativeHashDict.Count);*/