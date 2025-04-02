using GenSpil.Type;

namespace GenSpil.Model
{
    public class BoardGame
    {
        public int BoardGameID { get; set; }
        public string Title { get; set; }
        public List<BoardGameVariant> Variants { get; private set; }
        public List<Genre> Genre { get; private set; }

        /// <summary>
        /// Constructor til at at samle al data, i en.
        /// </summary>
        /// <param name="boardGameID"></param>
        /// <param name="title"></param>
        /// <param name="variants">List of game board variants</param>
        /// <param name="genre"></param>
        public BoardGame(int boardGameID, string title, List<BoardGameVariant> variants, List<Genre> genre)
        {
            BoardGameID = boardGameID;
            Title = title;
            Variants = variants;
            Genre = genre;
        }

        public override string ToString()
        {
            string result = $"Titel {Title}\n";
            return result;
        }
    }
}
