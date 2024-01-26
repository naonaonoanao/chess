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
                Password = password,
                WinCount = 0,
                LoseCount = 0,
                DrawCount = 0
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

        public void IncrementWins(string username)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.WinCount++;
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Пользователь не найден.");
            }
        }

        public void IncrementLoses(string username)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.LoseCount++;
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Пользователь не найден.");
            }
        }

        public void IncrementDraw(string username)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.DrawCount++;
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Пользователь не найден.");
            }
        }

        public User GetUserByUsername(string username)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                return user;
            }
            else
            {
                throw new Exception("Пользователь не найден.");
            }
        }
    }
}