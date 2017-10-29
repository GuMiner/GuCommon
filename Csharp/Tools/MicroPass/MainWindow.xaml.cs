using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Common.Cryptography;

namespace MicroPass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ITextEncoder textEncoder = new Aes256TextEncoder();
        private readonly string passwordDirectory;

        private ObservableCollection<string> accounts;

        public MainWindow()
        {
            InitializeComponent();

            passwordDirectory = ConfigurationManager.AppSettings["passwordDirectory"];
            if (string.IsNullOrWhiteSpace(passwordDirectory) || !Directory.Exists(passwordDirectory))
            {
                passwordDirectory = Path.Combine(Directory.GetCurrentDirectory(), "passwords");
            }

            accounts = new ObservableCollection<string>();
            foreach (string file in Directory.GetFiles(passwordDirectory)
                .Select(file => file.Substring(file.LastIndexOf('\\') + 1)))
            {
                accounts.Add(file);
            }

            accountsBox.ItemsSource = accounts;
        }

        private void accountsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountsBox.SelectedItem != null)
            {
                if (string.IsNullOrWhiteSpace(rootPasswordBox.Password))
                {
                    throw new ArgumentException("The root password must be filled in.");
                }

                string fileName = Path.Combine(this.passwordDirectory, (string)accountsBox.SelectedItem);

                passwordBox.Text = textEncoder.DecryptText(File.ReadAllText(fileName, Encoding.UTF8), rootPasswordBox.Password);
            }
        }

        private void encodeButton_Click(object sender, RoutedEventArgs e)
        {
            string account = newAccountBox.Text;
            string password = newAccountPasswordBox.Text;

            if (string.IsNullOrWhiteSpace(rootPasswordBox.Password) ||
                string.IsNullOrWhiteSpace(account) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("The root password, account, and password boxes must all be filled in.");
            }

            string fileName = account + ".txt";
            File.WriteAllText(Path.Combine(this.passwordDirectory, fileName), textEncoder.EncryptText(password, rootPasswordBox.Password), Encoding.UTF8);

            newAccountBox.Text = string.Empty;
            newAccountPasswordBox.Text = string.Empty;
            if (!accounts.Contains(account))
            {
                accounts.Add(fileName);
            }
        }
    }
}
