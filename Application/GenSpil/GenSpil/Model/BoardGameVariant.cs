namespace GenSpil.Model
{
    public class BoardGameVariant
    {
        public string Title { get; private set; }
        public Reserve Reserved { get; private set; } = null!; // Initialize to null to avoid unassigned variable error
        public string NumbersOfPlayers { get; set; }
        public ConditionList ConditionList { get; private set; }

        public BoardGameVariant(string title, string numbersOfPlayers, ConditionList conditionList)
        {
            Title = title;
            NumbersOfPlayers = numbersOfPlayers;
            if (conditionList != null)
            {
                ConditionList = conditionList;
            }
            else
            {
                ConditionList = new ConditionList();
            }
        }

        public override string ToString()
        {
            return Title;
        }

        public Reserve GetReserved()
        {
            return Reserved;
        }

        public void SetReserved(Reserve reserved)
        {
            Reserved = reserved;
        }
    }
}
