using GenSpil.Type;

namespace GenSpil.Model
{
    public class BoardGame
    {
        public Guid Guid { get; private set; }
        public string Title { get; set; }
        public List<BoardGameVariant> Variants { get; private set; }
        public List<Genre> Genre { get; private set; }

        /// <summary>
        /// Constructor til at at samle al data.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="variants">List of game board variants</param>
        /// <param name="genre"></param>
        public BoardGame(string title, List<BoardGameVariant> variants, List<Genre> genre)
        {
            Title = title;
            Variants = variants;
            Genre = genre;
        }

        public void SetGuid(Guid guid)
        {
            Guid = guid;
        }

        public override string ToString()
        {
            string result = $"Titel : {Title}\n";
            result += "Genre : ";
            string prefix = "";
            foreach (var item in this.Genre)
            {
                result += $"{prefix}{item}";
                prefix = ", ";
            }
            return result;
        }
    }
}
