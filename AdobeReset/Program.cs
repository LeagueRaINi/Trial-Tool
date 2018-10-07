using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AdobeReset.Helpers;

namespace AdobeReset
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Adobe Reset - by RaIN";
            Console.SetWindowSize(Console.LargestWindowWidth - 20, Console.LargestWindowHeight - 20);

            try
            {
                var adobePath = Path.Combine(Environment.Is64BitOperatingSystem
                    ? Environment.GetEnvironmentVariable("ProgramW6432")
                    : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Adobe");
                if (!Directory.Exists(adobePath))
                    throw new Exception("could not find adobe main folder");

                var potentialProductFolders = new DirectoryInfo(adobePath).GetDirectories().Where(x => x.Name.StartsWith("Adobe")).ToList();
                if (!potentialProductFolders.Any())
                    throw new Exception($"could not find any adobe product in {adobePath}");

                var randomChars = "0123456789".ToArray();
                Parallel.ForEach(potentialProductFolders, (folder) =>
                {
                    Logger.Debug("scanning folder: {0}", folder);

                    var applicationFiles = Directory.GetFiles(folder.FullName, "application.xml", SearchOption.AllDirectories);
                    if (!applicationFiles.Any())
                        return;

                    foreach (var applicationFile in applicationFiles)
                    {
                        Logger.Debug("searching key in {0}", applicationFile);

                        var applicationDoc = XDocument.Load(applicationFile);
                        var needsSave = false;

                        foreach (var trialKey in applicationDoc.Descendants("Data").Where(x => (string)x.Attribute("key") == "TrialSerialNumber").ToList())
                        {
                            var key = trialKey.Value;
                            if (key.Length < 15)
                                continue;

                            var randomNumberLength = key.Length - 15;
                            var randomKey = key.Remove(15, randomNumberLength) + RandomGenerator.String(randomNumberLength, randomChars);

                            trialKey.SetValue(randomKey);

                            Logger.Debug("generated new key ({0}) for file {1}", randomKey, applicationFile);

                            needsSave = true;
                        }

                        if (!needsSave)
                        {
                            Logger.Warning("no key found in file {0}", applicationFile);
                            continue;
                        }

                        applicationDoc.Save(applicationFile);

                        Logger.Success("refreshed key in {0}", applicationFile);
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }

            Logger.Info("press any key to exit");

            Console.ReadKey();
        }
    }
}
