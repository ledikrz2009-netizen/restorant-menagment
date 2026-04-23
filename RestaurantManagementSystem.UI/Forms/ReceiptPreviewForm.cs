using System;
using System.Windows.Forms;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.UI.Forms
{
    public class ReceiptPreviewForm : Form
    {
        public ReceiptPreviewForm(IReceiptPrinterService printerService, string content)
        {
            Text = "Parashikim Fature";
            Width = 500;
            Height = 650;

            var text = new TextBox { Multiline = true, Dock = DockStyle.Fill, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Text = content, Font = new System.Drawing.Font("Consolas", 10f) };
            var button = new Button { Text = "Printo", Dock = DockStyle.Bottom, Height = 40 };
            button.Click += (s, e) =>
            {
                try
                {
                    printerService.PrintReceipt(content);
                    MessageBox.Show("Fatura u dërgua për printim.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gabim gjatë printimit: " + ex.Message);
                }
            };

            Controls.Add(text);
            Controls.Add(button);
        }
    }
}
