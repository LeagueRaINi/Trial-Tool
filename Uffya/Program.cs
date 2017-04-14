using IniParser;
using IniParser.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Uffya
{
    class Program
    {
        private static List<string> PhotoshopDirectorys = new List<string>();
        private static List<string> AfterEffectsDirectorys = new List<string>();
        private static List<string> RegPaths = new List<string>();

        private static void AddRegPaths()
        {
            /* Photoshop RegPaths */
            RegPaths.Add("SOFTWARE\\Adobe\\Photoshop\\110.0");

            /* After Effects RegPaths */
            RegPaths.Add("SOFTWARE\\Adobe\\After Effects\\14.0");
        }

        static void Main(string[] args)
        {
            Console.Title = "Uffya Adobe Reset by RaIN";
            WriteGray("---------------------------------------------------------------------------------------------------------------->");
            WriteWhite("> Hello: " + Environment.UserName);
            WriteWhite("> This Programm will reset your Adobe Trial for: Photoshop & After Effects");
            WriteGray("---------------------------------------------------------------------------------------------------------------->");

            string UffyaDirectory = Directory.GetCurrentDirectory() + @"\" + AppDomain.CurrentDomain.FriendlyName;

            if (!File.Exists("Config.ini")) { File.WriteAllText("Config.ini", "[Autostart]\r\nEnabled = false"); }

            var IniParser = new FileIniDataParser();
            IniData Data = IniParser.ReadFile("Config.ini");

            RegistryKey Startup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (bool.Parse(Data["Autostart"]["Enabled"]))
            {
                if (Startup.GetValue("Uffya Adobe Reset") == null)
                {
                    Startup.SetValue("Uffya Adobe Reset", UffyaDirectory);
                    WriteGreen("> Uffya Adobe Reset is now registered as autostart programm");
                    WriteGray("---------------------------------------------------------------------------------------------------------------->");
                }
                else
                {
                    if (!Startup.GetValue("Uffya Adobe Reset").Equals(UffyaDirectory))
                    {
                        Startup.SetValue("Uffya Adobe Reset", UffyaDirectory);
                        WriteGreen("> Updated Autostart Path");
                        WriteGreen("> New Path: " + UffyaDirectory);
                        WriteGray("---------------------------------------------------------------------------------------------------------------->");
                    }
                }
            }
            else
            {
                if (Startup.GetValue("Uffya Adobe Reset") != null)
                {
                    Startup.DeleteValue("Uffya Adobe Reset");
                    WriteGreen("> Uffya Adobe Reset is now removed from autostart");
                    WriteGray("---------------------------------------------------------------------------------------------------------------->");
                }
            }

            AddRegPaths();
            foreach (string Path in RegPaths)
            {
                using (var RegKey = Registry.LocalMachine.OpenSubKey(Path))
                {
                    if (RegKey != null)
                    {
                        if (RegKey.ToString().Contains("Photoshop"))
                        {
                            string Temp = RegKey.GetValue("ApplicationPath").ToString().Replace("/", @"\") + "AMT\\application.xml";
                            if (File.Exists(Temp))
                            {
                                WriteGreen("> Found Key File >> " + Temp);
                                PhotoshopDirectorys.Add(Temp);
                            }
                        }
                        else if (RegKey.ToString().Contains("After Effects"))
                        {
                            string Temp = RegKey.GetValue("InstallPath").ToString().Replace("/", @"\") + "AMT\\application.xml";
                            if (File.Exists(Temp))
                            {
                                WriteGreen("> Found Key File >> " + Temp);
                                AfterEffectsDirectorys.Add(Temp);
                            }
                        }
                    }
                    else
                    {
                        WriteRed("> Could not find Reg Path: " + Path);
                    }
                }
            }

            WriteGray("---------------------------------------------------------------------------------------------------------------->");

            var Randomizer = new Random();
            var RandomKey = Randomizer.Next(100000000, 999999999);

            foreach (string Directory in PhotoshopDirectorys)
            {
                try
                {
                    var PhotoshopDoc = XDocument.Load(Directory);
                    var WritePhotoshopKey = PhotoshopDoc.Descendants("Data").First(d => (string)d.Attribute("key") == "TrialSerialNumber");
                    WritePhotoshopKey.Value = "911987082836993" + RandomKey;
                    PhotoshopDoc.Save(Directory);
                    WriteGreen("> Refreshed >> " + Directory);
                }
                catch (Exception Error)
                {
                    WriteRed("> Upps...\r\n" + Error);
                }
            }

            foreach (string Directory in AfterEffectsDirectorys)
            {
                try
                {
                    var AfterEffectsDoc = XDocument.Load(Directory);
                    var WriteAfterEffectsKey = AfterEffectsDoc.Descendants("Data").First(d => (string)d.Attribute("key") == "TrialSerialNumber");
                    WriteAfterEffectsKey.Value = "102310015133850" + RandomKey;
                    AfterEffectsDoc.Save(Directory);
                    WriteGreen("> Refreshed >> " + Directory);
                }
                catch (Exception Error)
                {
                    WriteRed("> Upps...\r\n" + Error);
                }
            }

            WriteGray("---------------------------------------------------------------------------------------------------------------->");
            Thread.Sleep(1500);
        }

        private static void WriteRed(string Text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Text);
        }

        private static void WriteGreen(string Text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Text);
        }

        private static void WriteGray(string Text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Text);
        }

        private static void WriteWhite(string Text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Text);
        }
    }
}
