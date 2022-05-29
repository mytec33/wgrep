using System;
using System.Collections.Generic;
using System.Text;

namespace wgrep // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private const string ANSI_CODE_START_BLUE = "\x1b[34m";
        private const string ANSI_CODE_START_RED = "\x1b[31m";
        private const string ANSI_CODE_STOP = "\x1b[0m";

        private static Queue<string> afterQueue = new();
        private static Queue<string> beforeQueue = new();
        private static bool shownBeforeLines = false;


        private static ArgsConfig config = new();

        static void Main(string[] args)
        {
            int bufferSize = 16384;
            long matchCount = 0;

            if (!config.ParseArgs(args))
            {
                DisplayHelp();
                Environment.Exit(1);
            }

            string[] files = GetFiles();
            foreach (string file in files)
            {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new(fs, Encoding.UTF8, true, bufferSize);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(config.SearchPhrase))
                    {
                        if (config.Count)
                        {
                            matchCount++;
                            continue;
                        }

                        DisplayBeforeLines();
                        DisplayLine(line);
                        DisplayAfterLines();
                    }
                    else
                    {
                        AddBeforeLine(line);
                    }
                }
            }

            if (config.Count)
            {
                Console.WriteLine(matchCount);
            }
        }

        private static void AddBeforeLine(string line)
        {
            if (config.LinesBefore > 0 && !config.Count)
            {
                if (beforeQueue.Count >= config.LinesBefore)
                {
                    beforeQueue.Dequeue();
                }
                beforeQueue.Enqueue(line);
            }
        }

        private static void DisplayAfterLines()
        {

        }

        private static void DisplayBeforeLines()
        {
            if (config.TrackingLinesBefore && shownBeforeLines && beforeQueue.Count >= config.LinesBefore)
            {
                DisplaySeparator("--");
            }

            foreach (string qLine in beforeQueue)
            {
                Console.WriteLine(qLine);
            }

            shownBeforeLines = true;
            beforeQueue.Clear();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("usage: wgrep [-A num] [-B num] searchPattern filepath");
        }

        private static void DisplayLine(string line)
        {
            if (config.Color)
            {
                line = line.Replace(config.SearchPhrase, ANSI_CODE_START_RED + config.SearchPhrase + ANSI_CODE_STOP);
            }

            Console.WriteLine(line);
        }

        private static void DisplaySeparator(string line)
        {
            if (config.Color)
            {
                line = ANSI_CODE_START_BLUE + "--" + ANSI_CODE_STOP;
            }

            Console.WriteLine(line);
        }

        private static string[] GetFiles()
        {
            // If we have a match, we were passed a single file
            bool fileExists = File.Exists(config.FilePath);
            if (fileExists)
            {
                string[] singleFile = new string[1];
                singleFile[0] = config.FilePath;
                return singleFile;
            }

            // Check to see if any wildcards were used
            string? fileDir = Path.GetDirectoryName(config.FilePath);
            string fileGlob = Path.GetFileName(config.FilePath);

            if (fileDir == null)
            {
                Console.WriteLine("Directory not found.");
            }
            else
            {
                Console.WriteLine($"File directory: {fileDir}");
                Console.WriteLine($"File glob: {fileGlob}");

                string[] files = Directory.GetFiles(fileDir, fileGlob);
                if (files.Length > 0)
                {
                    return files;
                }
            }

            // Don't see a match, return empty
            Console.WriteLine("No file(s) found.");
            return Array.Empty<string>();
        }
    }
}
