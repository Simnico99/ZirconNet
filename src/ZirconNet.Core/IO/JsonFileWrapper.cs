// <copyright file="JsonFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Text.Json;
using System.Text.Json.Nodes;

namespace ZirconNet.Core.IO;

/// <summary>
/// Should be disposed after use.
/// </summary>
public sealed class JsonFileWrapper : FileWrapperBase<JsonFileWrapper>
{
    private readonly JsonObject _fileContent;

    public JsonFileWrapper(string file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
        var reader = File.OpenRead(FullName);
        if (reader.Length == 0)
        {
            var newDocument = JsonDocument.Parse("{}");
            _fileContent = newDocument.Deserialize<JsonObject>()!;
            return;
        }

        var document = JsonDocument.Parse(reader);
        _fileContent = document.Deserialize<JsonObject>()!;
    }

    public JsonFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
        var reader = File.OpenRead(FullName);
        if (reader.Length == 0)
        {
            var newDocument = JsonDocument.Parse("{}");
            _fileContent = newDocument.Deserialize<JsonObject>()!;
            return;
        }

        var document = JsonDocument.Parse(reader);
        _fileContent = document.Deserialize<JsonObject>()!;
    }

    public T? ReadKey<T>(string key)
    {
        return _fileContent[key].Deserialize<T>();
    }

    public bool TryReadKey<T>(string key, out T? result)
    {
        result = default;
        if (_fileContent.TryGetPropertyValue(key, out var element))
        {
            result = element.Deserialize<T>();
            return true;
        }

        return false;
    }

    public void ModifyKey<T>(string key, T newValue)
    {
        if (_fileContent.ContainsKey(key))
        {
            _fileContent[key] = JsonNode.Parse(JsonSerializer.Serialize(newValue));
        }
        else
        {
            throw new KeyNotFoundException($"The key '{key}' was not found in the JSON file.");
        }
    }

    public void AddKey<T>(string key, T value)
    {
        _fileContent[key] = JsonNode.Parse(JsonSerializer.Serialize(value));
    }

    public void DeleteKey(string key)
    {
        _fileContent.Remove(key);
    }

    public void Flush()
    {
        using var stream = new FileStream(FullName, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        _fileContent.WriteTo(writer);
        writer.Flush();
    }

#if NET6_0_OR_GREATER
    public Task FlushAsync()
    {
        using var stream = new FileStream(FullName, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        _fileContent.WriteTo(writer);
        return writer.FlushAsync();
    }
#endif
}