using System.Text.Json.Serialization;

namespace GenSpil.Model;

/// <summary>
/// Singleton class for handling a list of board games.
/// TODO Should not interact with the user directly. (Tirsvad)
/// </summary>
public sealed class BoardGameList
{
    static BoardGameList? instance = null;
    static readonly object _lock = new object();
    public static BoardGameList Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new BoardGameList();
                }
                return instance;
            }
        }
    } ///> Singleton instance of the BoardGameList

    int _index = 0;
    public List<BoardGame> BoardGames { get; set; }

    [JsonConstructor]
    BoardGameList()
    {
        BoardGames = new List<BoardGame>();
    }

    /// <summary>
    /// Tilføjer et brætspil.
    /// </summary>
    public void Add(BoardGame boardGame)
    {
        Guid guid = Guid.NewGuid();
        do
        {
            guid = Guid.NewGuid();
        } while (BoardGames.Any(bg => bg.Guid == guid));
        boardGame.SetGuid(guid);
        this.BoardGames.Add(boardGame);
    }

    public void Add(BoardGameVariant boardGameVariant, Guid guid)
    {
        BoardGames.Where(x => x.Guid == guid).First().Variants.Add(boardGameVariant);
    }

    public void Clear()
    {
        BoardGames.Clear();
    }

    /// <summary>
    /// Vis brætspil
    /// </summary>
    public override string ToString()
    {
        if (BoardGames.Count == 0)
        {
            return "Ingen brætspil fundet.";
        }
        string result = "--- Brætspil ---\n";
        foreach (var game in BoardGames)
        {
            result += game.ToString();
        }
        return result;
    }

    public IEnumerable<BoardGame> FindByTitle(string? title, IEnumerable<BoardGame> games)
    {
        // Filter Title
        //if (title != null & title != "")
        //{
        //    var boardGame = games.Where(x => x.Title.Contains(title!, StringComparison.OrdinalIgnoreCase)).ToList();
        //    if (boardGame.Count == 0)
        //    {
        //        return new List<BoardGame>();
        //    }
        //    return boardGame;
        //}
        return games;
    }

    public IEnumerable<BoardGame> FindByGenre(List<Type.Genre>? genre, IEnumerable<BoardGame> games)
    {
        // Filter Genre
        //if (genre != null)
        //{
        //    var boardGame = BoardGames.Where(x => x.Genre.Any(g => genre.Contains(g))).ToList();
        //    if (boardGame.Count == 0)
        //    {
        //        return new List<BoardGame>();
        //    }
        //    return boardGame;
        //}
        return games;
    }

    public IEnumerable<BoardGame> FindByVariant(string? variant, IEnumerable<BoardGame> games)
    {
        // Filter Variant
        //if (variant != null & variant != "")
        //{
        //    var boardGame = BoardGames.Where(x => x.Variants.Any(v => v.Title.Contains(variant!, StringComparison.OrdinalIgnoreCase))).ToList();
        //    if (boardGame.Count == 0)
        //    {
        //        return new List<BoardGame>();
        //    }
        //    return boardGame;
        //}
        return games;
    }

    public IEnumerable<BoardGame> FindByCondition(List<Type.Condition>? condition, IEnumerable<BoardGame> games)
    {
        // Filter Condition
        //if (condition != null)
        //{
        //    var boardGame = BoardGames.Where(x => x.Variants.Any(v => v.ConditionList.Conditions.Any(c => condition.Contains(c.ConditionEnum)))).ToList();
        //    if (boardGame.Count == 0)
        //    {
        //        return new List<BoardGame>();
        //    }
        //    return boardGame;
        //}
        return games;
    }

    public IEnumerable<BoardGame> FindByPrice(string? price, IEnumerable<BoardGame> games)
    {
        // Filter Price
        //if (price != null && price != "")
        //{
        //    string priceOperator;
        //    if (price.Contains(">=") | price.Contains("=>"))
        //        priceOperator = ">=";
        //    else if (price.Contains("<=") | price.Contains("=<"))
        //        priceOperator = "<=";
        //    else
        //        priceOperator = "=";
        //    decimal priceValue = decimal.Parse(price);
        //    var boardGame = BoardGames.Where(x => x.Variants.Any(v => v.ConditionList.Conditions.Any(c => c.Price == priceValue))).ToList();
        //    if (boardGame.Count == 0)
        //    {
        //        return new List<BoardGame>();
        //    }
        //    return boardGame;
        //}
        return games;
    }

    public BoardGame? GetBoardGameById(Guid guid)
    {
        return BoardGames.Where(x => x.Guid == guid).FirstOrDefault();
    }

    /// <summary>
    /// Søg efter brætspil
    /// </summary>
    public List<BoardGame> Search(string? title, List<Type.Genre>? genre, string? variant, List<Type.Condition>? condition, string? price)
    {
        lock (_lock)
        {
            var filteredBoardGames = BoardGames.AsEnumerable();
            filteredBoardGames = FindByTitle(title, filteredBoardGames);
            filteredBoardGames = FindByGenre(genre, filteredBoardGames);
            filteredBoardGames = FindByVariant(variant, filteredBoardGames);
            filteredBoardGames = FindByCondition(condition, filteredBoardGames);
            filteredBoardGames = FindByPrice(price, filteredBoardGames);
            //// Filter Variant
            //if (variant != null & variant != "")
            //{
            //    for (int i = 0; i < filteredBoardGames.Count(); i++)
            //    {
            //        var game = filteredBoardGames.ElementAt(i);
            //        for (int j = 0; j < game.Variants.Count; j++)
            //        {
            //            var v = game.Variants.ElementAt(j);
            //            if (!v.Title.Contains(variant, StringComparison.OrdinalIgnoreCase))
            //            {
            //                filteredBoardGames.ElementAt(i).Variants.Remove(v);
            //            }
            //        }
            //    }
            //    filteredBoardGames = filteredBoardGames.Where(x => x.Variants.Any(v => v.Title.Contains(variant, StringComparison.OrdinalIgnoreCase)));
            //}
            // Filter Condition
            //if (condition != null)
            //{
            //    for (int i = 0; i < filteredBoardGames.Count(); i++)
            //    {
            //        var game = filteredBoardGames.ElementAt(i);
            //        for (int j = 0; j < game.Variants.Count; j++)
            //        {
            //            var v = game.Variants.ElementAt(j);
            //            List<Condition> itemsToRemove = new List<Condition>();
            //            foreach (var c in filteredBoardGames.ElementAt(i).Variants.ElementAt(j).ConditionList.Conditions)
            //            {
            //                if (!condition.Contains(c.ConditionEnum))
            //                {
            //                    itemsToRemove.Add(c);
            //                }
            //            }
            //            foreach (var item in itemsToRemove)
            //            {
            //                filteredBoardGames.ElementAt(i).Variants.ElementAt(j).ConditionList.Conditions.Remove(item);
            //            }
            //        }
            //    }
            //}
            //if (price != null & price != "")
            //{
            //    string priceOperator;
            //    if (price.Contains(">=") | price.Contains("=>"))
            //        priceOperator = ">=";
            //    else if (price.Contains("<=") | price.Contains("=<"))
            //        priceOperator = "<=";
            //    else
            //        priceOperator = "=";
            //    //    price = new string(price.Where(char.IsDigit).ToArray());
            //    decimal priceValue = decimal.Parse(price);
            //    for (int i = 0; i < filteredBoardGames.Count(); i++)
            //    {
            //        var game = filteredBoardGames.ElementAt(i);
            //        for (int j = 0; j < game.Variants.Count; j++)
            //        {
            //            var v = game.Variants.ElementAt(j);
            //            for (int k = 0; k < v.ConditionList.Conditions.Count; k++)
            //            {
            //                var c = v.ConditionList.Conditions.ElementAt(k);
            //                {
            //                    filteredBoardGames.ElementAt(i).Variants.ElementAt(j).ConditionList.Conditions.Remove(c);
            //                }
            //                if (!v.Title.Contains(variant, StringComparison.OrdinalIgnoreCase))
            //                {
            //                    filteredBoardGames.ElementAt(i).Variants.Remove(v);
            //                }
            //            }
            //        }

            //        //filteredBoardGames = filteredBoardGames.Where(x => x.Variants.Any(v => v.ConditionList.Any(c => c. >= decimal.Parse(price.Replace(">=", ""))));
            //        //    }
            //        //    else if (price.Contains("<="))
            //        //    {
            //        //        //filteredBoardGames = filteredBoardGames.Where(x => x.Variants.Any(v => v.Price <= decimal.Parse(price.Replace("<=", ""))));
            //        //    }
            //        //    else
            //        //    {
            //        //        //filteredBoardGames = filteredBoardGames.Where(x => x.Variants.Any(v => v.Price == decimal.Parse(price)));
            //        // }
            //    }
            //}
            return filteredBoardGames.ToList();
        }
    }

    /// <summary>
    /// Fjern brætspil
    /// </summary>
    public void Remove(BoardGame boardGame) //TODO Could be renamed to Remove and take parameter BoardGame. Should not interaction with user (Tirsvad)
    {
        BoardGames.Remove(boardGame);
    }
}
