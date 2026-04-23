using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.DAL
{
    public class UserRepository
    {
        private readonly DatabaseContext _db;

        public UserRepository(DatabaseContext db) { _db = db; }

        public User GetByUsername(string username)
        {
            const string query = "SELECT TOP 1 * FROM Users WHERE Username=@Username AND IsActive=1";
            var table = _db.ExecuteDataTable(query, new SqlParameter("@Username", username));
            return table.Rows.Count == 0 ? null : MapUser(table.Rows[0]);
        }

        public User GetByCardUid(string cardUid)
        {
            const string query = "SELECT TOP 1 * FROM Users WHERE CardUid=@CardUid AND IsActive=1";
            var table = _db.ExecuteDataTable(query, new SqlParameter("@CardUid", cardUid));
            return table.Rows.Count == 0 ? null : MapUser(table.Rows[0]);
        }

        public List<User> GetWaiterUsers()
        {
            const string query = "SELECT * FROM Users WHERE RoleId=2 ORDER BY Username";
            var table = _db.ExecuteDataTable(query);
            var list = new List<User>();
            foreach (DataRow row in table.Rows) list.Add(MapUser(row));
            return list;
        }

        private static User MapUser(DataRow row)
        {
            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Username = Convert.ToString(row["Username"]),
                PasswordHash = Convert.ToString(row["PasswordHash"]),
                RoleId = Convert.ToInt32(row["RoleId"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CardUid = row["CardUid"] == DBNull.Value ? null : Convert.ToString(row["CardUid"])
            };
        }
    }

    public class TableRepository
    {
        private readonly DatabaseContext _db;
        public TableRepository(DatabaseContext db) { _db = db; }

        public List<RestaurantTable> GetAll()
        {
            var data = _db.ExecuteDataTable("SELECT * FROM RestaurantTables ORDER BY TableName");
            var list = new List<RestaurantTable>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new RestaurantTable
                {
                    TableId = Convert.ToInt32(row["TableId"]),
                    TableName = Convert.ToString(row["TableName"]),
                    Status = (TableStatus)Convert.ToInt32(row["Status"])
                });
            }
            return list;
        }

        public void Save(RestaurantTable table)
        {
            if (table.TableId == 0)
            {
                _db.ExecuteNonQuery("INSERT INTO RestaurantTables(TableName, Status) VALUES(@Name, @Status)",
                    new SqlParameter("@Name", table.TableName),
                    new SqlParameter("@Status", (int)table.Status));
            }
            else
            {
                _db.ExecuteNonQuery("UPDATE RestaurantTables SET TableName=@Name, Status=@Status WHERE TableId=@Id",
                    new SqlParameter("@Name", table.TableName),
                    new SqlParameter("@Status", (int)table.Status),
                    new SqlParameter("@Id", table.TableId));
            }
        }

        public void Delete(int tableId)
        {
            _db.ExecuteNonQuery("DELETE FROM RestaurantTables WHERE TableId=@Id", new SqlParameter("@Id", tableId));
        }
    }

    public class MenuRepository
    {
        private readonly DatabaseContext _db;
        public MenuRepository(DatabaseContext db) { _db = db; }

        public DataTable GetMenuItemsWithCategory()
        {
            const string query = @"SELECT mi.MenuItemId, mc.Name AS CategoryName, mi.Name, mi.Price, mi.IsActive, mi.CategoryId
FROM MenuItems mi INNER JOIN MenuCategories mc ON mc.CategoryId = mi.CategoryId ORDER BY mc.Name, mi.Name";
            return _db.ExecuteDataTable(query);
        }

        public DataTable GetCategories()
        {
            return _db.ExecuteDataTable("SELECT * FROM MenuCategories ORDER BY Name");
        }

        public void SaveMenuItem(MenuItem item)
        {
            if (item.MenuItemId == 0)
            {
                _db.ExecuteNonQuery("INSERT INTO MenuItems(CategoryId, Name, Price, IsActive) VALUES(@CategoryId,@Name,@Price,@IsActive)",
                    new SqlParameter("@CategoryId", item.CategoryId), new SqlParameter("@Name", item.Name),
                    new SqlParameter("@Price", item.Price), new SqlParameter("@IsActive", item.IsActive));
                return;
            }

            _db.ExecuteNonQuery("UPDATE MenuItems SET CategoryId=@CategoryId, Name=@Name, Price=@Price, IsActive=@IsActive WHERE MenuItemId=@Id",
                new SqlParameter("@CategoryId", item.CategoryId), new SqlParameter("@Name", item.Name),
                new SqlParameter("@Price", item.Price), new SqlParameter("@IsActive", item.IsActive), new SqlParameter("@Id", item.MenuItemId));
        }

        public void DeleteMenuItem(int menuItemId)
        {
            _db.ExecuteNonQuery("DELETE FROM MenuItems WHERE MenuItemId=@Id", new SqlParameter("@Id", menuItemId));
        }
    }

    public class OrderRepository
    {
        private readonly DatabaseContext _db;
        public OrderRepository(DatabaseContext db) { _db = db; }

        public int CreateOrder(int tableId, int waiterId)
        {
            const string check = "SELECT COUNT(1) FROM Orders WHERE TableId=@TableId AND Status IN (1,2)";
            var activeCount = Convert.ToInt32(_db.ExecuteScalar(check, new SqlParameter("@TableId", tableId)));
            if (activeCount > 0)
            {
                throw new InvalidOperationException("Kjo tavolinë ka porosi aktive.");
            }

            const string insert = @"INSERT INTO Orders(TableId, WaiterId, CreatedAt, Status)
VALUES(@TableId,@WaiterId,GETDATE(),1); SELECT SCOPE_IDENTITY();";
            var id = _db.ExecuteScalar(insert, new SqlParameter("@TableId", tableId), new SqlParameter("@WaiterId", waiterId));
            _db.ExecuteNonQuery("UPDATE RestaurantTables SET Status=2 WHERE TableId=@TableId", new SqlParameter("@TableId", tableId));
            return Convert.ToInt32(id);
        }

        public DataTable GetOpenOrders()
        {
            const string query = @"SELECT o.OrderId, t.TableName, w.FullName AS WaiterName, o.CreatedAt, o.Status
FROM Orders o
INNER JOIN RestaurantTables t ON t.TableId = o.TableId
INNER JOIN Waiters w ON w.WaiterId = o.WaiterId
WHERE o.Status IN (1,2)
ORDER BY o.CreatedAt DESC";
            return _db.ExecuteDataTable(query);
        }

        public DataTable GetOrderItems(int orderId)
        {
            return _db.ExecuteDataTable(@"SELECT oi.OrderItemId, oi.MenuItemId, mi.Name, oi.Quantity, oi.UnitPrice, (oi.Quantity * oi.UnitPrice) AS LineTotal
FROM OrderItems oi INNER JOIN MenuItems mi ON mi.MenuItemId = oi.MenuItemId WHERE oi.OrderId=@OrderId", new SqlParameter("@OrderId", orderId));
        }

        public void AddOrUpdateOrderItem(int orderId, int menuItemId, int quantity)
        {
            var priceObj = _db.ExecuteScalar("SELECT Price FROM MenuItems WHERE MenuItemId=@Id AND IsActive=1", new SqlParameter("@Id", menuItemId));
            if (priceObj == null)
            {
                throw new InvalidOperationException("Produkti nuk u gjet ose nuk është aktiv.");
            }

            var unitPrice = Convert.ToDecimal(priceObj);
            var existing = _db.ExecuteScalar("SELECT OrderItemId FROM OrderItems WHERE OrderId=@OrderId AND MenuItemId=@MenuItemId",
                new SqlParameter("@OrderId", orderId), new SqlParameter("@MenuItemId", menuItemId));

            if (existing == null)
            {
                _db.ExecuteNonQuery("INSERT INTO OrderItems(OrderId, MenuItemId, Quantity, UnitPrice) VALUES(@OrderId,@MenuItemId,@Quantity,@UnitPrice)",
                    new SqlParameter("@OrderId", orderId), new SqlParameter("@MenuItemId", menuItemId),
                    new SqlParameter("@Quantity", quantity), new SqlParameter("@UnitPrice", unitPrice));
            }
            else
            {
                _db.ExecuteNonQuery("UPDATE OrderItems SET Quantity=@Quantity WHERE OrderItemId=@OrderItemId",
                    new SqlParameter("@Quantity", quantity), new SqlParameter("@OrderItemId", Convert.ToInt32(existing)));
            }
        }

        public void RemoveOrderItem(int orderItemId)
        {
            _db.ExecuteNonQuery("DELETE FROM OrderItems WHERE OrderItemId=@Id", new SqlParameter("@Id", orderItemId));
        }

        public void TransferOrderTable(int orderId, int targetTableId)
        {
            var activeCount = Convert.ToInt32(_db.ExecuteScalar("SELECT COUNT(1) FROM Orders WHERE TableId=@TableId AND Status IN (1,2)", new SqlParameter("@TableId", targetTableId)));
            if (activeCount > 0) throw new InvalidOperationException("Tavolina destinacion ka porosi aktive.");

            var currentTableId = Convert.ToInt32(_db.ExecuteScalar("SELECT TableId FROM Orders WHERE OrderId=@OrderId", new SqlParameter("@OrderId", orderId)));
            _db.ExecuteNonQuery("UPDATE Orders SET TableId=@TableId WHERE OrderId=@OrderId", new SqlParameter("@TableId", targetTableId), new SqlParameter("@OrderId", orderId));
            _db.ExecuteNonQuery("UPDATE RestaurantTables SET Status=1 WHERE TableId=@TableId", new SqlParameter("@TableId", currentTableId));
            _db.ExecuteNonQuery("UPDATE RestaurantTables SET Status=2 WHERE TableId=@TableId", new SqlParameter("@TableId", targetTableId));
        }

        public decimal CalculateTotal(int orderId)
        {
            var totalObj = _db.ExecuteScalar("SELECT ISNULL(SUM(Quantity * UnitPrice), 0) FROM OrderItems WHERE OrderId=@OrderId", new SqlParameter("@OrderId", orderId));
            return Convert.ToDecimal(totalObj);
        }

        public void CompletePayment(int orderId, decimal cashReceived)
        {
            var total = CalculateTotal(orderId);
            if (cashReceived < total) throw new InvalidOperationException("Shuma e dhënë është më e vogël se totali.");

            var change = cashReceived - total;
            _db.ExecuteNonQuery("INSERT INTO Payments(OrderId, AmountPaid, CashReceived, ChangeAmount, PaidAt) VALUES(@OrderId,@Amount,@Cash,@Change,GETDATE())",
                new SqlParameter("@OrderId", orderId), new SqlParameter("@Amount", total),
                new SqlParameter("@Cash", cashReceived), new SqlParameter("@Change", change));
            _db.ExecuteNonQuery("UPDATE Orders SET Status=3, ClosedAt=GETDATE() WHERE OrderId=@OrderId", new SqlParameter("@OrderId", orderId));
            var tableId = Convert.ToInt32(_db.ExecuteScalar("SELECT TableId FROM Orders WHERE OrderId=@OrderId", new SqlParameter("@OrderId", orderId)));
            _db.ExecuteNonQuery("UPDATE RestaurantTables SET Status=1 WHERE TableId=@Id", new SqlParameter("@Id", tableId));
        }
    }

    public class StaffRepository
    {
        private readonly DatabaseContext _db;
        public StaffRepository(DatabaseContext db) { _db = db; }

        public DataTable GetWaiters()
        {
            return _db.ExecuteDataTable(@"SELECT w.WaiterId, u.UserId, u.Username, w.FullName, w.Phone, w.Notes, u.CardUid, u.IsActive
FROM Waiters w INNER JOIN Users u ON u.UserId = w.UserId ORDER BY w.FullName");
        }

        public DataTable GetSchedules()
        {
            return _db.ExecuteDataTable(@"SELECT s.ScheduleId, w.FullName, s.WorkDate, s.StartTime, s.EndTime, s.ShiftName
FROM StaffSchedules s INNER JOIN Waiters w ON w.WaiterId = s.WaiterId ORDER BY s.WorkDate DESC");
        }
    }

    public class ReportsRepository
    {
        private readonly DatabaseContext _db;
        public ReportsRepository(DatabaseContext db) { _db = db; }

        public DataTable GetSalesByDateRange(DateTime startDate, DateTime endDate)
        {
            const string query = @"SELECT CAST(p.PaidAt AS DATE) AS DateKey,
SUM(p.AmountPaid) AS TotalRevenue,
COUNT(DISTINCT p.OrderId) AS OrderCount,
STRING_AGG(mi.Name + ' x' + CAST(oi.Quantity AS VARCHAR(10)), ', ') AS SoldItemsSummary
FROM Payments p
INNER JOIN Orders o ON o.OrderId = p.OrderId
INNER JOIN OrderItems oi ON oi.OrderId = o.OrderId
INNER JOIN MenuItems mi ON mi.MenuItemId = oi.MenuItemId
WHERE p.PaidAt >= @StartDate AND p.PaidAt < DATEADD(DAY, 1, @EndDate)
GROUP BY CAST(p.PaidAt AS DATE)
ORDER BY DateKey DESC";
            return _db.ExecuteDataTable(query, new SqlParameter("@StartDate", startDate), new SqlParameter("@EndDate", endDate));
        }

        public DataTable GetQuickStats()
        {
            const string query = @"SELECT
(SELECT COUNT(1) FROM Orders WHERE Status IN (1,2)) AS ActiveOrders,
(SELECT COUNT(1) FROM RestaurantTables WHERE Status = 1) AS AvailableTables,
(SELECT ISNULL(SUM(AmountPaid),0) FROM Payments WHERE CAST(PaidAt AS DATE)=CAST(GETDATE() AS DATE)) AS TodayRevenue,
(SELECT COUNT(1) FROM Users WHERE RoleId=2 AND IsActive=1) AS ActiveWaiters";
            return _db.ExecuteDataTable(query);
        }
    }
}
