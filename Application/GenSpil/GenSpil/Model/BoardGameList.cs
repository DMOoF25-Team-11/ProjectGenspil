using GenSpil.Type;

//TODO: All interactions with the user should be handled in the UI layer, not in the model.

namespace GenSpil.Model;

/// <summary>
/// Singleton class for handling a list of board games.
/// TODO Should not interact with the user directly. (Tirsvad)
/// </summary>
class BoardGameList
{
    private static BoardGameList? _instance;
    private static readonly object _lock = new object();
    public static BoardGameList Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new BoardGameList();
                }
                return _instance;
            }
        }
    } ///> Singleton instance of the BoardGameList


    public List<BoardGame> _boardGames = new List<BoardGame>();

    
    /// <summary>
    /// Tilføjer et brætspil.
    /// </summary>
    public void AddBoardGame()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Titel: ");
        Console.ResetColor();
        string title = Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Variant: ");
        Console.ResetColor();
        string variant = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Genre ({string.Join(" / ", Enum.GetNames(typeof(Genre)))}): ");
        Console.ResetColor();

        string genreInput = Console.ReadLine();

        // Forsøg at konvertere genre-input til Genre enum (ignorerer store/små bogstaver)
        bool genreValid = Enum.TryParse(genreInput, true, out Genre genre);
        if (!genreValid)
        {
            Console.WriteLine("Ugyldig genre angivet. Handlingen afbrydes.");
            return;
        }


        // Viser de mulige tilstands-valg
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Tilstand ({string.Join(" / ", Enum.GetNames(typeof(Type.Condition)))}): ");
        Console.ResetColor();
        string conditionInput = Console.ReadLine();

        // Forsøg at konvertere tilstands-input til ConditionEnum (ignorerer store/små bogstaver)
        bool conditionValid = Enum.TryParse(conditionInput, true, out Type.Condition conditionEnum);
        if (!conditionValid)
        {
            Console.WriteLine("Ugyldig tilstand angivet. Handlingen afbrydes.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Antal: ");
        Console.ResetColor();

        string quantityInput = Console.ReadLine();

        // Forsøg at konvertere antal-input til et heltal (int)
        bool quantityValid = int.TryParse(quantityInput, out int quantity);
        if (!quantityValid || quantity < 0) // Antal skal være et positivt heltal
        {
            Console.WriteLine("Ugyldigt antal angivet (skal være et positivt heltal). Handlingen afbrydes.");
            return;
        }

        // --- Price Input ---
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Pris: ");
        Console.ResetColor();
        string priceInput = Console.ReadLine();

        // Forsøg at konvertere pris-input til et decimaltal
        // Bemærk: TryParse bruger computerens lokale indstillinger for decimaltegn (typisk komma på dansk)
        bool priceValid = decimal.TryParse(priceInput, out decimal price);
        if (!priceValid || price < 0) // Pris skal være et positivt tal
        {
            Console.WriteLine("Ugyldig pris angivet (skal være et positivt tal). Handlingen afbrydes.");
            return; // Gå ud af switch-casen
        }


        // Opret variant-objekt (bruger stadig titel som en del af varianten)
        BoardGameVariant bgVariant = new BoardGameVariant(title, variant);

        // Opret condition-objekt med de indtastede værdier
        Condition bgCondition = new Condition(conditionEnum, quantity, price);

        // Opret det nye brætspil med alle data. + 1 pga. ny linje
        BoardGame newGame = new BoardGame(_boardGames.Count + 1, title, bgVariant, genre, bgCondition);

        // Tilføj spillet til listen
        _boardGames.Add(newGame);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Brætspil tilføjet!");
        Console.ResetColor();

        Console.ReadLine();
        return;

    }

    /// <summary>
    /// Vis brætspil
    /// </summary>
    public void DisplayBoardGames()
    {
        Console.Clear(); // Ryd skærm før visning
        Console.WriteLine("--- Liste over Brætspil (Grupperet) ---");
        if (_boardGames.Count == 0)
        {
            Console.WriteLine("Der er ingen brætspil i listen endnu.");
            return; // Gå tilbage hvis listen er tom
        }

        // Gruppér listen efter spillets Titel property
        // OrderBy(g => g.Title) sorterer titlerne alfabetisk (valgfrit)
        var groupedGames = _boardGames
                            .OrderBy(g => g.Title)
                            .GroupBy(g => g.Title);

        foreach (var group in groupedGames)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            // Skriv titlen én gang for gruppen
            Console.WriteLine($"\n{group.Key}:"); // group.Key indeholder titlen

            // Gennemgå hvert spil inden for denne titel-gruppe
            // OrderBy(g => g.Variant.Variant) sorterer varianterne under titlen (valgfrit)
            foreach (var game in group.OrderBy(g => g.Variant.Variant))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                // Skriv detaljerne for dette specifikke spil (variant, genre, tilstand)               
                Console.WriteLine($"  - Variant: {game.Variant.Variant}, Genre: {game.Genre}, Tilstand: {game.Condition}");
                Console.ResetColor();
            }
        }
        Console.WriteLine("\n------------------------------------");
        Console.ReadLine();
    }

    /// <summary>
    /// Søg efter brætspil
    /// </summary>
    public List<BoardGame> SearchBoardGames() //SearchBoardGames() should take parameters (Tirsvad)
    {
        Console.Write("Indtast titel - eller del af den: ");
        string searchTitle = Console.ReadLine();

        var foundGames = _boardGames
                            .Where(g => g.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase)) //Filtrer elementer i en liste basseret på en betingelse
                            .OrderBy(g => g.Title) //Sortere en liste efter en bestemt egenskab.
                            .GroupBy(g => g.Title); //Grupperer en liste efter en bestemt egenskab

        if (foundGames.Any()) // Tjekker om der er fundne resultater
        {
            Console.WriteLine($"--- Fundne Brætspil (Søgning: '{searchTitle}') ---");

            foreach (var group in foundGames)
            {
                Console.WriteLine($"\n{group.Key}:"); // Titel Bliver skrevet øverst

                foreach (var game in group.OrderBy(g => g.Variant.Variant)) // Kigger videre ud fra titlen, og sortere efter dens varianter
                {
                    Console.WriteLine($"  - Variant: {game.Variant.Variant}, Genre: {game.Genre}, Tilstand: {game.Condition}");
                }
            }
            Console.WriteLine("----------------------------------------------");
        }
        else
        {
            Console.WriteLine($"Ingen brætspil med '{searchTitle}' fundet.");
        }

        Console.ReadLine();
        return foundGames.SelectMany(g => g).ToList(); // Konverterer grupperingen tilbage til en flad liste
    }

    /// <summary>
    /// Fjern brætspil helt.
    /// </summary>
    public void RemoveBoardGame()
    {
        Console.Clear();
        Console.WriteLine("--- Fjern Brætspil ---");

        if (_boardGames.Count == 0)
        {
            Console.WriteLine("Listen er tom. Der er ingen spil at fjerne.");
            return; // Gå tilbage, da der ikke er noget at gøre
        }

        Console.WriteLine("Vælg nummeret på det spil, du vil fjerne:");

        // Vis alle spil med et nummer (startende fra 1 (Den starter normalt med 0)
        for (int i = 0; i < _boardGames.Count; i++)
        {
            // Brug i + 1 for at vise 1-baseret nummerering til brugeren
            Console.WriteLine($"{i + 1}. {_boardGames[i]}"); // Bruger BoardGame.ToString()
        }
        Console.WriteLine("-------------------------");
        Console.Write("Indtast nummer på spil der skal fjernes (eller 0 for at annullere): ");

        string input = Console.ReadLine();

        // Prøv at konvertere input til et tal
        if (int.TryParse(input, out int choice))
        {
            if (choice == 0)
            {
                Console.WriteLine("Fjernelse annulleret.");
                return; // Gå tilbage
            }
            // Tjek om nummeret er inden for det gyldige område (1 til antal spil)
            if (choice >= 1 && choice <= _boardGames.Count)
            {
                // Konverter brugerens 1-baserede valg til 0-baseret index
                int indexToRemove = choice - 1;

                // Få fat i spillet der skal fjernes for at vise det i beskeden
                BoardGame gameToRemove = _boardGames[indexToRemove];

                // Valgfri: Spørg om bekræftelse
                Console.WriteLine($"\nDu er ved at fjerne: {gameToRemove}");
                Console.Write("Er du sikker? (J/N): ");
                string confirmation = Console.ReadLine();

                if (confirmation != null && confirmation.Trim().Equals("J", StringComparison.OrdinalIgnoreCase))
                {
                    // Fjern spillet fra listen ved det valgte index
                    _boardGames.RemoveAt(indexToRemove);
                    Console.WriteLine($"'{gameToRemove.Title}' er blevet fjernet.");
                }
                else
                {
                    Console.WriteLine("Fjernelse annulleret.");
                }
            }
            else
            {
                // Brugeren indtastede et tal, men det var uden for listen
                Console.WriteLine("Ugyldigt nummer. Indtast venligst et nummer fra listen.");
            }
        }
        else
        {
            // Brugeren indtastede ikke et gyldigt tal
            Console.WriteLine("Ugyldigt input. Indtast venligst et tal.");
        }
        Console.ReadLine();
    }
  
    /// <summary>
    /// Metode til at redigere et brætspil baseret på titel.
    /// Brugeren indtaster titlen på spillet, vælger det korrekte spil, og kan derefter redigere tilstand, antal og pris.
    /// Hvis antallet sættes til 0, fjernes spillet automatisk fra listen.
    /// </summary>
    public void EditBoardGame()
    {
        Console.Write("Indtast titel på spillet, du vil redigere: ");
        string searchTitle = Console.ReadLine();

        // Find alle spil, der matcher titlen 
        var foundGames = _boardGames
                            .Where(g => g.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase)) // Søger efte
                            .OrderBy(g => g.Title) // Sorterer fundne spil alfabetisk ud fra Variant.
                            .ToList();

        if (foundGames.Count == 0) // Hvis ingen spil matcher titlen
        {
            Console.WriteLine($"Ingen spil fundet med titlen '{searchTitle}'.");
            Console.ReadLine();
            return; // Stopper metoden
        }

        // Viser fundne spil med deres detaljer
        Console.WriteLine("\n--- Fundne spil ---");
        for (int i = 0; i < foundGames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {foundGames[i].Title} - Variant: {foundGames[i].Variant.Variant}, Genre: {foundGames[i].Genre}, " +
                              $"Tilstand: {foundGames[i].Condition.ConditionType}, Antal: {foundGames[i].Condition.Quantity}, Pris: {foundGames[i].Condition.Price} kr.");
        }

        // Brugeren vælger hvilket spil der skal redigeres
        Console.Write("\nIndtast nummeret på det spil, du vil redigere: ");
        if (!int.TryParse(Console.ReadLine(), out int gameIndex) || gameIndex < 1 || gameIndex > foundGames.Count) // out int  gameIndex = variablen først er gældende ved tildeling af resultat.
        {
            Console.WriteLine("Ugyldigt valg.");
            Console.ReadLine();
            return;
        }

        BoardGame selectedGame = foundGames[gameIndex - 1]; // Henter det valgte spil. - 1 pga. at den nummeringen starter med 0, med vises med 1.
        Console.WriteLine($"\nRedigerer: {selectedGame.Title} - {selectedGame.Variant.Variant}");

        // Brugeren kan vælge at ændre tilstand
        Console.Write($"Ny tilstand (tryk Enter for at beholde den nuværende: {selectedGame.Condition.ConditionType}): ");
        string newCondition = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newCondition) && Enum.TryParse(newCondition, true, out Type.Condition condition)) // String blank = ingen ændring. Ellers sker ændring.
            selectedGame.Condition.ConditionType = condition; // Opdaterer tilstanden

        // Brugeren kan vælge at ændre antal
        Console.Write($"Nyt antal (tryk Enter for at beholde det nuværende: {selectedGame.Condition.Quantity}): ");
        string newQuantity = Console.ReadLine();
        if (int.TryParse(newQuantity, out int quantity) && quantity >= 0)
            selectedGame.Condition.Quantity = quantity;

        // Brugeren kan vælge at ændre pris
        Console.Write($"Ny pris (tryk Enter for at beholde den nuværende: {selectedGame.Condition.Price} kr.): ");
        string newPrice = Console.ReadLine();
        if (decimal.TryParse(newPrice, out decimal price) && price >= 0) // Hvis værdien ikke er højere eller lig 0. Ingen værdi.
            selectedGame.Condition.Price = price;

        // Hvis antallet nu er 0, fjernes spillet fra listen
        if (selectedGame.Condition.Quantity == 0)
        {
            _boardGames.Remove(selectedGame);
            Console.WriteLine($"'{selectedGame.Title}' er blevet fjernet, da antallet er 0.");
        }
        else
        {
            // Bekræfter at spillet er opdateret
            Console.WriteLine("\nSpillet er opdateret!");
            Console.WriteLine($"{selectedGame.Title} - Variant: {selectedGame.Variant.Variant}, Genre: {selectedGame.Genre}, " +
                              $"Tilstand: {selectedGame.Condition.ConditionType}, Antal: {selectedGame.Condition.Quantity}, Pris: {selectedGame.Condition.Price} kr.");
        }

        Console.ReadLine(); // Forhindrer konsollen i at lukke med det samme
    }
}
