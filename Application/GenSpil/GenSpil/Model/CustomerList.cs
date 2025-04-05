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

        [JsonConstructor]
        private CustomerList()
        {
            Customers = new List<Customer>();
        }

        //Adds a customer to the list.
        public void AddCustomer(Customer customer)
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
            return Customers.Find(c => c.CustomerID == customerID);
        }

        //Update function isn't completed.
        public void UpdateCustomer(int customerID, string newName, string newAddress)
        {
            var customer = Customers.Find(c => c.CustomerID == customerID);

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


        public int GenerateID(int customerID)
        {
            if (Customers.Count == 0)
            {
                return 1;
            }

            return Customers.Max(c => c.CustomerID) + 1;
        }

        public void Clear()
        {
            Customers.Clear();
        }
    }
}
