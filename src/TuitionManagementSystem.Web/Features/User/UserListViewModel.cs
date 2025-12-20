namespace TuitionManagementSystem.Web.Features.User;

public class UserListViewModel
{
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = default!;
}
