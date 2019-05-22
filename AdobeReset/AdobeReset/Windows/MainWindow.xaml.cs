using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using AdobeReset.Helpers;
using AdobeReset.Models;
using AdobeReset.Structs;

namespace AdobeReset.Windows
{
    public partial class MainWindow : Window
    {
        private readonly AdobeProductModel _adobeProductModel;

        public MainWindow()
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) {
                MessageWindow.Show("Error", "This program needs to be run as admin!");
                this.Close();
            }

            InitializeComponent();

            AdobePathTextBox.Text = Path.Combine(Environment.Is64BitOperatingSystem
                ? Environment.GetEnvironmentVariable("ProgramW6432")
                : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Adobe");
            DataContext = _adobeProductModel = new AdobeProductModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Blur.ApplyBlur(this);
        }

        private void Navbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) {
                return;
            }

            this.DragMove();
        }

        private void NavbarExitLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) {
                return;
            }

            this.Close();
        }

        private void SearchProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AdobePathTextBox.Text) || !Directory.Exists(AdobePathTextBox.Text)) {
                MessageWindow.Show("Error", "Invalid adobe folder path");
                return;
            }

            var potentialProducts = new DirectoryInfo(AdobePathTextBox.Text)
                .GetDirectories()
                .Where(x => x.Name.StartsWith("Adobe"))
                .ToList();
            if (!potentialProducts.Any()) {
                MessageWindow.Show("Warning", "Could not find any adobe products");
                return;
            }

            var productList = new ConcurrentBag<AdobeProduct>();
            Parallel.ForEach(potentialProducts, (x) => {
                var appFiles = Directory.GetFiles(x.FullName, "application.xml",
                    SearchOption.AllDirectories);
                if (!appFiles.Any()) {
                    return;
                }

                productList.Add(new AdobeProduct {
                    FolderName = x.Name,
                    ApplicationFiles = appFiles
                });
            });

            _adobeProductModel.SetCollection(productList);
        }

        private void RefreshTrialsButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdobeProductsListBox.SelectedItems.Count == 0) {
                MessageWindow.Show("Error", "No product selected");
                return;
            }

            var failedList = new ConcurrentBag<string>();
            Parallel.ForEach(AdobeProductsListBox.SelectedItems.Cast<AdobeProduct>(), (product) => {
                foreach (var appFile in product.ApplicationFiles) {
                    var appDoc = XDocument.Load(appFile);
                    var saveFile = true;
                    foreach (var trialKey in appDoc.Descendants("Data")
                        .Where(x => (string)x.Attribute("key") == "TrialSerialNumber")
                        .ToList()) {
                        var key = trialKey.Value;
                        if (key.Length < 15) {
                            saveFile = false;
                            continue;
                        }

                        var randoKeyLen = key.Length - 15;
                        var randoKey = string.Empty;
                        var oldKey = key.Substring(15);
                        var keyPart = key.Substring(0, 15);
                        while ((randoKey = keyPart + Utils.RandoGen(randoKeyLen)) == oldKey) {
                            //
                        }

                        trialKey.SetValue(randoKey);
                    }

                    if (saveFile) {
                        appDoc.Save(appFile);
                    }
                    else {
                        failedList.Add(product.FolderName);
                    }
                }
            });

            MessageWindow.Show("Info", failedList.Any()
                ? $"Failed to reset:\n{string.Join("\n", failedList)}"
                : "Finished");
        }
    }
}
