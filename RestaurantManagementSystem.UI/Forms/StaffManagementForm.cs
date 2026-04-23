using System.Windows.Forms;
using RestaurantManagementSystem.BLL;

namespace RestaurantManagementSystem.UI.Forms
{
    public class StaffManagementForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly TabControl _tabControl = new TabControl { Dock = DockStyle.Fill };

        public StaffManagementForm(ServiceFactory services)
        {
            _services = services;
            Text = "Menaxhimi i Stafit";
            Width = 900;
            Height = 550;

            var waitersGrid = new DataGridView { Dock = DockStyle.Fill, DataSource = _services.StaffService.GetWaiters(), AutoGenerateColumns = true };
            var schedulesGrid = new DataGridView { Dock = DockStyle.Fill, DataSource = _services.StaffService.GetSchedules(), AutoGenerateColumns = true };

            _tabControl.TabPages.Add(new TabPage("Kamarierët") { Controls = { waitersGrid } });
            _tabControl.TabPages.Add(new TabPage("Orari") { Controls = { schedulesGrid } });
            Controls.Add(_tabControl);
        }
    }
}
