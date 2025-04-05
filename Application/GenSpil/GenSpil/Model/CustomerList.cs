using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GenSpil.Model
{
    public class CustomerList
    {
        
        private List<Customer> _customers;

        public CustomerList()
        {
            _customers = new List<Customer>();
        }

        //Adds a customer to the list.
        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        //Removes a customer to the list.
        public void RemoveCustomer(Customer customer)
        {
            _customers.Remove(customer);
        }

        //Findes a customer on the list.
        public Customer? GetCustomerByID(int customerID)
        {
            return _customers.Find(c => c.CustomerID == customerID);
        }

        //Update function isn't completed.
        public void UpdateCustomer(int customerID, string newName, string newAddress) 
        {
            var customer = _customers.Find(c => c.CustomerID == customerID);

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
            if (_customers.Count == 0)
            {
                return 1;
            }

            return _customers.Max(c => c.CustomerID) + 1;
        }
    }
}
