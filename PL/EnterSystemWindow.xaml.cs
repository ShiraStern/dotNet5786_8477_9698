using System;
using System.Windows;

namespace PL
{
    public partial class EnterSystemWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // -------- Dependency Properties --------

        public int CourierId
        {
            get => (int)GetValue(CourierIdProperty);
            set => SetValue(CourierIdProperty, value);
        }
        private bool _isPasswordVisible = false;/////////////
        public static readonly DependencyProperty CourierIdProperty =
            DependencyProperty.Register(
                "CourierId",
                typeof(int),
                typeof(EnterSystemWindow));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(EnterSystemWindow));

        // -------- ctor --------

        public EnterSystemWindow()
        {
            InitializeComponent();
        }

        // -------- PasswordBox --------

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.PasswordBox pb)
                Password = pb.Password;
        }

        // -------- Login --------

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (CourierId <= 0 || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show(
                    "Please enter ID and password",
                    "Missing data",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                // קריאה אחת בלבד ל-BL
                string role = s_bl.Courier.Login(CourierId.ToString(), Password);

                switch (role)
                {
                    case "Courier":
                        new PL.Courier.CourierWindow(CourierId).Show();
                        break;

                    case "Manager":
                        var result = MessageBox.Show(
                            "Open Manager screen?",
                            "Choose screen",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                            new MainWindow().Show();
                        else
                            new PL.Courier.CourierWindow(CourierId).Show();
                        break;

                    default:
                        MessageBox.Show(
                            "Unknown role",
                            "Login error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                }
                UserContext.UserId = CourierId;

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Login failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
