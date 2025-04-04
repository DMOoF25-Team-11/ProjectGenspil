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

    public List<BoardGame> BoardGames { get; set; }

    [JsonConstructor]
    BoardGameList()
    {
        BoardGames = new List<BoardGame>();
#if DEBUG
        Seed();
#endif
    }

    /// <summary>
    /// Tilføjer et brætspil.
    /// </summary>
    public void Add(BoardGame boardGame)
    {
        BoardGames.Add(boardGame);
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

    public static IEnumerable<BoardGame> FindByTitle(string? title, IEnumerable<BoardGame> games)
    {
        // Filter Title
        if (title != null & title != "")
        {
            var boardGame = games.Where(x => x.Title.Contains(title!, StringComparison.OrdinalIgnoreCase)).ToList();
            if (boardGame.Count == 0)
            {
                return new List<BoardGame>();
            }
            return boardGame;
        }
        return games;
    }

    public IEnumerable<BoardGame> FindByGenre(List<Type.Genre>? genre, IEnumerable<BoardGame> games)
    {
        // Filter Genre
        if (genre != null)
        {
            var boardGame = BoardGames.Where(x => x.Genre.Any(g => genre.Contains(g))).ToList();
            if (boardGame.Count == 0)
            {
                return new List<BoardGame>();
            }
            return boardGame;
        }
        return games;
    }

    public IEnumerable<BoardGame> FindByVariant(string? variant, IEnumerable<BoardGame> games)
    {
        // Filter Variant
        if (variant != null & variant != "")
        {
            var boardGame = BoardGames.Where(x => x.Variants.Any(v => v.Title.Contains(variant!, StringComparison.OrdinalIgnoreCase))).ToList();
            if (boardGame.Count == 0)
            {
                return new List<BoardGame>();
            }
            return boardGame;
        }
        return games;
    }

    public IEnumerable<BoardGame> FindByCondition(List<Type.Condition>? condition, IEnumerable<BoardGame> games)
    {
        // Filter Condition
        if (condition != null)
        {
            var boardGame = BoardGames.Where(x => x.Variants.Any(v => v.ConditionList.Conditions.Any(c => condition.Contains(c.ConditionEnum)))).ToList();
            if (boardGame.Count == 0)
            {
                return new List<BoardGame>();
            }
            return boardGame;
        }
        return games;
    }

    public IEnumerable<BoardGame> FindByPrice(string? price, IEnumerable<BoardGame> games)
    {
        // Filter Price
        if (price != null && price != "")
        {
            string priceOperator;
            if (price.Contains(">=") | price.Contains("=>"))
                priceOperator = ">=";
            else if (price.Contains("<=") | price.Contains("=<"))
                priceOperator = "<=";
            else
                priceOperator = "=";
            decimal priceValue = decimal.Parse(price);
            var boardGame = BoardGames.Where(x => x.Variants.Any(v => v.ConditionList.Conditions.Any(c => c.Price == priceValue))).ToList();
            if (boardGame.Count == 0)
            {
                return new List<BoardGame>();
            }
            return boardGame;
        }
        return games;
    }

    /// <summary>
    /// Søg efter brætspil
    /// </summary>
    public List<BoardGame> Search(string? title, List<Type.Genre>? genre, string? variant, List<Type.Condition>? condition, string? price)
    {
        lock (_lock)
        {
            var filteredBoardGames = BoardGames.AsEnumerable();
            filteredBoardGames = BoardGameList.FindByTitle(title, filteredBoardGames);
            filteredBoardGames = FindByGenre(genre, filteredBoardGames);
            filteredBoardGames = FindByVariant(variant, filteredBoardGames);
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
#if DEBUG
    private void Seed()
    {
        BoardGameVariant boardGameVariant;
        List<BoardGameVariant> boardGameVariants = new List<BoardGameVariant>();

        Console.WriteLine("Seeding board games...");
        boardGameVariant = new BoardGameVariant("", "1-4", new ConditionList());
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Quantity = 5;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Price = 200;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Quantity = 1;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Price = 175;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Price = 150;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Quantity = 2;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Price = 125;
        BoardGames.Add(new BoardGame(1, "Catan", new List<BoardGameVariant> { boardGameVariant }, new List<Type.Genre> { Type.Genre.Strategi }));

        boardGameVariant = new BoardGameVariant("", "1-4", new ConditionList());
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Price = 250;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Quantity = 1;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Price = 220;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Price = 200;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Price = 100;
        boardGameVariants.Add(boardGameVariant);

        boardGameVariant = new BoardGameVariant("Europe", "1-4", new ConditionList());
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Price = 250;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Price = 220;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Quantity = 2;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Price = 200;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Quantity = 1;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Price = 100;
        boardGameVariants.Add(boardGameVariant);

        boardGameVariant = new BoardGameVariant("Märklin", "1-4", new ConditionList());
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Price = 250;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Price = 220;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Quantity = 2;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Price = 200;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Quantity = 1;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Price = 100;
        boardGameVariants.Add(boardGameVariant);


        boardGameVariant = new BoardGameVariant("Nordic Countries", "1-4", new ConditionList());
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Ny).First().Price = 250;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Quantity = 0;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.God).First().Price = 220;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Quantity = 2;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Brugt).First().Price = 200;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Quantity = 1;
        boardGameVariant.ConditionList.Conditions.Where(c => c.ConditionEnum == Type.Condition.Slidt).First().Price = 100;
        boardGameVariants.Add(boardGameVariant);
        BoardGames.Add(new BoardGame(2, "Ticket to Ride", boardGameVariants, new List<Type.Genre> { Type.Genre.Strategi }));
    }
#endif
}
