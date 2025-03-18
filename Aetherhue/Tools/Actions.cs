using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console;
using Color = SixLabors.ImageSharp.Color;

namespace Aetherhue.Tools;

static class Actions
{
    public enum Action
    {
        SeparateDiffuseAndAlpha,
        ExtendUVIslands
    }

    public static readonly Dictionary<Action, String> ActionMap = new()
    {
        { Action.SeparateDiffuseAndAlpha, "Separate Diffuse and Alpha" },
        { Action.ExtendUVIslands, "Extend UV Islands" }
    };

    public static void ExecuteAction(Action action, String imagePath, Image<Rgba32> image)
    {
        switch (action)
        {
            case Action.SeparateDiffuseAndAlpha:
                SeparateDiffuseAndAlpha(imagePath, image);
                break;

            case Action.ExtendUVIslands:
                ExtendUVIslands(imagePath, image);
                break;
        }
    }

    static void SeparateDiffuseAndAlpha(String imagePath, Image<Rgba32> image)
    {
        AnsiConsole.MarkupLine($"Separating diffuse and alpha...");

        var (diffuse, alpha) = Framework.ImageUtils.SeparateDiffuseAndAlpha.Execute(image);

        var folderPath = Path.GetDirectoryName(imagePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);

        var diffuseFileName = fileWithoutExtension + "_diffuse.png";
        var alphaFileName = fileWithoutExtension + "_alpha.png";

        diffuse.SaveAsPng(Path.Join(folderPath, diffuseFileName));
        alpha.SaveAsPng(Path.Join(folderPath, alphaFileName));

        diffuse.Dispose();
        alpha.Dispose();

        AnsiConsole.MarkupLine($"Diffuse written to {diffuseFileName} and alpha to {alphaFileName}");
    }

    static void ExtendUVIslands(String imagePath, Image<Rgba32> image)
    {
        // Margin
        int margin = AnsiConsole.Prompt(new TextPrompt<int>("UV Island Margin?").DefaultValue(4));

        // Ignore alpha
        bool ignoreAlpha = AnsiConsole.Prompt(new ConfirmationPrompt("Ignore alpha?"));

        // Neutral color
        String inputColor = AnsiConsole.Prompt(new TextPrompt<String>("Background color?")
            .DefaultValue("#000000")
            .Validate(static value =>
            {
                if (Framework.ImageUtils.ImageHelpers.TryParseColor(out _, value))
                {
                    return ValidationResult.Success();
                }

                return ValidationResult.Error("[red]Invalid color format. Please use RGB (e.g., 255,0,0) or hex (e.g., #FFFFFF).[/]");
            }
        ));
        Framework.ImageUtils.ImageHelpers.TryParseColor(out Color color, inputColor);


        AnsiConsole.MarkupLine($"Extending UV islands...");

        var newImage = Framework.ImageUtils.ExtendUVIslands.Execute(image, margin, color, ignoreAlpha);

        var folderPath = Path.GetDirectoryName(imagePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
        var extendedFileName = fileWithoutExtension + "_extended.png";

        newImage.SaveAsPng(Path.Join(folderPath, extendedFileName));

        newImage.Dispose();

        AnsiConsole.MarkupLine($"Extended image written to {extendedFileName}");
    }
}
