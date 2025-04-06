using System.Globalization;
using System.Reflection;
using System.Text;
using GenSpil.Handler;
using GenSpil.Model;
using TirsvadCLI.Frame;
using TirsvadCLI.MenuPaginator;

namespace GenSpil;

internal class Program
{
    const string TITLE = "GenSpil";
    const string DATA_JSON_FILE = "./data/genspil.json";
    static BoardGameList _boardGameList;
    static Authentication _auth;
    static UserList _userList;
    static JsonFileHandler _jsonFileHandler = JsonFileHandler.Instance;

    /// <summary>
    /// Static constructor to initialize static fields.
    /// </summary>
    static Program()
    {
        _boardGameList = BoardGameList.Instance;
        _userList = UserList.Instance;
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("da-DK");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("da-DK");
        _auth = new Authentication();
    }
    /// <summary>
    /// Gets the version of the executing assembly.
    /// </summary>
    /// <returns>The version as a string.</returns>
    static string GetVersion()
    {
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        return version != null ? $"{version.Major}.{version.Minor}" : "Unknown version";
    }
    /// <summary>
    /// Login with username and password
    /// </summary>
    static void Login()
    {
        int cTop;
        int cInputLeft = 14;
        string? username;
        string? password;
        do
        {
            Console.CursorVisible = true;
            // Headline
            HeadLine("Log på");
            // Form 
            cTop = Console.CursorTop;
            Console.Write("Brugernavn");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
            Console.Write("Adgangskode");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
            // user input 
            Console.SetCursorPosition(cInputLeft, cTop++);
            username = ReadLineWithEscape();
            Console.SetCursorPosition(cInputLeft, cTop++);
            //TODO hide password input
            password = ReadLineWithEscape(true);
            Console.CursorVisible = false;
            // Authenticate
            if (username == null || password == null)
            {
                ErrorMessage("Brugernavn eller adgangskode er tom");
                continue;
            }

            if (_auth.Login(username, password))
            {
                Console.WriteLine($"Du er logget ind som {username}");
                var role = _auth.GetRole(username);
                Console.WriteLine($"Din rolle er {role}");
                break;
            }
            else
            {
                ErrorMessage("Forkert brugernavn eller adgangskode");

            }

        } while (true);


    }
    /// <summary>
    /// Logout the current user and prompt for login again.
    /// </summary>
    static void Logout()
    {
        _auth.Logout();
        Login();
    }
    /// <summary>
    /// Displays the details of a board game.
    /// </summary>
    /// <param name="boardGame">The board game to display.</param>
    /// <returns>The displayed board game.</returns>
    static BoardGame ShowBoardGame(BoardGame boardGame)
    {
        HeadLine(boardGame.Title);
        Console.WriteLine(boardGame.ToString());
        ShowBoardGameBody(boardGame);
        Console.WriteLine("\nTryk på en tast for at fortsætte...");
        Console.ReadKey();
        return boardGame;
    }
    /// <summary>
    /// Displays the details of a board game and its variants.
    /// </summary>
    static void ShowBoardGameBody(BoardGame boardGame)
    {
        int indent = 0;
        Console.WriteLine(boardGame.ToString());
        foreach (BoardGameVariant boardGameVariant in boardGame.Variants)
        {
            string prefix = "";
            if (boardGameVariant.Title == "")
                prefix = "";
            else
                prefix = " : ";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(boardGame.Title + prefix + boardGameVariant.ToString());
            Console.ResetColor();
            indent += 2;
            foreach (var conditions in boardGameVariant.ConditionList.Conditions)
            {
                string indentString = new string(' ', indent);
                Console.WriteLine(indentString + "Condition : " + conditions.ToString());
            }
            indent -= 2;
        }
    }
    /// <summary>
    /// Displays the details of a list of board games.
    /// </summary>
    /// <param name="boardGames">The list of board games to display.</param>
    static void ShowBoardGame(IEnumerable<BoardGame> boardGames)
    {
        foreach (BoardGame boardGame in boardGames)
        {
            ShowBoardGame(boardGame);
        }
    }
    /// <summary>
    /// Displays the details of a specific board game variant.
    /// </summary>
    /// <param name="boardGame">The board game to display.</param>
    /// <param name="boardGameVariant">The variant of the board game to display.</param>
    static void ShowBoardGameVariant(BoardGame boardGame, BoardGameVariant boardGameVariant)
    {
        HeadLine(boardGame.Title);
        Console.WriteLine(boardGame.ToString());
        Console.WriteLine("Variant : " + boardGameVariant.ToString());
        foreach (var conditions in boardGameVariant.ConditionList.Conditions)
        {
            Console.WriteLine("Condition : " + conditions.ToString());
        }
        Console.WriteLine("\nTryk på en tast for at fortsætte...");
        Console.ReadKey();
    }
    /// <summary>
    /// Prompts the user to add a new board game.
    /// </summary>
    static void AddBoardGame()
    {
        BoardGame? boardGame;
        do
        {
            Console.Clear();
            Console.CursorVisible = true;
            HeadLine("Tilføj brætspil");
            // Form
            int cTop = Console.CursorTop;
            int cInputLeft = 14;
            Console.Write("Title");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
            Console.Write("Genre");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
            for (int i = 0; i < Enum.GetValues<Type.Genre>().Length; i++)
            {
                object? genreValue = Enum.GetValues(typeof(Type.Genre)).GetValue(i);
                if (genreValue != null)
                {
                    Console.Write((int)genreValue);
                    Console.Write(" - ");
                    Console.WriteLine(Enum.GetName(typeof(Type.Genre), i));
                }
            }

            // User input
            Console.SetCursorPosition(cInputLeft, cTop++);
            string? title = ReadLineWithEscape();
            Console.SetCursorPosition(cInputLeft, cTop++);
            string? genre = ReadLineWithEscape();
            Console.CursorVisible = false;

            List<Type.Genre>? genreEnum = ParseGenre(genre);
            if (genreEnum == null)
            {
                ErrorMessage("Intet indtastet om genre.");
                return;
            }
            if (title == null)
            {
                ErrorMessage("Intet indtastet om titel.");
                return;
            }
            // Check if title already exists
            foreach (BoardGame item in _boardGameList.BoardGames)
            {
                if (item.Title == title)
                {
                    ErrorMessage("Brætspil med denne titel findes allerede.");
                    return;
                }
            }
            // Add board game
            boardGame = new BoardGame(title, genreEnum);
            _boardGameList.Add(boardGame);
            break;
        } while (true);
        AddBoardGameVariant(boardGame.Guid);
    }
    /// <summary>
    /// Prompts the user to add a new variant to a board game.
    /// </summary>
    /// <param name="boardGame">The board game to add a variant to.</param>
    /// <returns>The added board game variant.</returns>
    static void AddBoardGameVariant(Guid? guid = null)
    {
        int cTop;
        int cInputLeft = 27;
        string? variantTitle;
        string? numbersOfPlayers;
        BoardGame? boardGame;
        BoardGameVariant variant;

        if (guid == null)
            boardGame = MenuChooseBoardGame(false);
        else
            boardGame = _boardGameList.GetBoardGameById(guid.Value);
        if (boardGame != null)
        {
            Console.CursorVisible = true;
            HeadLine($"Tilføj variant til {boardGame.Title}");
            // Form
            cTop = Console.CursorTop;
            Console.Write("Variant");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
            Console.Write("Antal spillere (eks 1-4)");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
            // User input
            Console.SetCursorPosition(cInputLeft, cTop++);
            variantTitle = ReadLineWithEscape();
            Console.SetCursorPosition(cInputLeft, cTop++);
            numbersOfPlayers = ReadLineWithEscape();
            Console.CursorVisible = false;
            if (variantTitle == null)
            {
                ErrorMessage("Intet indtastet om variant.");
                return;
            }
            if (numbersOfPlayers == null)
            {
                ErrorMessage("Intet indtastet om antal spillere.");
                return;
            }

            variant = new BoardGameVariant(variantTitle, numbersOfPlayers, new ConditionList());

            _boardGameList.Add(variant, boardGame.Guid);
            ChangePriceAndQuantity(variant, boardGame.Guid);
            return;
        }
        Console.WriteLine("Ingen brætspil valgt. Tryk på en tast for at fortsætte...");
        Console.ReadKey();
        return;
    }
    static void ChangePriceAndQuantity(BoardGameVariant boardGameVariant, Guid guid)
    {
        int cTop;
        int cInputLeft = 21;
        string? price;
        string? quantity;
        Console.CursorVisible = true;
        // Headline
        HeadLine("Ændre pris og antal");
        // Form
        cTop = Console.CursorTop;
        for (int i = 0; i < boardGameVariant.ConditionList.Conditions.Count; i++)
        {
            Console.Write(boardGameVariant.ConditionList.Conditions.ElementAt(i).ConditionEnum.ToString());
            Console.Write(" - ");
            Console.Write("Pris");
            Console.CursorLeft = cInputLeft - 2;
            Console.Write(": ");
            Console.WriteLine(boardGameVariant.ConditionList.Conditions.ElementAt(i).Price.ToString());

            Console.Write("Antal");
            Console.CursorLeft = cInputLeft - 2;
            Console.WriteLine(":");
        }
        for (int i = 0; i < boardGameVariant.ConditionList.Conditions.Count; i++)
        {
            // User input
            Console.SetCursorPosition(cInputLeft, cTop++);
            price = ReadLineWithEscape();
            Console.SetCursorPosition(cInputLeft, cTop++);
            quantity = ReadLineWithEscape();
            if (decimal.TryParse(price, out decimal parsedPrice))
            {
                boardGameVariant.ConditionList.Conditions.ElementAt(i).Price = parsedPrice;
            }
            if (int.TryParse(quantity, out int parsedQuantity))
            {
                boardGameVariant.ConditionList.Conditions.ElementAt(i).Quantity = parsedQuantity;
            }
        }
        Console.CursorVisible = false;
    }
    /// <summary>
    /// Removes a board game.
    /// </summary>
    /// <param name="boardGame">The board game to remove.</param>
    static void RemoveBoardGame(BoardGame boardGame)
    {
        _boardGameList.Remove(boardGame);
    }
    /// <summary>
    /// Prompts the user to search for board games based on various criteria.
    /// </summary>
    /// <returns>A list of board games that match the search criteria.</returns>
    static void SearchBoardGame()
    {
        int cTop;
        int cInputLeft = 14;
        int i;
        string? title;
        string? genre;
        string? variant;
        string? condition;
        string? price;
        Console.CursorVisible = true;
        // Headline
        HeadLine("Søg efter brætspil");
        // Form
        cTop = Console.CursorTop;
        Console.Write("Title");
        Console.CursorLeft = cInputLeft - 2;
        Console.WriteLine(":");
        Console.Write("Genre");
        Console.CursorLeft = cInputLeft - 2;
        Console.WriteLine(":");
        Console.Write("Variants");
        Console.CursorLeft = cInputLeft - 2;
        Console.WriteLine(":");
        Console.Write("Condition");
        Console.CursorLeft = cInputLeft - 2;
        Console.WriteLine(":");
        for (i = 0; i < Enum.GetValues<Type.Condition>().Length; i++)
        {
            object? conditionValue = Enum.GetValues<Type.Condition>().GetValue(i);
            if (conditionValue != null)
            {
                Console.Write((int)conditionValue);
                Console.Write(" - ");
                Console.WriteLine(Enum.GetName(typeof(Type.Condition), i));
            }
        }
        Console.Write("Pris");
        Console.CursorLeft = cInputLeft - 2;
        Console.WriteLine(":");
        // User input
        Console.SetCursorPosition(cInputLeft, cTop++);
        title = ReadLineWithEscape();
        Console.SetCursorPosition(cInputLeft, cTop++);
        genre = ReadLineWithEscape();
        Console.SetCursorPosition(cInputLeft, cTop++);
        variant = ReadLineWithEscape();
        Console.SetCursorPosition(cInputLeft, cTop++);
        condition = ReadLineWithEscape();
        Console.SetCursorPosition(cInputLeft, cTop + i);
        price = ReadLineWithEscape();
        Console.CursorVisible = false;

        List<Type.Condition>? conditionEnum = ParseCondition(condition);
        List<Type.Genre>? genreEnum = ParseGenre(genre);

        ICollection<BoardGame> filteredBoardGames = _boardGameList.BoardGames.Where(x => x.Title != null).ToList();
        filteredBoardGames = _boardGameList.FilterByTitle(title, filteredBoardGames);
        filteredBoardGames = _boardGameList.FilterByGenre(genreEnum, filteredBoardGames);
        filteredBoardGames = _boardGameList.FilterByVariant(variant, filteredBoardGames);
        filteredBoardGames = _boardGameList.FilterByCondition(conditionEnum, filteredBoardGames);
        filteredBoardGames = _boardGameList.FilterByPrice(price, filteredBoardGames);

        ShowBoardGame(filteredBoardGames);
    }
    /// <summary>
    /// Displays a report of board games sorted by title.
    /// </summary>
    static void ShowReportGameList(List<BoardGame> boardGames)
    {
        HeadLine("List af brætspil");
        foreach (BoardGame boardGame in boardGames)
        {
            SubHeadLine(boardGame.Title);
            Console.WriteLine(boardGame.ToString());
            ShowBoardGameBody(boardGame);
        }
        Console.WriteLine("\nTryk på en tast for at fortsætte...");
        Console.ReadKey();
    }
    /// <summary>
    /// Displays a report of board games sorted by genre.
    /// </summary>
    static void ShowReportBoardGameSortTitle()
    {
        var sortedBoardGames = _boardGameList.BoardGames.OrderBy(bg => bg.Title).ToList();
        foreach (var boardGame in sortedBoardGames)
        {
            ShowBoardGame(boardGame);
        }
    }
    /// <summary>
    /// Displays a headline with the title and version of the program.
    /// </summary>
    /// <param name="headLine">The headline text to display.</param>
    static void ShowReportBoardGameSortGenre()
    {
        var sortedBoardGames = _boardGameList.BoardGames.OrderBy(bg => bg.Genre).ToList();
        foreach (var boardGame in sortedBoardGames)
        {
            ShowBoardGame(boardGame);
        }
    }
    /// <summary>
    /// Centers the given text within a specified width.
    /// </summary>
    /// <param name="text">The text to center.</param>
    /// <param name="width">The width within which to center the text.</param>
    /// <returns>The centered text with padding.</returns>
    static void HeadLine(string headLine)
    {
        Console.Clear();
        string title = $" {TITLE} version {GetVersion()} ";

        int l = Math.Max(title.Length, title.Length) + 1;
        Frame frame = new Frame(l, 2);
        frame.SetFrameText(title);
        frame.Render();
        Console.WriteLine();
        Console.WriteLine(CenterString(headLine, l));
        Console.WriteLine(new string('-', l + 1));
        Console.WriteLine();
    }
    static void SubHeadLine(string headLine)
    {
        Console.WriteLine(CenterString(headLine, 80));
        Console.WriteLine(new string('-', 80));
    }
    /// <summary>
    /// Centers the given text within a specified width.
    /// </summary>
    /// <param name="text">The text to center.</param>
    /// <param name="width">The width within which to center the text.</param>
    /// <returns>The centered text with padding.</returns>
    static string CenterString(string text, int width)
    {
        if (width <= text.Length)
        {
            return text; // Or throw an exception, or truncate the string
        }
        int padding = width - text.Length;
        int leftPadding = padding / 2;
        int rightPadding = padding - leftPadding;
        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
    }
    /// <summary>
    /// Reads a line of input from the console, with optional hiding of input characters.
    /// </summary>
    /// <param name="hideInput">Whether to hide the input characters (e.g., for passwords).</param>
    /// <returns>The input string, or null if the escape key was pressed.</returns>
    static string? ReadLineWithEscape(bool hideInput = false)
    {
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo keyInfo;
        while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                return null; // Return null if Esc is pressed
            }
            if (keyInfo.Key == ConsoleKey.Tab)
            {
                continue; // Ignore Tab key
            }
            if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (keyInfo.Key != ConsoleKey.Backspace)
            {
                input.Append(keyInfo.KeyChar);
                if (hideInput)
                {
                    Console.Write('*');
                }
                else
                {
                    Console.Write(keyInfo.KeyChar);
                }
            }
        }
        Console.WriteLine();
        return input.ToString();
    }
    /// <summary>
    /// Parses a string of conditions into a list of Condition enums.
    /// </summary>
    /// <param name="condition">The string of conditions to parse.</param>
    /// <returns>A list of Condition enums, or null if parsing failed.</returns>
    static List<Type.Condition>? ParseCondition(string? condition)
    {
        List<Type.Condition> list = new List<Type.Condition>();
        if (condition == null)
        {
            return null;
        }
        string[] conditionArray = condition.Split(" ");

        foreach (string conditionString in conditionArray)
        {
            // Try to parse as integer
            if (int.TryParse(conditionString, out int conditionInt))
            {
                if (Enum.IsDefined(typeof(Type.Condition), conditionInt))
                {
                    list.Add((Type.Condition)conditionInt);
                }
            }
            else if (Enum.TryParse(conditionString, true, out Type.Condition conditionEnum))
            {
                list.Add(conditionEnum);
            }
        }
        if (list.Count > 0)
            return list;
        return null;
    }
    /// <summary>
    /// Parses a string of genres into a list of Genre enums.
    /// </summary>
    /// <param name="gerne">The string of genres to parse.</param>
    /// <returns>A list of Genre enums, or null if parsing failed.</returns>
    static List<Type.Genre>? ParseGenre(string? gerne)
    {
        List<Type.Genre> list = new List<Type.Genre>();
        if (gerne == null)
        {
            return null;
        }

        string[] genreArray = gerne.Split(" ");

        foreach (string item in genreArray)
        {
            // Try to parse as integer
            if (int.TryParse(item, out int gerneInt))
            {
                if (Enum.IsDefined(typeof(Type.Genre), gerneInt))
                {
                    list.Add((Type.Genre)gerneInt);
                }
            }
            else
            {
                // Try to parse as string
                if (Enum.TryParse(item, true, out Type.Genre genreEnum))
                {
                    list.Add(genreEnum);
                }
            }
        }

        if (list.Count > 0)
            return list;
        return null;
    }
    /// <summary>
    /// Displays an error message in red text.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    static void ErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine("Tryk på en tast for at fortsætte...");
        Console.ReadKey();
    }

    #region menu
    /// <summary>
    /// Main menu
    /// (Action) is a delegate to a method with no parameters and no return value.
    /// When calling PaginateMenu() the menu is displayed and the user can select an item.
    /// Item with it action is returned.
    /// Then we execute the action.
    /// </summary>
    static void MenuMain()
    {
        do
        {
            Console.Clear();
            HeadLine("Hoved menu");
            // Create a list of menu items
            List<MenuItem> menuItems = new();
            menuItems.Add(new MenuItem("Brætspil", MenuBoardGame));
            menuItems.Add(new MenuItem("Kunde", MenuCostumer));
            menuItems.Add(new MenuItem("Rapporter", MenuReport));
            if (_auth.GetRole(_auth.User) == Type.Role.Admin)
                menuItems.Add(new MenuItem("Admin", MenuAdmin));
            menuItems.Add(new MenuItem("Logout", Logout));
            // Create a menu paginator
            MenuPaginator menu = new(menuItems, 10, true);
            if (menu.menuItem != null && menu.menuItem.Action is Action action)
                action(); // Execute the action
            else
                return;
        } while (true);
    }
    /// <summary>
    /// Customer menu.
    /// </summary>
    static void MenuCostumer()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Report menu
    /// (Action) is a delegate to a method with no parameters and no return value.
    /// When calling PaginateMenu() the menu is displayed and the user can select an item.
    /// Item with it action is returned.
    /// Then we execute the action.
    /// </summary>
    static void MenuReport()
    {
        do
        {
            Console.Clear();
            HeadLine("Rapport");
            List<MenuItem> menuItems = new();
            menuItems.Add(new MenuItem("Lagerstatus sorteret på titel", ShowReportBoardGameSortTitle));
            menuItems.Add(new MenuItem("Lagerstatus sorteret på gerne", ShowReportBoardGameSortGenre));
            MenuPaginator menu = new(menuItems, 10);
            if (menu.menuItem != null && menu.menuItem.Action is Action action)
                action();
            else
                return;
        } while (true);
    }
    /// <summary>
    /// Admin menu.
    /// </summary>
    static void MenuAdmin()
    {
        do
        {
            Console.Clear();
            HeadLine("Administrator");
            List<MenuItem> menuItems = new();
            throw new NotImplementedException();
        } while (true);
    }
    /// <summary>
    /// Board game menu
    /// (Action) is a delegate to a method with no parameters and no return value.
    /// When calling PaginateMenu() the menu is displayed and the user can select an item.
    /// Item with it action is returned.
    /// Then we execute the action.
    /// </summary>
    static void MenuBoardGame()
    {
        do
        {
            Console.Clear();
            HeadLine("Brætspil menu");
            List<MenuItem> menuItems = new();
            menuItems.Add(new MenuItem("Vælg spil", () => MenuChooseBoardGame()));
            menuItems.Add(new MenuItem("Tilføj spil", AddBoardGame));
            menuItems.Add(new MenuItem("Tilføj variant", () => AddBoardGameVariant()));
            menuItems.Add(new MenuItem("Søg", new Action(() => SearchBoardGame())));
            MenuPaginator menu = new(menuItems, 10);
            if (menu.menuItem != null && menu.menuItem.Action is Action action)
            {
                action();
            }
            else
            {
                return;
            }
        } while (true);
    }
    /// <summary>
    /// Choose board game menu
    /// (Action) is a delegate to a method with no parameters and no return value.
    /// When calling PaginateMenu() the menu is displayed and the user can select an item.
    /// Item with it action is returned.
    /// Then we execute the action.
    /// </summary>
    static BoardGame? MenuChooseBoardGame(bool ShowVariants = true)
    {
        BoardGame? result = null;
        string prefix = "";
        Console.Clear();
        HeadLine("Vælg spil");
        List<MenuItem> menuItems = new();
        foreach (BoardGame boardGame in _boardGameList.BoardGames.OrderBy(bg => bg.Title).ToList())
        {
            if (ShowVariants)
            {
                foreach (BoardGameVariant boardGameVariant in boardGame.Variants)
                {
                    if (boardGameVariant.Title == "")
                        prefix = "";
                    else
                        prefix = " : ";
                    menuItems.Add(new MenuItem(boardGame.Title + prefix + boardGameVariant.Title, (() => ShowBoardGameVariant(boardGame, boardGameVariant))));
                }
            }
            else
            {
                menuItems.Add(new MenuItem(boardGame.Title, (() => result = ShowBoardGame(boardGame))));
            }
        }
        MenuPaginator menu = new(menuItems, 10);
        if (menu.menuItem != null && menu.menuItem.Action is Action action)
        {
            action();
            return result;
        }
        else
        {
            return null;
        }
    }
    #endregion menu

    /// <summary>
    /// Main method of the program.
    /// Loads data from a JSON file, displays the main menu, and exports data back to the JSON file.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        JsonFileHandler.Instance.ImportData(DATA_JSON_FILE);
        //Login();
        MenuMain();
        JsonFileHandler.Instance.ExportData(DATA_JSON_FILE);
    }
}
