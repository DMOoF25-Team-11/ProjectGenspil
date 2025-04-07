using System.Text.Json.Serialization;

namespace GenSpil.Model
{
    public sealed class CustomerList
    {
        static CustomerList? instance = null; ///> Singleton instance of the CustomerList
        static readonly object _lock = new object(); ///> Lock object for thread safety
        public static CustomerList Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new CustomerList();
                    }
                    return instance;
                }
            }
        } ///> Get or set singleton instance of the CustomerList

        public List<Customer> Customers { get; set; } ///> List of customers

        private CustomerList()
        {
            Customers = new List<Customer>();
        }

        [JsonConstructor]
        private CustomerList(List<Customer> customers)
        {
            Customers = customers;
        }

        //Adds a customer to the list.
        public void Add(Customer customer)
        {
            Customers.Add(customer);
        }

        //Removes a customer to the list.
        public void RemoveCustomer(Customer customer)
        {
            Customers.Remove(customer);
        }

        //Findes a customer on the list.
        public Customer? GetCustomerByID(int customerID)
        {
            return Customers.Find(c => c.Id == customerID);
        }

        //Update function isn't completed.
        public void UpdateCustomer(int customerID, string newName, string newAddress)
        {
            var customer = Customers.Find(c => c.Id == customerID);

            if (customer != null)
            {
                // If customer exists, update the name and address using the setter methods
                customer.SetName(newName);
                customer.SetAddress(newAddress);

                Console.WriteLine($"Customer with ID {customerID} has been updated.");
            }
            else
            {
                // If the customer doesn't exist
                Console.WriteLine($"Customer with ID {customerID} not found.");
            }
        }


        public int GenerateID()
        {
            if (Customers.Count == 0)
            {
                return 1;
            }
            return Customers.Max(c => c.Id) + 1;
        }

        public void Clear()
        {
            Customers.Clear();
        }

        public bool Exists(int customerID)
        {
            return Customers.Any(c => c.Id == customerID);
        }
    }
}
