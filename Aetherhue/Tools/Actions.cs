using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Spectre.Console;
using Color = SixLabors.ImageSharp.Color;

namespace Aetherhue.Tools;

static class Actions
{
    public enum Action
    {
        IDMapToXIVIDMap,
        OverlayImages,
        SeparateDiffuseAndAlpha,
        ExtendUVIslands
    }

    public static readonly Dictionary<Action, String> ActionMap = new()
    {
        { Action.IDMapToXIVIDMap, "ID Map to XIV ID Map" },
        { Action.OverlayImages, "Overlay Images" },
        { Action.SeparateDiffuseAndAlpha, "Separate Diffuse and Alpha" },
        { Action.ExtendUVIslands, "Extend UV Islands" }
    };

    public static void ExecuteAction(Action action, String imagePath, Image<Rgba32> image)
    {
        switch (action)
        {
            case Action.IDMapToXIVIDMap:
                IDMapToXIVIDMap(imagePath, image);
                break;

            case Action.OverlayImages:
                OverlayImages(imagePath, image);
                break;

            case Action.SeparateDiffuseAndAlpha:
                SeparateDiffuseAndAlpha(imagePath, image);
                break;

            case Action.ExtendUVIslands:
                ExtendUVIslands(imagePath, image);
                break;
        }
    }

    static void IDMapToXIVIDMap(String imagePath, Image<Rgba32> image)
    {      
        // Row offset
        int rowOffset = AnsiConsole.Prompt(new TextPrompt<int>("Row Offset?")
            .DefaultValue(2)
            .Validate(value =>
            {
                if(value >= 0 && value < Framework.ImageUtils.XIVUtils.XIVColorRowCount)
                {
                    return ValidationResult.Success();
                }
                return ValidationResult.Error($"[red]Row offset must be between 0 and {Framework.ImageUtils.XIVUtils.XIVColorRowCount-1}.[/]");
            }));

        // Row pairs
        bool bPackRowPairs = AnsiConsole.Prompt(new ConfirmationPrompt("Pack using row pairs?"));

        // Ignore color
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


        // Execute
        AnsiConsole.MarkupLine($"Converting ID map to XIV ID map...");
        var (xivIdMap, penumbraColorSet) = Framework.ImageUtils.IDMapToXIVIDMap.Execute(image, rowOffset, bPackRowPairs, color);
        
        // Generate side maps
        var redOnly = xivIdMap.Clone();
        var greenOnly = xivIdMap.Clone();

        for(int y = 0; y < xivIdMap.Height; y++)
        {
            for (int x = 0; x < xivIdMap.Width; x++)
            {
                var pixel = xivIdMap[x, y];
                redOnly[x, y] = new Rgba32(pixel.R, pixel.R, pixel.R, 255);
                greenOnly[x, y] = new Rgba32(pixel.G, pixel.G, pixel.G, 255);
            }
        }

        // Save
        var folderPath = Path.GetDirectoryName(imagePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
        
        var xivIDMapFileName = fileWithoutExtension + "_xivid_map.png";
        xivIdMap.SaveAsPng(Path.Join(folderPath, xivIDMapFileName));

        var redMapFileNap = fileWithoutExtension + "_xivid_map_r.png";
        redOnly.SaveAsPng(Path.Join(folderPath, redMapFileNap));

        var greenMapFileName = fileWithoutExtension + "_xivid_map_g.png";
        greenOnly.SaveAsPng(Path.Join(folderPath, greenMapFileName));

        var penumbraColorSetFileName = fileWithoutExtension + "_penumbra_colorset.txt";
        File.WriteAllText(Path.Join(folderPath, penumbraColorSetFileName), Convert.ToBase64String(penumbraColorSet));

        // Cleanup
        xivIdMap.Dispose();
        redOnly.Dispose();
        greenOnly.Dispose();
        AnsiConsole.MarkupLine($"XIV ID map written to {xivIDMapFileName}");
    }

    static void OverlayImages(String imagePath, Image<Rgba32> image)
    {
        bool bShouldLoop = true;
        List<Image<Rgba32>> overlayImages = [];

        do
        {
            // Overlay image
            String rawOverlayImagePath = AnsiConsole.Prompt(new TextPrompt<String>("Overlay Image Path?").Validate(value => { 
                var newPath = Utils.SanitizePath(value);
                if (File.Exists(newPath))
                {
                    return ValidationResult.Success();
                }
                return ValidationResult.Error("[red]File not found.[/]");
            }));
            var overlayImagePath = Utils.SanitizePath(rawOverlayImagePath);
            var overlayImage = Utils.PathToImage(overlayImagePath);

            if(overlayImage.Size != image.Size)
            {
                AnsiConsole.MarkupLine("[yellow]Overlay image does not have same size as the base image.[/]");
                if(AnsiConsole.Prompt(new ConfirmationPrompt("Automatically resize?")))
                {
                    overlayImage.Mutate(x => x.Resize(image.Width, image.Height, new NearestNeighborResampler()));
                }
                else
                {
                    overlayImage.Dispose();
                    break;
                }
            }

            overlayImages.Add(overlayImage);

            // Add another overlay?
            bShouldLoop = AnsiConsole.Prompt(new ConfirmationPrompt("Add another overlay?"));

        } while (bShouldLoop);

        // Execute
        AnsiConsole.MarkupLine($"Overlaying images...");
        var newImage = Framework.ImageUtils.OverlayImages.Execute(image, [.. overlayImages]);

        // Save
        var folderPath = Path.GetDirectoryName(imagePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
        var overlayFileName = fileWithoutExtension + "_overlay.png";
        newImage.SaveAsPng(Path.Join(folderPath, overlayFileName));

        // Cleanup
        newImage.Dispose();
        foreach (var overlayImage in overlayImages)
        {
            overlayImage.Dispose();
        }

        AnsiConsole.MarkupLine($"Overlay written to {overlayFileName}");
    }

    static void SeparateDiffuseAndAlpha(String imagePath, Image<Rgba32> image)
    {
        AnsiConsole.MarkupLine($"Separating diffuse and alpha...");

        // Execute
        var (diffuse, alpha) = Framework.ImageUtils.SeparateDiffuseAndAlpha.Execute(image);

        // Save
        var folderPath = Path.GetDirectoryName(imagePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);

        var diffuseFileName = fileWithoutExtension + "_diffuse.png";
        var alphaFileName = fileWithoutExtension + "_alpha.png";

        diffuse.SaveAsPng(Path.Join(folderPath, diffuseFileName));
        alpha.SaveAsPng(Path.Join(folderPath, alphaFileName));

        // Cleanup
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

        // Execute
        AnsiConsole.MarkupLine($"Extending UV islands...");

        var newImage = Framework.ImageUtils.ExtendUVIslands.Execute(image, margin, color, ignoreAlpha);

        // Save
        var folderPath = Path.GetDirectoryName(imagePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
        var extendedFileName = fileWithoutExtension + "_extended.png";

        newImage.SaveAsPng(Path.Join(folderPath, extendedFileName));

        // Cleanup
        newImage.Dispose();

        AnsiConsole.MarkupLine($"Extended image written to {extendedFileName}");
    }
}
