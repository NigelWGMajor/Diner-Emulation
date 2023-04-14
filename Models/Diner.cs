namespace Models.Common
{

    public class Diner : Person
    {
        public Diner(string name)
        {
            Name = name;
        }
        public void OrderFood(params MenuItem[] menuItems)
        {
            foreach (var menuItem in menuItems)
            {
                Selection.Add(menuItem);
            }
        }
        public string Name { get; set; }

        public int TableNumber { get; set; }
        public List<MenuItem> Selection { get; set; } = new();
    }
}