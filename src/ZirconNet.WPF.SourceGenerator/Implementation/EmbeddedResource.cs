﻿using System.Reflection;

namespace ZirconNet.WPF.SourceGenerator.Implementation;
internal static class EmbeddedResource
{
    public static string GetContent(string relativePath)
    {
        var baseName = Assembly.GetExecutingAssembly().GetName().Name;
        var resourceName = relativePath
            .TrimStart('.')
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');

        var manifestResourceName = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceNames()
            .FirstOrDefault(x => x!.EndsWith(resourceName, StringComparison.InvariantCulture));

        if (string.IsNullOrEmpty(manifestResourceName))
            throw new InvalidOperationException(
                $"Did not find required resource ending in '{resourceName}' in assembly '{baseName}'."
            );

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResourceName) ?? throw new InvalidOperationException($"Did not find required resource '{manifestResourceName}' in assembly '{baseName}'.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}