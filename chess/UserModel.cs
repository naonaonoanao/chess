using System.ComponentModel.DataAnnotations;

namespace uwp
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }

        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public int DrawCount { get; set; }

    }
}

