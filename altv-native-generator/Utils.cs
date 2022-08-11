using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using RageKit.GameFiles;

namespace AltV.Native.Generator
{
    public static class StreamExtensions
    {
        public static T ReadStruct<T>(this Stream stream) where T : struct
        {
            var sz = Marshal.SizeOf(typeof(T));
            var buffer = new byte[sz];
            stream.Read(buffer, 0, sz);
            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(
                pinnedBuffer.AddrOfPinnedObject(), typeof(T));
            pinnedBuffer.Free();
            return structure;
        }
    }

    static class Utils
	{

        public static bool CheckFileURLExists(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            // instruct the server to return headers only
            request.Method = "HEAD";

            // make the connection
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                // get the status code
                HttpStatusCode status = response.StatusCode;

                return status == HttpStatusCode.OK;
            }
            catch(WebException e)
            {
                return false;
            }
        }

        public static class Log
        {

            private static void Print(string message, ConsoleColor color = ConsoleColor.Blue, bool newLine = true)
            {
                if(!newLine)
                {
                    Console.Title = message;
                    return;
                }

                StackTrace stackTrace = new StackTrace();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                //Console.Write($"{stackTrace.GetFrame(2).GetMethod().DeclaringType} - ");
                var stack = stackTrace.GetFrame(2).GetMethod();
                if(stack.DeclaringType.ReflectedType != null)
                    Console.Write($"{stackTrace.GetFrame(2).GetMethod().DeclaringType.ReflectedType.Name}");
                else
                    Console.Write($"{stackTrace.GetFrame(2).GetMethod().DeclaringType}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("] ");
                Console.ForegroundColor = color;
                Console.Write($"{stackTrace.GetFrame(1).GetMethod().Name.ToUpper()} ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(message);

                Console.Title = message;
                Console.ResetColor();
            }

            public static void Info(string message, params object?[] args) => Print(String.Format(message, args));
            public static void Error(string message, params object?[] args) => Print(String.Format(message, args), ConsoleColor.Red);
            public static void Debug(string message, params object?[] args) => Print(String.Format(message, args), ConsoleColor.Green);
            public static void Warning(string message, params object?[] args) => Print(String.Format(message, args), ConsoleColor.Yellow);
            public static void Status(string message, params object?[] args) => Print(String.Format(message, args), ConsoleColor.Gray, false);
        }

        public static List<RpfEntry> SearchFile(this RpfManager manager, string search)
        {
            return manager.EntryDict.Where(x => x.Value.Name.Contains(search)).Select(x => x.Value).ToList();
        }

        public static List<RpfEntry> SearchFiles(RpfFile rpfFile, List<string> filesToSearch)
        {
            List<RpfEntry> filesList = new List<RpfEntry>();
            foreach (var child in rpfFile.Children)
            {
                filesList.AddRange(SearchFiles(child, filesToSearch));
            }

            foreach (var entry in rpfFile.AllEntries)
            {
                foreach (var search in filesToSearch)
                    if (entry.Name.Contains(search))
                        filesList.Add(entry);
            }

            return filesList;
        }

        public static List<RpfEntry> SearchFiles(RpfFile rpfFile, string fileName)
        {
            List<RpfEntry> filesList = new List<RpfEntry>();
            foreach (var child in rpfFile.Children)
            {
                filesList.AddRange(SearchFiles(child, fileName));
            }

            foreach (var entry in rpfFile.AllEntries)
            {
                if (entry.Name.StartsWith(fileName))
                    filesList.Add(entry);
            }

            return filesList;
        }
    }
}
