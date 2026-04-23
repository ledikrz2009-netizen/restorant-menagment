using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;

namespace RestaurantManagementSystem.UI.Forms
{
    public class DashboardForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly SessionContext _session;
        private readonly Label _lblStats = new Label { AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };

        public DashboardForm(ServiceFactory services, SessionContext session)
        {
            _services = services;
            _session = session;
            InitializeComponent();
            LoadStats();
        }

        private void InitializeComponent()
        {
            Text = "Dashboard";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            var menu = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8) };

            var btnOrders = new Button { Text = "Porositë", Width = 120 };
            btnOrders.Click += (s, e) => new OrderManagementForm(_services, _session).ShowDialog();
            menu.Controls.Add(btnOrders);

            if (_session.IsAdmin)
            {
                var btnTables = new Button { Text = "Tavolinat", Width = 120 };
                btnTables.Click += (s, e) => new TableManagementForm(_services).ShowDialog();
                menu.Controls.Add(btnTables);

                var btnMenu = new Button { Text = "Menu", Width = 120 };
                btnMenu.Click += (s, e) => new MenuManagementForm(_services).ShowDialog();
                menu.Controls.Add(btnMenu);

                var btnStaff = new Button { Text = "Stafi", Width = 120 };
                btnStaff.Click += (s, e) => new StaffManagementForm(_services).ShowDialog();
                menu.Controls.Add(btnStaff);

                var btnReports = new Button { Text = "Raportet", Width = 120 };
                btnReports.Click += (s, e) => new ReportsForm(_services).ShowDialog();
                menu.Controls.Add(btnReports);
            }

            var btnRefresh = new Button { Text = "Refresh", Width = 120 };
            btnRefresh.Click += (s, e) => LoadStats();
            menu.Controls.Add(btnRefresh);

            Controls.Add(menu);
            Controls.Add(new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), Controls = { _lblStats } });
        }

        private void LoadStats()
        {
            try
            {
                DataTable stats = _services.ReportService.GetQuickStats();
                if (stats.Rows.Count == 0) return;
                var row = stats.Rows[0];
                _lblStats.Text = string.Format("Porosi aktive: {0}\nTavolina të lira: {1}\nXhiro sot: {2:0.00}\nKamarier aktivë: {3}",
                    row["ActiveOrders"], row["AvailableTables"], row["TodayRevenue"], row["ActiveWaiters"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gabim në ngarkimin e statistikave: " + ex.Message);
            }
        }
    }
}
