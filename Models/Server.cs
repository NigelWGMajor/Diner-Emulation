namespace Models
{
    public class Server
    {
        public string Name {get; set; }
        public List<int> TableNumbers {get; set; } = new();
    }
}