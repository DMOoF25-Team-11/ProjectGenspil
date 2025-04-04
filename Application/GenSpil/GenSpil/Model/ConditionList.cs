namespace GenSpil.Model
{
    public class ConditionList
    {
        public Condition[] Conditions { get; private set; } = new Condition[5];

        public ConditionList()
        {
            int i;
            for (i = 0; i < Conditions.Length; i++)
            {
                Conditions[i] = new Condition((Type.Condition)i, 0, 0m); // Initialize each element
            }
            //i = 0;
            //foreach (Type.Condition condition in Enum.GetValues(typeof(Type.Condition)))
            //{
            //    Conditions[i].ConditionEnum = condition;
            //    Conditions[i].Quantity = 0;
            //    i++;
            //}
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
            if (Conditions.Length == 0)
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
