using System.Reflection;

namespace Aetherhue.Framework.Utils;

internal static class ResourceUtils
{
    public static byte[] GetBytes(string name)
    {
        using var stream = GetRawResourceStream(name);
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        return bytes;
    }

    public static Stream GetRawResourceStream(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Aetherhue.Framework.Resources.{name}";
        var stream = assembly.GetManifestResourceStream(resourceName);
        return stream ?? throw new Exception($"Resource {name} not found.");
    }
}
