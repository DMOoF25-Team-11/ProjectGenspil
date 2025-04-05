using TirsvadCLI.AnsiCode;

namespace GenSpil.Model
{
    public class Condition
    {
        public Type.Condition ConditionEnum { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        /// <summary>
        /// Constructor til Condition
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        public Condition(Type.Condition condition, int quantity, decimal price)
        {
            ConditionEnum = condition;
            Quantity = quantity;
            Price = price;
        }

        /// <summary>
        /// Sets the price of the condition.
        /// </summary>
        /// <param name="price"></param>
        public void SetPrice(decimal price)
        {
            Price = price;
        }

        /// <summary>
        /// Sets the quantity of the condition.
        /// </summary>
        /// <param name="quantity"></param>
        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
        }



        public override string ToString()
        {
            string result;
            if (Quantity == 0)
            {
                result = $"{AnsiCode.BRIGHT_BLACK}{ConditionEnum} - Ingen på lager";
            }
            else
            {
                result = $"{ConditionEnum} - Antal: {Quantity}";
            }
            result += $", Pris: {Price} kr{AnsiCode.ANSI_RESET}";
            return result;
        }
    }
}
