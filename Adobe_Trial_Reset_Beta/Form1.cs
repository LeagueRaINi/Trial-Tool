using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using IniParser;
using MetroFramework;

namespace Adobe_Trial_Reset_Beta
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        //On Start
        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("Config.ini"))
            {
                var pConfig = new FileIniDataParser();
                var dConfig = pConfig.ReadFile("Config.ini");

                if (File.Exists(@"C:\Program Files\Adobe\Adobe After Effects CC 2017\Support Files\AMT\application.xml"))
                {
                    dConfig["Directory"]["After Effects"] =
                        @"C:\Program Files\Adobe\Adobe After Effects CC 2017\Support Files\AMT\application.xml";
                    pConfig.WriteFile("Config.ini", dConfig);
                }
                else if (File.Exists(@"C:\Program Files\Adobe\Adobe After Effects CC 2014\Support Files\AMT\application.xml"))
                {
                    dConfig["Directory"]["After Effects"] =
                        @"C:\Program Files\Adobe\Adobe After Effects CC 2014\Support Files\AMT\application.xml";
                    pConfig.WriteFile("Config.ini", dConfig);
                }

                if (File.Exists(@"C:\Program Files\Adobe\Adobe Photoshop CC 2017\AMT\application.xml"))
                {
                    dConfig["Directory"]["Photoshop"] =
                        @"C:\Program Files\Adobe\Adobe Photoshop CC 2017\AMT\application.xml";
                    pConfig.WriteFile("Config.ini", dConfig);
                }
                else if (File.Exists(@"C:\Program Files\Adobe\Adobe Photoshop CC 2015.5\AMT\application.xml"))
                {
                    dConfig["Directory"]["Photoshop"] =
                        @"C:\Program Files\Adobe\Adobe Photoshop CC 2015.5\AMT\application.xml";
                    pConfig.WriteFile("Config.ini", dConfig);
                }
            }
            else
            {
                try
                {
                    var text = "[Directory]\r\nAfter Effects = \r\nPhotoshop = ";
                    File.WriteAllText("Config.ini", text);

                    Application.Restart();
                }
                catch (Exception)
                {
                    DialogResult CreateIni = MetroMessageBox.Show(this, "Could not Create Config.ini", "Information",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Error);

                    if (CreateIni == DialogResult.Retry)
                    {
                        try
                        {
                            var text = "[Directory]\r\nAfter Effects = \r\nPhotoshop = ";
                            File.WriteAllText("Config.ini", text);

                            Application.Restart();
                        }
                        catch (Exception)
                        {
                            MetroMessageBox.Show(this, "Could not Create Config.ini\r\nProgramm will now Close!",
                                "Information", MessageBoxButtons.RetryCancel,
                                MessageBoxIcon.Error);
                            Close();
                        }
                    }
                }
            }
        }

        //After Effects
        private void AfterEffects()
        {
            var pDirectory = new FileIniDataParser();
            var dDirectory = pDirectory.ReadFile("Config.ini");
            string AfterEffects = dDirectory["Directory"]["Photoshop"];

            if (File.Exists(AfterEffects))
            {
                metroButton2.Enabled = true;
            }
            else
            {
                DialogResult SelectAfterEffects = MetroMessageBox.Show(this,
                    "Could not find After Effects Directory\r\nPress OK to Select the After Effects Folder",
                    "Information", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);

                if (SelectAfterEffects == DialogResult.OK)
                {
                    var SearchFolder = new FolderBrowserDialog();
                    var tempPath = "";

                    if (SearchFolder.ShowDialog() == DialogResult.OK)
                    {
                        tempPath = SearchFolder.SelectedPath;

                        if (File.Exists(tempPath + @"\Support Files\AMT\application.xml"))
                        {
                            var pConfig = new FileIniDataParser();
                            var dConfig = pConfig.ReadFile("Config.ini");
                            dConfig["Directory"]["After Effects"] = tempPath + @"\Support Files\AMT\application.xml";
                            pConfig.WriteFile("Config.ini", dConfig);

                            if (File.Exists(AfterEffects))
                            {
                                metroButton2.Enabled = true;
                            }

                        }
                        else
                        {
                            MetroMessageBox.Show(this,
                                "After Effects File not Found\r\nMake sure to select the correct Folder!", "Information",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        //Photoshop
        private void Photoshop()
        {
            var pDirectory = new FileIniDataParser();
            var dDirectory = pDirectory.ReadFile("Config.ini");
            string Photoshop = dDirectory["Directory"]["Photoshop"];

            if (File.Exists(Photoshop))
            {
                metroButton2.Enabled = true;
            }
            else
            {
                DialogResult SelectPhotoshop = MetroMessageBox.Show(this,
                    "Could not find Photoshop Directory\r\nPress OK to Select the Photoshop Folder", "Information",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);

                if (SelectPhotoshop == DialogResult.OK)
                {
                    var SearchFolder = new FolderBrowserDialog();
                    var tempPath = "";

                    if (SearchFolder.ShowDialog() == DialogResult.OK)
                    {
                        tempPath = SearchFolder.SelectedPath;

                        if (File.Exists(tempPath + @"\AMT\application.xml"))
                        {
                            var pConfig = new FileIniDataParser();
                            var dConfig = pConfig.ReadFile("Config.ini");
                            dConfig["Directory"]["Photoshop"] = tempPath + @"\AMT\application.xml";
                            pConfig.WriteFile("Config.ini", dConfig);

                            if (File.Exists(Photoshop))
                            {
                                metroButton2.Enabled = true;
                            }

                        }
                        else
                        {
                            MetroMessageBox.Show(this,
                                "Photoshop File not Found\r\nMake sure to select the correct Folder!", "Information",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        //Combobox
        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedItem == "After Effects")
            {
                AfterEffects();

                metroButton1.Enabled = true;
            }
            else if (metroComboBox1.SelectedItem == "Photoshop")
            {
                Photoshop();

                metroButton1.Enabled = true;
            }
        }

        //Reset Trial Button
        private void metroButton2_Click(object sender, EventArgs e)
        {
            var pDirectory2 = new FileIniDataParser();
            var dDirectory2 = pDirectory2.ReadFile("Config.ini");
            var Photoshop = dDirectory2["Directory"]["Photoshop"];
            var AfterEffects = dDirectory2["Directory"]["After Effects"];

            //Random Number Generator
            var random = new Random();
            var randomNumber = random.Next(100000000, 999999999);

            if (metroComboBox1.SelectedItem == "Photoshop" & File.Exists(Photoshop))
            {
                try
                {
                    var doc = XDocument.Load(Photoshop);
                    var write = doc.Descendants("Data").First(d => (string) d.Attribute("key") == "TrialSerialNumber");
                    write.Value = "911987082836993" + randomNumber;
                    doc.Save(Photoshop);

                    var load = XDocument.Load(Photoshop);
                    var loadnew = load.Descendants("Data")
                        .First(d => (string) d.Attribute("key") == "TrialSerialNumber");
                    metroTextBox1.Text = (string) loadnew;
                }
                catch (Exception)
                {
                    MetroMessageBox.Show(this, "Error while trying to reset the Trial Key\r\nPress OK to continue",
                        "Information", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else if (metroComboBox1.SelectedItem == "After Effects" & File.Exists(AfterEffects))
            {
                try
                {
                    var doc = XDocument.Load(AfterEffects);
                    var write = doc.Descendants("Data").First(d => (string)d.Attribute("key") == "TrialSerialNumber");
                    write.Value = "102310015133850" + randomNumber;
                    doc.Save(AfterEffects);

                    var load = XDocument.Load(AfterEffects);
                    var loadnew = load.Descendants("Data")
                        .First(d => (string)d.Attribute("key") == "TrialSerialNumber");
                    metroTextBox1.Text = (string)loadnew;
                }
                catch (Exception)
                {
                    MetroMessageBox.Show(this, "Error while trying to reset the Trial Key\r\nPress OK to continue",
                        "Information", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        //Change Directory Button
        private void metroButton1_Click(object sender, EventArgs e)
        {
            var pDirectory = new FileIniDataParser();
            var dDirectory = pDirectory.ReadFile("Config.ini");

            var SearchFolder = new FolderBrowserDialog();
            var tempPath = "";

            if (metroComboBox1.SelectedItem == "Photoshop")
            {
                if (SearchFolder.ShowDialog() == DialogResult.OK)
                {
                    tempPath = SearchFolder.SelectedPath;

                    if (File.Exists(tempPath + @"\AMT\application.xml"))
                    {
                        var pConfig = new FileIniDataParser();
                        var dConfig = pConfig.ReadFile("Config.ini");
                        dConfig["Directory"]["Photoshop"] = tempPath + @"\AMT\application.xml";
                        pConfig.WriteFile("Config.ini", dConfig);

                        string Photoshop = dDirectory["Directory"]["Photoshop"];

                        if (File.Exists(Photoshop))
                        {
                            metroButton2.Enabled = true;
                        }

                    }
                    else
                    {
                        MetroMessageBox.Show(this,
                            "Photoshop File not Found\r\nMake sure to select the correct Folder!", "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            else if (metroComboBox1.SelectedItem == "After Effects")
            {
                if (SearchFolder.ShowDialog() == DialogResult.OK)
                {
                    tempPath = SearchFolder.SelectedPath;

                    if (File.Exists(tempPath + @"\Support Files\AMT\application.xml"))
                    {
                        var pConfig = new FileIniDataParser();
                        var dConfig = pConfig.ReadFile("Config.ini");
                        dConfig["Directory"]["After Effects"] = tempPath + @"\Support Files\AMT\application.xml";
                        pConfig.WriteFile("Config.ini", dConfig);

                        string AfterEffects = dDirectory["Directory"]["After Effects"];

                        if (File.Exists(AfterEffects))
                        {
                            metroButton2.Enabled = true;
                        }

                    }
                    else
                    {
                        MetroMessageBox.Show(this,
                            "Photoshop File not Found\r\nMake sure to select the correct Folder!", "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
