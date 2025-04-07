namespace GenSpil.Model
{
    public class Customer
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }

        public Customer(int id, string name, string? address)
        {
            Id = id;
            Name = name;
            Address = address ?? "";
        }

        //takes input name and assigns the value to Name.
        public void SetName(string name)
        {
            Name = name;
        }

        //takes input address and assigns the value to Address.
        public void SetAddress(string address)
        {
            Address = address;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Address: {Address}";
        }
    }
}
