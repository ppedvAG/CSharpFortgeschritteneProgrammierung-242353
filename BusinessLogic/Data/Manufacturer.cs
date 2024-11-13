namespace BusinessLogic.Data
{
    public class Manufacturer
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
