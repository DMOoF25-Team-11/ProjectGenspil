namespace GenSpil.Model
{
    public class BoardGameVariant
    {
        public string Title { get; private set; }
        public ICollection<Reserve> Reserves { get; private set; } = null!; // Initialize to null to avoid unassigned variable error
        public string NumbersOfPlayers { get; set; }
        public ConditionList ConditionList { get; private set; }

        public BoardGameVariant(string title, string numbersOfPlayers, ConditionList conditionList, ICollection<Reserve>? reserves = null)
        {
            Title = title;
            NumbersOfPlayers = numbersOfPlayers;
            if (conditionList != null)
                ConditionList = conditionList;
            else
                ConditionList = new ConditionList();
            if (reserves != null)
                Reserves = reserves;
            else
                Reserves = new List<Reserve>();
        }

        public override string ToString()
        {
            return Title;
        }

        public ICollection<Reserve> GetReserved()
        {
            return Reserves;
        }

        public void SetReserved(ICollection<Reserve> reserved)
        {
            Reserves = reserved;
        }
    }
}
