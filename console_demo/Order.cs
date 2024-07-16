namespace console_demo
{
    public class Order
    {
        public Order()
        {

        }
        public int ID { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Price { get; set; }
    }
}
