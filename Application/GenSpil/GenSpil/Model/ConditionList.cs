using System.Text.Json.Serialization;

namespace GenSpil.Model
{
    public class ConditionList
    {
        public ICollection<Condition> Conditions { get; private set; }
        [JsonConstructor]
        public ConditionList()
        {
            Conditions = new List<Condition>();
            foreach (Type.Condition condition in Enum.GetValues(typeof(Type.Condition)))
            {
                Conditions.Add(new Condition(condition, 0, 0m));
            }
        }

        public void SetPrice(Type.Condition condition, decimal price)
        {
            var conditionItem = Conditions.FirstOrDefault(c => c.ConditionEnum == condition);
            if (conditionItem != null)
            {
                conditionItem.Price = price;
            }
        }

        public void SetQuantity(Type.Condition condition, int quantity)
        {
            var conditionItem = Conditions.FirstOrDefault(c => c.ConditionEnum == condition);
            if (conditionItem != null)
            {
                conditionItem.Quantity = quantity;
            }
        }

        public override string ToString()
        {
            if (Conditions.Count == 0)
            {
                return "Intet fundet.";
            }
            string result = "--- Tilstand ---\n";
            foreach (var condition in Conditions)
            {
                result += condition.ToString() + "\n";
            }
            return result;
        }
    }
}
