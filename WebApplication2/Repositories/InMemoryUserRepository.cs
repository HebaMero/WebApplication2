using System.Xml.Linq;
using WebApplication2;
using WebApplication2.Repositories;

namespace WebApplication2.Repositories
{
    public class InMemoryUserRepository : IUserRepository // Fixed missing colon and added interface implementation
    {
        private readonly List<User> _users = new();

        public InMemoryUserRepository()
        {
            // seed with sample users
            _users.Add(new User { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Role = "HR" });
            _users.Add(new User { FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", Role = "IT" });
        }

        public IEnumerable<User> GetAll() => _users.OrderBy(u => u.CreatedAt);

        public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);

        public User Create(User user)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            _users.Add(user);
            return user;
        }

        public bool Update(Guid id, User updatedUser)
        {
            var existing = GetById(id);
            if (existing == null) return false;

            // update fields (avoid updating Id/CreatedAt)
            existing.FirstName = updatedUser.FirstName;
            existing.LastName = updatedUser.LastName;
            existing.Email = updatedUser.Email;
            existing.Role = updatedUser.Role;
            return true;
        }

        public bool Delete(Guid id)
        {
            var existing = GetById(id);
            if (existing == null) return false;
            return _users.Remove(existing);
        }
    }
}
