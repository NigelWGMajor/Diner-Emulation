namespace Models.Common
{
    public class Server : Person
    {
        public Server(string name)
        {
            Name = name;
        }
        public List<int> TableNumbers { get; set; } = new();
    }
}
