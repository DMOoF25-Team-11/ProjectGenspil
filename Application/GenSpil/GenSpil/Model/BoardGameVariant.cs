namespace GenSpil.Model
{
    public class BoardGameVariant
    {
        public string Title { get; private set; }
        public ConditionList ConditionList { get; private set; }

        public BoardGameVariant(string title, ConditionList conditionList)
        {
            Title = title;
            ConditionList = conditionList;
        }
    }
}
