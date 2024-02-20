using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Baiguzin41size
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Text;
            if (login =="" || password =="")
            {
                MessageBox.Show("Ест пустые поля");
            }

            User user = Baiguzin_41Entities1.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null) 
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                Login.Text = "";
                Password.Text = "";
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                LoginBtn.IsEnabled = false;
                await Task.Delay(TimeSpan.FromSeconds(10));
                LoginBtn.IsEnabled = true;
            }
        }

        private void LoginGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            User user = Baiguzin_41Entities1.GetContext().User.ToList().Find(p => p.UserLogin == null && p.UserPassword == null);
            Manager.MainFrame.Navigate(new ProductPage(user));
            Login.Text = "";
            Password.Text = "";
        }
    }
}
