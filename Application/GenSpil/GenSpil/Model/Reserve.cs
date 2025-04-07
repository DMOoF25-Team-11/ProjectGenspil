namespace GenSpil.Model;

public class Reserve
{
    /// <summary>
    /// Egenskaber for Reserve class 
    /// </summary>
    public DateTime ReservedDate { get; private set; }
    public int Quantity { get; private set; }
    public int CustomerID { get; private set; }

    /// <summary>
    /// Constructor for Reserve class
    /// </summary>
    /// <param name="reservedDate"></param>
    /// <param name="quantity"></param>
    /// <param name="customerID"></param>
    public Reserve(DateTime reservedDate, int quantity, int customerID)
    {
        ReservedDate = reservedDate;
        Quantity = quantity;
        CustomerID = customerID;
    }
}
