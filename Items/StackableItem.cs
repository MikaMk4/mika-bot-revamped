namespace MikaBotRevamped.Items
{
    [Serializable]
    public struct StackableItem
    {
        public int Id { get; set; }
        public int Amount { get; set; }

        public StackableItem(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}