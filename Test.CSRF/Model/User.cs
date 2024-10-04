namespace Test.CSRF.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public int Balance { get; set; }

        public User(int id, string name, string password, string authToken, int balance)
        {
            Id = id;
            Name = name;
            Password = password;
            AuthToken = authToken;
            Balance = balance;
        }
    }
}
