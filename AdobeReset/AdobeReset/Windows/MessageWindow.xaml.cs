using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdobeReset.Helpers;

namespace AdobeReset.Windows
{
    // TODO: rework this shit later :shrug:
    public partial class MessageWindow : Window
    {
        public MessageWindow()
            : this("Message Window", "Hello it's a me a message window :>") { }

        public MessageWindow(string header, string text, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            InitializeComponent();

            NavbarLabel.Content = header;
            MessageTextBlock.Text = text;

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    OkButton.IsEnabled = true;
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    OkButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    YesButton.IsEnabled = true;
                    NoButton.IsEnabled = true;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    YesButton.IsEnabled = true;
                    NoButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
            }

            SystemSounds.Asterisk.Play();
        }

        public MessageBoxResult Result { get; private set; }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "OkButton":
                    Result = MessageBoxResult.OK;
                    break;
                case "CancelButton":
                    Result = MessageBoxResult.Cancel;
                    break;
                case "YesButton":
                    Result = MessageBoxResult.Yes;
                    break;
                case "NoButton":
                    Result = MessageBoxResult.No;
                    break;
            }

            Close();
        }

        public static MessageBoxResult Show(string header, string text, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            var messageWindow = new MessageWindow(header, text, buttons);
            messageWindow.ShowDialog();
            return messageWindow.Result;
        }
    }
}
