namespace ShopDataBase
{
    public class ListItem
    {
        public ListItem(Item<string, int> data)
        {
            Data = data;
        }
        public Item<string, int> Data { get; set; }
        public ListItem Next { get; set; }
    }
}
