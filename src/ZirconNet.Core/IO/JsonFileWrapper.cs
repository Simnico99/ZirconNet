using System.Text.Json.Nodes;
using System.Text.Json;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public sealed class JsonFileWrapper : FileWrapperBase
{
    private JsonObject? _fileContent;
    private static readonly LockAsync _asyncLock = new();

    public JsonFileWrapper(string file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
    public JsonFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }

    private void AddKey(string fieldToAdd, object value)
    {
        if (_fileContent is null)
        {
            return;
        }

        lock (_fileContent)
        {
            if (_fileContent?[fieldToAdd] is null)
            {
                _fileContent = _fileContent is not null ? _fileContent : JsonNode.Parse("{  }")?.AsObject();

                _fileContent?.Add(fieldToAdd, value.ToString());
            }
        }
    }

    public void DeleteKey(string fieldToDelete)
    {
        if (_fileContent is null)
        {
            return;
        }

        lock (_fileContent)
        {
            if (_fileContent?[fieldToDelete] is not null)
            {
                _fileContent = _fileContent is not null ? _fileContent : JsonNode.Parse("{  }")?.AsObject();

                _fileContent?.Remove(fieldToDelete);
            }
        }
    }

    public void ModifyKey(string fieldToModify, object value)
    {
        if (_fileContent is null)
        {
            return;
        }

        lock (_fileContent)
        {
            if (_fileContent?[fieldToModify] is not null)
            {
                var token = _fileContent[fieldToModify];
                token?.AsObject().Remove(fieldToModify);
            }
            else
            {
                AddKey(fieldToModify, value);
            }
        }
    }

    public JsonObject GetJsonObject()
    {
        if (_fileContent is null)
        {
            return new();
        }

        return _fileContent;
    }

    public void MergeJsonObject(JsonObject jobject)
    {
        _fileContent?.Concat(jobject);
    }

    private async Task<bool> IsFileLockedAsync()
    {
        var canContinue = false;
        var count = 0;

        while (!canContinue)
        {
            if (count >= 10)
            {
                throw new IOException($"The current process cannot access the file: '{FullName}' because it is being used by another process.!");
            }
            count++;

            if (IsFileLocked(_fileInfo))
            {
                await Task.Delay(1000);
            }
            else
            {
                canContinue = true;
            }
        }

        return false;
    }

    public void Clear()
    {
        _fileContent = new();
    }

    public async Task LoadFileAsync(bool forceRead = false)
    {
        await _asyncLock.Lock(async () =>
        {
            if ((_fileContent == null || forceRead) && !await IsFileLockedAsync())
            {
                using StreamReader r = new(FullName);
                try
                {
                    _fileContent = JsonNode.Parse(await r.ReadToEndAsync())?.AsObject();
                }
                catch (Exception ex) when (ex is IOException or JsonException)
                {
                    _fileContent = JsonNode.Parse("{  }")?.AsObject();
                }
            }
        });
    }

    public void LoadFile(bool forceRead = false)
    {
        if ((_fileContent == null || forceRead) && !IsFileLocked(_fileInfo))
        {
            using StreamReader r = new(FullName);
            try
            {
                _fileContent = JsonNode.Parse(r.ReadToEnd())?.AsObject();
            }
            catch (Exception ex) when (ex is IOException or JsonException)
            {
                _fileContent = JsonNode.Parse("{  }")?.AsObject();
            }
        }
    }

    public async ValueTask<(T? Result, bool Success)> ReadKeyAsync<T>(string fieldName, bool forceRead = false)
    {
        await LoadFileAsync(forceRead);

        if (_fileContent?[fieldName] is not null)
        {
            try
            {
                var field = _fileContent?[fieldName];
                if (field is not null)
                {
                    var success = field.AsValue().TryGetValue<T>(out var content);
                    return (content, success);
                }
            }
            catch (FormatException)
            {
                return (default, false);
            }
        }

        return (default, false);
    }

    public void WriteAndFlush()
    {
        if (_fileContent is not null)
        {
            File.WriteAllText(FullName, _fileContent.ToString());
        }
    }

#if NET5_0_OR_GREATER
    public async Task WriteAndFlushAsync()
    {
        if (_fileContent is not null)
        {
            await File.WriteAllTextAsync(FullName, _fileContent.ToString());
        }
    }
#endif
}
