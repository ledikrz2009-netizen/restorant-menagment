using System;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.UI.Forms
{
    public class TableManagementForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly DataGridView _grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };

        public TableManagementForm(ServiceFactory services)
        {
            _services = services;
            Text = "Menaxhimi i Tavolinave";
            Width = 700;
            Height = 500;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            var btnAdd = new Button { Text = "Shto" };
            var btnDelete = new Button { Text = "Fshi" };
            btnAdd.Click += (s, e) => AddTable();
            btnDelete.Click += (s, e) => DeleteTable();
            top.Controls.Add(btnAdd);
            top.Controls.Add(btnDelete);

            Controls.Add(_grid);
            Controls.Add(top);
            LoadData();
        }

        private void LoadData() => _grid.DataSource = _services.TableService.GetTables();

        private void AddTable()
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Emri/Numri i tavolinës", "Shto tavolinë", "T1");
            if (string.IsNullOrWhiteSpace(name)) return;
            _services.TableService.SaveTable(new RestaurantTable { TableName = name, Status = TableStatus.Available });
            LoadData();
        }

        private void DeleteTable()
        {
            if (_grid.CurrentRow == null) return;
            _services.TableService.DeleteTable(Convert.ToInt32(_grid.CurrentRow.Cells["TableId"].Value));
            LoadData();
        }
    }
}
