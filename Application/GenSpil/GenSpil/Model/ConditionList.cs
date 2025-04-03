namespace GenSpil.Model
{
    public class ConditionList
    {
        public List<Condition> Conditions { get; private set; }

        public ConditionList()
        {
            Conditions = new List<Condition>();
            foreach (Type.Condition condition in Enum.GetValues(typeof(Type.Condition)))
            {
                Conditions.Add(new Condition(condition, 0, 0));
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
