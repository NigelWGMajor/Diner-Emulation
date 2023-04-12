namespace Models
{
    public class MenuItem
    {
        	public MenuItem(string kind, string name)
	{
		ItemName = name;
		ItemKind = kind;
	}
        public String ItemName { get; set; }

        public String ItemKind { get; set; }

        public bool IsAvaiable { get; set; }
    }
}
