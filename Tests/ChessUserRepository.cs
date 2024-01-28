
using uwp;

namespace Tests
{
    [TestFixture]
    public class ChessUserRepositoryTests
    {
        private MyDbContext dbContext;
        private UserRepository userRepository;

        [SetUp]
        public void Setup()
        {
            dbContext = new MyDbContext();
            userRepository = new UserRepository();
        }

        [Test]
        public void AddUser_ValidInput_Success()
        {
            string username = "testuser";
            string password = "testpassword";

            userRepository.AddUser(username, password);

            Assert.IsTrue(userRepository.VerifyUser(username, password));
        }

        [Test]
        public void AddUser_ExistingUser_ThrowsException()
        {
            string username = "testuser2";
            string password = "testpassword2";
            userRepository.AddUser(username, password);

            Assert.Throws<InvalidOperationException>(() => userRepository.AddUser(username, password));
        }

        [Test]
        public void VerifyUser_ExistingUser_ReturnsTrue()
        {
            string username = "testuser3";
            string password = "testpassword3";
            userRepository.AddUser(username, password);

            bool result = userRepository.VerifyUser(username, password);

            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyUser_NonExistingUser_ReturnsFalse()
        {
            string username = "testuser1";
            string password = "testpassword1";

            bool result = userRepository.VerifyUser(username, password);

            Assert.IsFalse(result);
        }
    }
}
