// <copyright file="JsonFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Text.Json;
using System.Text.Json.Nodes;
using ZirconNet.Core.Async;

namespace ZirconNet.Core.IO;

/// <summary>
/// Should be disposed after use.
/// </summary>
public sealed class JsonFileWrapper : FileWrapperBase<JsonFileWrapper>, IDisposable
{
    private readonly StreamReader _reader;
    private readonly LockAsync _asyncLock = new();

    private JsonObject _fileContent = new();
    private bool _disposedValue;
    private bool _hasFlushedLastChanges;

    public JsonFileWrapper(string file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
        _reader = new StreamReader(FullName);
    }

    public JsonFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
        _reader = new StreamReader(FullName);
    }

    public void DeleteKey(string fieldToDelete)
    {
        _hasFlushedLastChanges = false;
        if (_fileContent is null)
        {
            return;
        }

        lock (_fileContent)
        {
            if (_fileContent?[fieldToDelete] is not null)
            {
                _fileContent = _fileContent is not null ? _fileContent : JsonNode.Parse("{  }")?.AsObject()!;

                _ = _fileContent?.Remove(fieldToDelete);
            }
        }
    }

    public void ModifyKey(string fieldToModify, object value)
    {
        _hasFlushedLastChanges = false;
        if (_fileContent is null)
        {
            return;
        }

        lock (_fileContent)
        {
            if (_fileContent?[fieldToModify] is not null)
            {
                _ = _fileContent.AsObject().Remove(fieldToModify);
            }
            else
            {
                AddKey(fieldToModify, value);
            }
        }
    }

    public JsonObject GetJsonObject()
    {
        return _fileContent is null ? new() : _fileContent;
    }

    public void MergeJsonObject(JsonObject jsonObject)
    {
        _hasFlushedLastChanges = false;
        _ = _fileContent?.Concat(jsonObject);
    }

    public void Clear()
    {
        _hasFlushedLastChanges = false;
        _fileContent = new();
    }

    public async Task LoadFileAsync(bool forceRead = false)
    {
        await _asyncLock.Lock<Task>(async () => await LoadFileInternalAsync(forceRead)).ConfigureAwait(false);
    }

    public void LoadFile(bool forceRead = false)
    {
        LoadFileInternalAsync(forceRead).GetAwaiter().GetResult();
    }

    public async ValueTask<(T? Result, bool Success)> ReadKeyAsync<T>(string fieldName, bool forceRead = false)
    {
        await LoadFileAsync(forceRead).ConfigureAwait(false);

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
            _hasFlushedLastChanges = true;
            File.WriteAllText(FullName, _fileContent.ToString());
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

#if NET5_0_OR_GREATER
    public async Task WriteAndFlushAsync()
    {
        if (_fileContent is not null)
        {
            _hasFlushedLastChanges = true;
            await File.WriteAllTextAsync(FullName, _fileContent.ToString()).ConfigureAwait(false);
        }
    }
#endif

    private async Task LoadFileInternalAsync(bool forceRead)
    {
        if ((_fileContent == null || forceRead) && !await IsFileLockedAsync())
        {
            try
            {
                _fileContent = JsonNode.Parse(await _reader.ReadToEndAsync())?.AsObject() ?? new();
            }
            catch (IOException)
            {
                _fileContent = new();
            }
            catch (JsonException)
            {
                _fileContent = new();
            }
        }
    }

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
                _fileContent = _fileContent is not null ? _fileContent : JsonNode.Parse("{  }")?.AsObject()!;

                _fileContent?.Add(fieldToAdd, value.ToString());
            }
        }
    }

    private async Task<bool> IsFileLockedAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            try
            {
                using var stream = File.Open(FileInfo.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        throw new IOException($"The current process cannot access the file: '{FullName}' because it is being used by another process!");
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (!_hasFlushedLastChanges)
                {
                    WriteAndFlush();
                }

                _asyncLock?.Dispose();
                _reader?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
