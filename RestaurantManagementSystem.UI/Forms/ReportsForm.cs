using System;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;

namespace RestaurantManagementSystem.UI.Forms
{
    public class ReportsForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly DataGridView _grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
        private readonly DateTimePicker _from = new DateTimePicker { Width = 140 };
        private readonly DateTimePicker _to = new DateTimePicker { Width = 140 };

        public ReportsForm(ServiceFactory services)
        {
            _services = services;
            Text = "Raportet e Shitjeve";
            Width = 1000;
            Height = 540;

            _from.Value = DateTime.Today;
            _to.Value = DateTime.Today;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45 };
            var btnDaily = new Button { Text = "Ditor" };
            var btnMonthly = new Button { Text = "Mujor" };

            btnDaily.Click += (s, e) => _grid.DataSource = _services.ReportService.GetSales(_from.Value.Date, _to.Value.Date);
            btnMonthly.Click += (s, e) =>
            {
                var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);
                _grid.DataSource = _services.ReportService.GetSales(start, end);
            };

            top.Controls.Add(new Label { Text = "Nga:" });
            top.Controls.Add(_from);
            top.Controls.Add(new Label { Text = "Deri:" });
            top.Controls.Add(_to);
            top.Controls.Add(btnDaily);
            top.Controls.Add(btnMonthly);

            Controls.Add(_grid);
            Controls.Add(top);
        }
    }
}
