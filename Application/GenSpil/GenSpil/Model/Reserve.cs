
namespace GenSpil.Model;

public class Reserve
{

    /// <summary>
    /// Egenskaber for Reserve class 
    /// </summary>
    DateTime ReservedDate { get; set; }
    int Quantity { get; set; }
    int CustomerID { get; set; }

    /// <summary>
    /// Constructor for Reserve class
    /// </summary>
    /// <param name="reservedDate"></param>
    /// <param name="quantity"></param>
    /// <param name="customerID"></param>
    public Reserve(DateTime reservedDate, int quantity, int customerID, int boardGameID)
    {
        ReservedDate = reservedDate;
        Quantity = quantity;
        CustomerID = customerID;

    }

}
