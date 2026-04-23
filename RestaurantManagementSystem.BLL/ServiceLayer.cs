using System;
using System.Data;
using RestaurantManagementSystem.DAL;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Services;

namespace RestaurantManagementSystem.BLL
{
    public class ServiceFactory
    {
        public AuthService AuthService { get; }
        public TableService TableService { get; }
        public MenuService MenuService { get; }
        public OrderService OrderService { get; }
        public StaffService StaffService { get; }
        public ReportService ReportService { get; }

        public ServiceFactory(string connectionString = null)
        {
            var db = new DatabaseContext(connectionString);
            var hasher = new BasicPasswordHasher();
            var cardReader = new MockCardLoginService();

            AuthService = new AuthService(new UserRepository(db), hasher, cardReader);
            TableService = new TableService(new TableRepository(db));
            MenuService = new MenuService(new MenuRepository(db));
            OrderService = new OrderService(new OrderRepository(db));
            StaffService = new StaffService(new StaffRepository(db));
            ReportService = new ReportService(new ReportsRepository(db));
        }
    }

    public class AuthService
    {
        private readonly UserRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly ICardLoginService _cardService;

        public AuthService(UserRepository repo, IPasswordHasher hasher, ICardLoginService cardService)
        {
            _repo = repo;
            _hasher = hasher;
            _cardService = cardService;
        }

        public LoginResult LoginWithPassword(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginResult { IsSuccess = false, ErrorMessage = "Username dhe password janë të detyrueshme." };
            }

            var user = _repo.GetByUsername(username.Trim());
            if (user == null || !_hasher.Verify(password, user.PasswordHash))
            {
                return new LoginResult { IsSuccess = false, ErrorMessage = "Kredenciale të pavlefshme." };
            }

            return new LoginResult { IsSuccess = true, User = user };
        }

        public LoginResult LoginWithCard()
        {
            var cardUid = _cardService.ReadCardUid();
            var user = _repo.GetByCardUid(cardUid);
            if (user == null) return new LoginResult { IsSuccess = false, ErrorMessage = "Karta nuk është e regjistruar." };
            return new LoginResult { IsSuccess = true, User = user };
        }
    }

    public class TableService
    {
        private readonly TableRepository _repo;
        public TableService(TableRepository repo) { _repo = repo; }

        public System.Collections.Generic.List<RestaurantTable> GetTables() => _repo.GetAll();

        public void SaveTable(RestaurantTable table)
        {
            if (string.IsNullOrWhiteSpace(table.TableName)) throw new ArgumentException("Emri i tavolinës është i detyrueshëm.");
            _repo.Save(table);
        }

        public void DeleteTable(int tableId) => _repo.Delete(tableId);
    }

    public class MenuService
    {
        private readonly MenuRepository _repo;
        public MenuService(MenuRepository repo) { _repo = repo; }

        public DataTable GetMenuItems() => _repo.GetMenuItemsWithCategory();
        public DataTable GetCategories() => _repo.GetCategories();
        public void SaveMenuItem(MenuItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) throw new ArgumentException("Emri i artikullit është i detyrueshëm.");
            if (item.Price <= 0) throw new ArgumentException("Çmimi duhet të jetë më i madh se zero.");
            _repo.SaveMenuItem(item);
        }

        public void DeleteMenuItem(int menuItemId) => _repo.DeleteMenuItem(menuItemId);
    }

    public class OrderService
    {
        private readonly OrderRepository _repo;
        public OrderService(OrderRepository repo) { _repo = repo; }

        public int CreateOrder(int tableId, int waiterId) => _repo.CreateOrder(tableId, waiterId);
        public DataTable GetOpenOrders() => _repo.GetOpenOrders();
        public DataTable GetOrderItems(int orderId) => _repo.GetOrderItems(orderId);
        public void AddOrUpdateOrderItem(int orderId, int menuItemId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Sasia duhet të jetë më e madhe se zero.");
            _repo.AddOrUpdateOrderItem(orderId, menuItemId, quantity);
        }

        public void RemoveOrderItem(int orderItemId) => _repo.RemoveOrderItem(orderItemId);
        public void TransferOrder(int orderId, int targetTableId) => _repo.TransferOrderTable(orderId, targetTableId);
        public decimal CalculateTotal(int orderId) => _repo.CalculateTotal(orderId);
        public void CompletePayment(int orderId, decimal cashReceived) => _repo.CompletePayment(orderId, cashReceived);
    }

    public class StaffService
    {
        private readonly StaffRepository _repo;
        public StaffService(StaffRepository repo) { _repo = repo; }

        public DataTable GetWaiters() => _repo.GetWaiters();
        public DataTable GetSchedules() => _repo.GetSchedules();
    }

    public class ReportService
    {
        private readonly ReportsRepository _repo;
        public ReportService(ReportsRepository repo) { _repo = repo; }

        public DataTable GetSales(DateTime startDate, DateTime endDate) => _repo.GetSalesByDateRange(startDate, endDate);
        public DataTable GetQuickStats() => _repo.GetQuickStats();
    }
}
