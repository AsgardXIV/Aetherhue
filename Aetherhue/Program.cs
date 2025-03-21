using Aetherhue.Tools;
using Serilog;
using Spectre.Console;
using Serilog.Sinks.Spectre;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
    static int Main(string[] args)
    {
        // Wait for input at the end?
        bool waitAtEnd = true;

        // Create logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Spectre()
            .CreateLogger();

        // Show Logo
        AnsiConsole.Write(new FigletText("Aetherhue").Color(Spectre.Console.Color.Purple));

        // Image path
        var rawImagePath = args.Length > 0 ? args[0] : AnsiConsole.Prompt(new TextPrompt<string>("Path to file?"));
        var imagePath = Utils.SanitizePath(rawImagePath);


        if (!File.Exists(imagePath))
        {
            AnsiConsole.MarkupLine($"[red]File not found: {imagePath}[/]");
            if(waitAtEnd) Console.ReadLine();
            return 1;
        }


        // Load image
        Image<Rgba32> image = null!;
        try
        {
            image = Utils.PathToImage(imagePath);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error loading image");
            AnsiConsole.MarkupLine($"[red]Error loading image: {e.Message}[/]");
            if (waitAtEnd) Console.ReadLine();
            return 2;
        }

        // Action
        var actionVerb = AnsiConsole.Prompt(
            new SelectionPrompt<Actions.Action>()
                .Title("What are you trying to do?")
                .PageSize(10)
                .UseConverter(action => Actions.ActionMap[action])
                .AddChoices(Enum.GetValues<Actions.Action>())
        );

        // Execute
        try
        {
            Actions.ExecuteAction(actionVerb, imagePath, image);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error executing action");
            AnsiConsole.MarkupLine($"[red]Error executing action: {e.Message}[/]");
            if (waitAtEnd) Console.ReadLine();
            return 3;
        }

        // Cleanup
        image.Dispose();

        // Success
        AnsiConsole.MarkupLine($"[green]Action complete.[/]");
        if (waitAtEnd) Console.ReadLine();

        return 0;
    }
}

