namespace GenSpil.Model
{
    public class Customer
    {
        public int CustomerID { get; private set; }
        string Name { get; set; }
        string Address { get; set; }

        public Customer(int customerID, string name, string address)
        {
            CustomerID = customerID;
            Name = name;
            Address = address;
        }

        //Takes customer object as input and returns the CustomerID of that specific customer as an integer.
        public static int GetCustomerID(Customer customer)
        {
            return customer.CustomerID;
        }

        //Takes customer object as input and returns the Name of that specific customer as an string.
        public static string GetName(Customer customer)
        {
            return customer.Name;
        }

        //Takes customer object as input and returns the Address of that specific customer as an string.
        public static string GetAddress(Customer customer)
        {
            return customer.Address;
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
            return $"CustomerID: {CustomerID}, Name: {Name}, Address: {Address}";
        }
    }
}
