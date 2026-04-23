using System;
using System.Drawing;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;

namespace RestaurantManagementSystem.UI.Forms
{
    public class LoginForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly SessionContext _session;
        private readonly TextBox _txtUsername = new TextBox { Width = 220 };
        private readonly TextBox _txtPassword = new TextBox { Width = 220, UseSystemPasswordChar = true };
        private readonly Label _lblError = new Label { AutoSize = true, ForeColor = Color.DarkRed };

        public LoginForm(ServiceFactory services, SessionContext session)
        {
            _services = services;
            _session = session;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Restaurant Management - Login";
            Width = 420;
            Height = 290;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(25) };
            panel.Controls.Add(new Label { Text = "Username" });
            panel.Controls.Add(_txtUsername);
            panel.Controls.Add(new Label { Text = "Password" });
            panel.Controls.Add(_txtPassword);

            var btnLogin = new Button { Text = "Login", Width = 220, Height = 32 };
            btnLogin.Click += (s, e) => LoginPassword();

            var btnCard = new Button { Text = "Login me NFC/RFID (Mock)", Width = 220, Height = 32 };
            btnCard.Click += (s, e) => LoginCard();

            panel.Controls.Add(btnLogin);
            panel.Controls.Add(btnCard);
            panel.Controls.Add(_lblError);
            Controls.Add(panel);
        }

        private void LoginPassword()
        {
            var result = _services.AuthService.LoginWithPassword(_txtUsername.Text, _txtPassword.Text);
            HandleLoginResult(result);
        }

        private void LoginCard()
        {
            var result = _services.AuthService.LoginWithCard();
            HandleLoginResult(result);
        }

        private void HandleLoginResult(RestaurantManagementSystem.Models.LoginResult result)
        {
            if (!result.IsSuccess)
            {
                _lblError.Text = result.ErrorMessage;
                return;
            }

            _session.CurrentUser = result.User;
            Hide();
            using (var dashboard = new DashboardForm(_services, _session))
            {
                dashboard.ShowDialog();
            }
            Close();
        }
    }
}
