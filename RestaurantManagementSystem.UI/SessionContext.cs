using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.UI
{
    public class SessionContext
    {
        public User CurrentUser { get; set; }
        public bool IsAdmin => CurrentUser != null && CurrentUser.RoleId == (int)UserRole.Admin;
        public bool IsWaiter => CurrentUser != null && CurrentUser.RoleId == (int)UserRole.Waiter;
    }
}
