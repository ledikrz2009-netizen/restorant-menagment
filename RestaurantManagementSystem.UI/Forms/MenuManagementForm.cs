using System;
using System.Data;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.UI.Forms
{
    public class MenuManagementForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly DataGridView _grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };

        public MenuManagementForm(ServiceFactory services)
        {
            _services = services;
            Text = "Menaxhimi i Menusë";
            Width = 850;
            Height = 520;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45 };
            var btnAdd = new Button { Text = "Shto artikull" };
            var btnDelete = new Button { Text = "Fshi" };
            btnAdd.Click += (s, e) => AddItem();
            btnDelete.Click += (s, e) => DeleteItem();
            top.Controls.Add(btnAdd);
            top.Controls.Add(btnDelete);

            Controls.Add(_grid);
            Controls.Add(top);
            LoadData();
        }

        private void LoadData() => _grid.DataSource = _services.MenuService.GetMenuItems();

        private void AddItem()
        {
            DataTable categories = _services.MenuService.GetCategories();
            if (categories.Rows.Count == 0) { MessageBox.Show("Nuk ka kategori."); return; }

            var name = Microsoft.VisualBasic.Interaction.InputBox("Emri i artikullit", "Shto", "Kafe");
            var priceText = Microsoft.VisualBasic.Interaction.InputBox("Çmimi", "Shto", "120");
            decimal price;
            if (!decimal.TryParse(priceText, out price)) { MessageBox.Show("Çmim i pavlefshëm."); return; }

            _services.MenuService.SaveMenuItem(new MenuItem
            {
                CategoryId = Convert.ToInt32(categories.Rows[0]["CategoryId"]),
                Name = name,
                Price = price,
                IsActive = true
            });
            LoadData();
        }

        private void DeleteItem()
        {
            if (_grid.CurrentRow == null) return;
            _services.MenuService.DeleteMenuItem(Convert.ToInt32(_grid.CurrentRow.Cells["MenuItemId"].Value));
            LoadData();
        }
    }
}
