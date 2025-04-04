namespace GenSpil.Model
{
    public class BoardGameVariant
    {
        public string Title { get; private set; }
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
    }
}
