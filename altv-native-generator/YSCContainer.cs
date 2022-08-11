using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltV.Native.Generator
{
    internal class YSCContainer
    {
        private Dictionary<string, SortedDictionary<uint, YSCFile.NativeInfo>> _files = new Dictionary<string, SortedDictionary<uint, YSCFile.NativeInfo>>();
        private DirectoryInfo _directory;
        public YSCContainer(string path)
        {
            if (File.GetAttributes(path) != FileAttributes.Directory)
                throw new ArgumentException($"Specified path is not a directory: \"{path}\"");

            _directory = new DirectoryInfo(path);
            Utils.Log.Info("Get files in path: {0}", _directory.FullName);
            foreach(var file in _directory.GetFiles("*.ysc", SearchOption.AllDirectories))
            {
                YSCFile yscFile = new YSCFile(file.FullName);
                var a = yscFile.GetNativeDictionary();

                _files[Path.GetFileName(file.FullName)] = a;

                yscFile.Close();
            }

            string outputFile = $"{Path.GetFileName(path)}.json";
            using (StreamWriter output = File.CreateText(outputFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(output, _files);

                Utils.Log.Info("Saved result to \"{0}\"", outputFile);
            }

            Utils.Log.Info("Done: {0}", _files.Count());
        }
    }
}
