using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace RestaurantManagementSystem.Services
{
    public interface IReceiptPrinterService
    {
        string BuildReceiptText(string restaurantName, string tableName, string waiterName, DataTable orderItems, decimal total, decimal cashReceived, decimal change);
        void PrintReceipt(string content);
    }

    public class ReceiptPrinterService : IReceiptPrinterService
    {
        public string BuildReceiptText(string restaurantName, string tableName, string waiterName, DataTable orderItems, decimal total, decimal cashReceived, decimal change)
        {
            var sb = new StringBuilder();
            sb.AppendLine(restaurantName);
            sb.AppendLine("--------------------------------");
            sb.AppendLine("Tavolina: " + tableName);
            sb.AppendLine("Kamarieri: " + waiterName);
            sb.AppendLine("Data: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            sb.AppendLine("--------------------------------");
            sb.AppendLine("Artikulli      Sasia   Cmimi");
            sb.AppendLine("--------------------------------");

            foreach (DataRow row in orderItems.Rows)
            {
                sb.AppendLine(string.Format("{0,-13}{1,4} {2,8:0.00}", row["Name"], row["Quantity"], row["LineTotal"]));
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine(string.Format("Totali: {0:0.00}", total));
            sb.AppendLine(string.Format("Cash: {0:0.00}", cashReceived));
            sb.AppendLine(string.Format("Kusuri: {0:0.00}", change));
            sb.AppendLine("Faleminderit!");

            return sb.ToString();
        }

        public void PrintReceipt(string content)
        {
            var document = new PrintDocument();
            document.PrintPage += (sender, args) =>
            {
                args.Graphics.DrawString(content, new Font("Consolas", 9), Brushes.Black, 5, 5);
            };
            document.Print();
        }
    }
}
