using System;
using System.Collections.Generic;

namespace RestaurantManagementSystem.Models
{
    public enum TableStatus
    {
        Available = 1,
        Occupied = 2,
        Reserved = 3,
        OutOfService = 4
    }

    public enum OrderStatus
    {
        Open = 1,
        Submitted = 2,
        Paid = 3,
        Cancelled = 4
    }

    public enum UserRole
    {
        Admin = 1,
        Waiter = 2
    }

    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string CardUid { get; set; }
    }

    public class Waiter
    {
        public int WaiterId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
    }

    public class StaffSchedule
    {
        public int ScheduleId { get; set; }
        public int WaiterId { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ShiftName { get; set; }
    }

    public class RestaurantTable
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public TableStatus Status { get; set; }
    }

    public class MenuCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public int WaiterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal CashReceived { get; set; }
        public decimal ChangeAmount { get; set; }
        public DateTime PaidAt { get; set; }
    }

    public class SalesReportRow
    {
        public DateTime DateKey { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public string SoldItemsSummary { get; set; }
    }

    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public User User { get; set; }
    }
}
