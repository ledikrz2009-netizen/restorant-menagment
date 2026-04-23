using System;
using System.Data;
using System.Windows.Forms;
using RestaurantManagementSystem.BLL;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.UI.Forms
{
    public class OrderManagementForm : Form
    {
        private readonly ServiceFactory _services;
        private readonly SessionContext _session;
        private readonly IReceiptPrinterService _receiptPrinter = new ReceiptPrinterService();
        private readonly DataGridView _ordersGrid = new DataGridView { Dock = DockStyle.Left, Width = 400, AutoGenerateColumns = true };
        private readonly DataGridView _itemsGrid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };

        public OrderManagementForm(ServiceFactory services, SessionContext session)
        {
            _services = services;
            _session = session;
            Text = "Menaxhimi i Porosive";
            Width = 1200;
            Height = 600;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45 };
            var btnRefresh = new Button { Text = "Rifresko" };
            var btnOpen = new Button { Text = "Hap porosi" };
            var btnAddItem = new Button { Text = "Shto artikull" };
            var btnRemoveItem = new Button { Text = "Hiq artikull" };
            var btnTransfer = new Button { Text = "Transfero tavolinën" };
            var btnPay = new Button { Text = "Paguaj + Printo" };

            btnRefresh.Click += (s, e) => LoadOrders();
            btnOpen.Click += (s, e) => OpenOrder();
            btnAddItem.Click += (s, e) => AddItem();
            btnRemoveItem.Click += (s, e) => RemoveItem();
            btnTransfer.Click += (s, e) => TransferOrder();
            btnPay.Click += (s, e) => CompletePayment();
            _ordersGrid.SelectionChanged += (s, e) => LoadItems();

            top.Controls.Add(btnRefresh);
            top.Controls.Add(btnOpen);
            top.Controls.Add(btnAddItem);
            top.Controls.Add(btnRemoveItem);
            top.Controls.Add(btnTransfer);
            top.Controls.Add(btnPay);

            Controls.Add(_itemsGrid);
            Controls.Add(_ordersGrid);
            Controls.Add(top);
            LoadOrders();
        }

        private int SelectedOrderId => _ordersGrid.CurrentRow == null ? 0 : Convert.ToInt32(_ordersGrid.CurrentRow.Cells["OrderId"].Value);

        private void LoadOrders() => _ordersGrid.DataSource = _services.OrderService.GetOpenOrders();

        private void LoadItems()
        {
            if (SelectedOrderId <= 0) return;
            _itemsGrid.DataSource = _services.OrderService.GetOrderItems(SelectedOrderId);
        }

        private void OpenOrder()
        {
            var tableIdText = Microsoft.VisualBasic.Interaction.InputBox("TableId", "Hap Porosi", "1");
            int tableId;
            if (!int.TryParse(tableIdText, out tableId)) return;

            var waiterIdText = Microsoft.VisualBasic.Interaction.InputBox("WaiterId", "Hap Porosi", "1");
            int waiterId;
            if (!int.TryParse(waiterIdText, out waiterId)) return;

            _services.OrderService.CreateOrder(tableId, waiterId);
            LoadOrders();
        }

        private void AddItem()
        {
            if (SelectedOrderId <= 0) return;
            var itemIdText = Microsoft.VisualBasic.Interaction.InputBox("MenuItemId", "Shto Artikull", "1");
            var qtyText = Microsoft.VisualBasic.Interaction.InputBox("Sasia", "Shto Artikull", "1");
            int itemId;
            int qty;
            if (!int.TryParse(itemIdText, out itemId) || !int.TryParse(qtyText, out qty)) return;
            _services.OrderService.AddOrUpdateOrderItem(SelectedOrderId, itemId, qty);
            LoadItems();
        }

        private void RemoveItem()
        {
            if (_itemsGrid.CurrentRow == null) return;
            _services.OrderService.RemoveOrderItem(Convert.ToInt32(_itemsGrid.CurrentRow.Cells["OrderItemId"].Value));
            LoadItems();
        }

        private void TransferOrder()
        {
            if (SelectedOrderId <= 0) return;
            var targetTableText = Microsoft.VisualBasic.Interaction.InputBox("Target TableId", "Transfero", "2");
            int tableId;
            if (!int.TryParse(targetTableText, out tableId)) return;
            _services.OrderService.TransferOrder(SelectedOrderId, tableId);
            LoadOrders();
        }

        private void CompletePayment()
        {
            if (SelectedOrderId <= 0) return;
            var total = _services.OrderService.CalculateTotal(SelectedOrderId);
            var cashText = Microsoft.VisualBasic.Interaction.InputBox("Shuma e dhënë", "Pagesa", total.ToString("0.00"));
            decimal cash;
            if (!decimal.TryParse(cashText, out cash)) return;

            _services.OrderService.CompletePayment(SelectedOrderId, cash);
            var orderItems = _services.OrderService.GetOrderItems(SelectedOrderId);
            var receipt = _receiptPrinter.BuildReceiptText("Restaurant RMS", "TBD", _session.CurrentUser.Username, orderItems, total, cash, cash - total);
            new ReceiptPreviewForm(_receiptPrinter, receipt).ShowDialog();
            LoadOrders();
            _itemsGrid.DataSource = new DataTable();
        }
    }
}
