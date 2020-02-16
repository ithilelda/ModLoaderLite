using System.Collections.Generic;
using System.IO;

namespace ModLoader
{
    public static class Configuration
    {
        public static Dictionary<string, string> ReadAll(string path, string name)
        {
            var fullName = Path.Combine(path, name) + ".cfg";
            var result = new Dictionary<string, string>();
            if(File.Exists(fullName))
            {
                using (var reader = new StreamReader(fullName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var tokens = line.Split('=');
                        if (tokens.Length == 2)
                        {
                            result.Add(tokens[0].Trim(), tokens[1].Trim());
                        }
                    }
                }
            }
            return result;
        }

        public static void WriteAll(Dictionary<string, string> configs, string path, string name)
        {
            var fullName = Path.Combine(path, name) + ".cfg";
            using (var writer = new StreamWriter(fullName))
            {
                foreach(var pair in configs)
                {
                    writer.WriteLine($"{pair.Key}={pair.Value}");
                }
            }
        }
    }
}