using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AdobeReset
{
    class Program
    {
        static string adobeFolderPath = Path.Combine(Environment.Is64BitOperatingSystem
            ? Environment.GetEnvironmentVariable("ProgramW6432")
            : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Adobe");

        static void Main(string[] args)
        {
            Console.Title = "Adobe Trial Reset Tool -by RaINi";

            if (!Directory.Exists(adobeFolderPath))
            {
                LogError($"could not find folder '{adobeFolderPath}'");
                goto EXIT;
            }

            Log("----------------------------------------------]");

            IEnumerable<DirectoryInfo> potentialProductFolders = new DirectoryInfo(adobeFolderPath).GetDirectories().Where(x => x.Name.StartsWith("Adobe"));

            if (!potentialProductFolders.Any())
            {
                LogError($"could not find any potential adobe products in '{adobeFolderPath}'");
                return;
            }

            foreach (DirectoryInfo potentialProductFolder in potentialProductFolders)
            {
                LogInfo($"scanning '{potentialProductFolder.FullName}'");

                string[] applicationFiles = Directory.GetFiles(potentialProductFolder.FullName, "application.xml", SearchOption.AllDirectories);

                if (!applicationFiles.Any())
                {
                    LogError("could not find any 'application.xml'");
                    goto ENDLOOP;
                }

                foreach (string applicationFile in applicationFiles)
                {
                    string applicationFileNameShort = applicationFile.Remove(0, potentialProductFolder.FullName.Length);

                    XDocument applicationDoc = null;

                    try
                    {
                        applicationDoc = XDocument.Load(applicationFile);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        LogError($"could not load file '{applicationFileNameShort}'");
                    }

                    IEnumerable<XElement> trialKeys = applicationDoc.Descendants("Data").Where(x => (string)x.Attribute("key") == "TrialSerialNumber");

                    if (!trialKeys.Any())
                    {
                        Log($"could not find 'TrialSerialNumber' in file '{applicationFileNameShort}'");
                        continue;
                    }

                    bool needsSave = false;

                    foreach (XElement trialKey in trialKeys)
                    {
                        string key = trialKey.Value;

                        if (key.Length < 15)
                        {
                            LogError($"key '{key} in file '{applicationFileNameShort}' had an incorrect size ({key.Length})");
                            continue;
                        }

                        int randomNumberLength = key.Length - 15;

                        string newKey = key.Remove(15, randomNumberLength) + GenerateRandomNumbers(randomNumberLength);

                        LogInfo($"trial key '{key}' in file '{applicationFileNameShort}'");
                        LogInfo($"new trial key '{newKey}'");

                        trialKey.SetValue(newKey);

                        needsSave = true;
                    }

                    if (needsSave)
                    {
                        try
                        {
                            applicationDoc.Save(applicationFile);

                            LogInfo($"saved file '{applicationFileNameShort}'");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            LogError($"could not save file '{applicationFileNameShort}'");
                        }
                    }
                }

            ENDLOOP:
                Log("----------------------------------------------]");
            }

        EXIT:
            Console.ReadKey();
        }

        #region Extensions

        static string GenerateRandomNumbers(int length)
        {
            byte[] data = new byte[1];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);

                data = new byte[length];

                crypto.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(length);

            char[] chars = "1234567890".ToArray();

            foreach (byte b in data)
                result.Append(chars[b % (chars.Length)]);

            return result.ToString();
        }

        #endregion

        #region ConsoleExtensions

        static void Log(string text)
            => Console.WriteLine($"[+] {text}");

        static void LogInfo(string info)
        {
            ConsoleColor oldColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"[?] {info}");

            Console.ForegroundColor = oldColor;
        }

        static void LogError(string error)
        {
            ConsoleColor oldColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"[!] {error}");

            Console.ForegroundColor = oldColor;
        }

        #endregion
    }
}
