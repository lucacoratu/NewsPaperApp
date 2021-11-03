using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace NewsPaperApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int login_button_click_count = 0;
        private long start;
        private long end;
        private long elapsedTime = 30;
        public MainWindow()
        {
            InitializeComponent();
        }

        public static string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }

            return Result.ToString();
        }

        private void txtbox_enterPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txtbox_enterPassword.Visibility = Visibility.Hidden;
            this.txtbox_password.Visibility = Visibility.Visible;
        }

        private void txtbox_username_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txtbox_username.Text = "";
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            //If the login failed 3 times then the user should wait 30 sec before retrying
            if (this.login_button_click_count == 3)
            {
                this.end = DateTimeOffset.Now.ToUnixTimeSeconds();
                if(this.end - this.start >= 30)
                {
                    this.login_button_click_count = 0;
                }
            }
            else
            {
                //Take the credentials from the username text box and password textbox and check if the are in the database
                string username = this.txtbox_username.Text;
                string password = this.txtbox_password.Password;
                SHA256 sha256 = SHA256.Create();
                Byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string HashedPassword = ByteArrayToHexString(hashedPassword);

                if (DatabaseConnection.CheckLoginCredentials(username, HashedPassword) == true)
                {
                    MessageBox.Show("Login successful", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    this.login_button_click_count = 0;
                }
                else
                {
                    MessageBox.Show("Login failed!", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    ++this.login_button_click_count;
                    this.start = DateTimeOffset.Now.ToUnixTimeSeconds();
                }
            }
        }

        private void lbl_signup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Go to the register page
            this.Hide();
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.Show();
        }
    }
}
