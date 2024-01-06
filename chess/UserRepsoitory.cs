namespace uwp
{
    using System;
    using System.Linq;

    public class UserRepository
    {
        private MyDbContext dbContext;

        public UserRepository()
        {
            dbContext = new MyDbContext();
        }

        public void AddUser(string username, string password)
        {
            var existingUser = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким именем уже существует.");
            }

            var newUser = new User
            {
                Username = username,
                Password = password
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
        }

        public bool VerifyUser(string username, string password)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            return user != null;
        }

        public void ChangePassword(string username, string newPassword)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.Password = newPassword;
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Пользователь не найден.");
            }
        }
    }
}