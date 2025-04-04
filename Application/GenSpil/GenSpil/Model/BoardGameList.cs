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

    public ICollection<BoardGame> FilterByTitle(string? title, ICollection<BoardGame> games)
    {
        if (!string.IsNullOrEmpty(title))
        {
            games = games.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
            if (games.Count == 0)
            {
                return new List<BoardGame>();
            }
        }
        return games;
    }

    public ICollection<BoardGame> FilterByGenre(List<Type.Genre>? genre, ICollection<BoardGame> games)
    {
        if (genre != null)
        {
            games = games.Where(x => x.Genre.Any(g => genre.Contains(g))).ToList();
            if (games.Count == 0)
            {
                return new List<BoardGame>();
            }
        }
        return games;
    }

    public ICollection<BoardGame> FilterByVariant(string? variant, ICollection<BoardGame> games)
    {
        if (string.IsNullOrEmpty(variant))
        {
            return games;
        }

        var filteredGames = new List<BoardGame>();

        foreach (var game in games)
        {
            var filteredVariants = game.Variants.Where(v => v.Title.Contains(variant, StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredVariants.Any())
            {
                var filteredGame = new BoardGame(game.Title, game.Genre, filteredVariants);
                filteredGame.SetGuid(game.Guid);
                filteredGames.Add(filteredGame);
            }
        }

        return filteredGames;
    }

    //public ICollection<BoardGame> FindByVariant(string? variant, ICollection<BoardGame> games)
    //{
    //    List<BoardGameVariant> removeVariant = new List<BoardGameVariant>();
    //    List<BoardGame> removeBoardGame = new List<BoardGame>();
    //    if (variant != null && variant != "")
    //    {
    //        for (int i = 0; i < games.Count(); i++)
    //        {
    //            for (int j = 0; j < games.ElementAt(i).Variants.Count; ++j)
    //                if (!games.ElementAt(i).Variants[j].Title.Contains(variant))
    //                    removeVariant.Add(games.ElementAt(i).Variants[j]);
    //            foreach (BoardGameVariant item in removeVariant)
    //                games.ElementAt(i).Variants.Remove(item);
    //            if (games.ElementAt(i).Variants.Count() == 0)
    //                removeBoardGame.Add(games.ElementAt(i));
    //        }
    //        foreach (var item in removeBoardGame)
    //            games.Remove(item);
    //    }
    //    return games;
    //}

    //public ICollection<BoardGame> FindByCondition(List<Type.Condition>? condition, ICollection<BoardGame> games)
    //{
    //    List<Condition> removeCondition = new List<Condition>();
    //    List<BoardGame> removeBoardGame = new List<BoardGame>();
    //    if (condition != null)
    //    {
    //        for (int i = 0; i < games.Count(); i++)
    //        {
    //            for (int j = 0; j < games.ElementAt(i).Variants.Count; ++j)
    //            {
    //                for (int k = 0; k < games.ElementAt(i).Variants[j].ConditionList.Conditions.Count(); ++k)
    //                    if (!condition.Contains(games.ElementAt(i).Variants[j].ConditionList.Conditions.ElementAt(k).ConditionEnum))
    //                        removeCondition.Add(games.ElementAt(i).Variants[j].ConditionList.Conditions.ElementAt(k));
    //                foreach (var item in removeCondition)
    //                    games.ElementAt(i).Variants[j].ConditionList.Conditions.Remove(item);
    //            }
    //        }
    //    }
    //    return games;
    //}

    public ICollection<BoardGame> FilterByCondition(List<Type.Condition>? condition, ICollection<BoardGame> games)
    {
        if (condition == null || condition.Count == 0)
        {
            return games;
        }

        var filteredGames = new List<BoardGame>();

        foreach (var game in games)
        {
            var filteredVariants = new List<BoardGameVariant>();

            foreach (var variant in game.Variants)
            {
                var filteredConditions = variant.ConditionList.Conditions
                    .Where(c => condition.Contains(c.ConditionEnum))
                    .ToList();

                if (filteredConditions.Any())
                {
                    var filteredConditionList = new ConditionList();
                    filteredConditionList.Conditions.Clear(); // Clear conditions before adding filtered ones
                    foreach (var cond in filteredConditions)
                    {
                        filteredConditionList.Conditions.Add(cond);
                    }
                    var filteredVariant = new BoardGameVariant(variant.Title, variant.NumbersOfPlayers, filteredConditionList);
                    filteredVariants.Add(filteredVariant);
                }
            }

            if (filteredVariants.Any())
            {
                var filteredGame = new BoardGame(game.Title, game.Genre, filteredVariants);
                filteredGame.SetGuid(game.Guid);
                filteredGames.Add(filteredGame);
            }
        }

        return filteredGames;
    }

    public ICollection<BoardGame> FilterByPrice(string? price, ICollection<BoardGame> games)
    {
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
            ICollection<BoardGame> filteredBoardGames = BoardGames.Where(x => x.Title != null).ToList();
            filteredBoardGames = FilterByTitle(title, filteredBoardGames);
            filteredBoardGames = FilterByGenre(genre, filteredBoardGames);
            filteredBoardGames = FilterByVariant(variant, filteredBoardGames);
            filteredBoardGames = FilterByCondition(condition, filteredBoardGames);
            filteredBoardGames = FilterByPrice(price, filteredBoardGames);
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
