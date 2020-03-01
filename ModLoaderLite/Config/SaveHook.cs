using System;
using System.IO;
using System.Runtime.Serialization;

namespace ModLoaderLite.Config
{
    static class SaveHook
    {
        public static void DoSavePrefix(string name)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), $"saves/{name}.mll");
            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                try
                {
                    Configuration.Serialize(fs);
                }
                catch(SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
            }
        }
    }
}
