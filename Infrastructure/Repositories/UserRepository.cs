using IlkRepom.Domain.Entities;

namespace IlkRepom.Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly List<User> users = new();

        public List<User> GetAll()
        {
            return users;
        }

        public User? GetById(int id)
        {
            return users.FirstOrDefault(x => x.Id == id);
        }

        public void Add(User user)
        {
            users.Add(user);
        }

        public bool Update(int id, User user)
        {
            var existingUser = GetById(id);

            if (existingUser == null)
                return false;

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.IsActive = user.IsActive;

            return true;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);

            if (user == null)
                return false;

            users.Remove(user);
            return true;
        }
    }
}
