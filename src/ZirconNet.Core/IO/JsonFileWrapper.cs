using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZirconNet.Core.Async;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public class JsonFileWrapper : FileWrapper
{
    private JObject? _fileContent;
    private static readonly AsyncLock _asyncLock = new();

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
                var jsonObject = _fileContent is not null ? _fileContent : JObject.Parse("{  }");

                jsonObject?.Add(new JProperty(fieldToAdd, value));
                _fileContent = jsonObject;
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
                var jsonObject = _fileContent is not null ? _fileContent : JObject.Parse("{  }");

                jsonObject.Remove(fieldToDelete);
                _fileContent = jsonObject;
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
                token?.Replace(JToken.FromObject(value));
            }
            else
            {
                AddKey(fieldToModify, value);
            }
        }
    }

    public JObject GetJsonObject()
    {
        if (_fileContent is null)
        {
            return new();
        }

        return _fileContent;
    }

    public void MergeJsonObject(JObject jobject)
    {
        _fileContent?.Merge(jobject);
    }

    private async Task<bool> IsFileLockedAsync()
    {
        var canContinue = false;
        var count = 0;

        while (!canContinue)
        {
            if (count >= 10)
            {
                throw new IOException($"The process cannot access the file: '{FullName}' because it is being used by another process.!");
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
                    _fileContent = JObject.Parse(await r.ReadToEndAsync());
                }
                catch (Exception ex) when (ex is IOException or JsonException or JsonReaderException)
                {
                    _fileContent = JObject.Parse("{  }");
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
                _fileContent = JObject.Parse(r.ReadToEnd());
            }
            catch (Exception ex) when (ex is IOException or JsonException or JsonReaderException)
            {
                _fileContent = JObject.Parse("{  }");
            }
        }
    }

    public async Task<(T? Result, bool Success)> ReadKeyAsync<T>(string fieldName, bool forceRead = false)
    {
        await LoadFileAsync(forceRead);

        if (_fileContent?[fieldName] is not null)
        {
            try
            {
                var field = _fileContent?[fieldName];
                if (field is not null)
                {
                    var content = field.ToObject<T?>();
                    return (content, true);
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
