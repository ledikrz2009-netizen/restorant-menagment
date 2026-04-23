using System;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;
using RestaurantManagementSystem.UI.Forms;

namespace RestaurantManagementSystem.UI
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceFactory();
            var session = new SessionContext();
            Application.Run(new LoginForm(services, session));
        }
    }
}
