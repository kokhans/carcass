// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.ObjectModel;
using System.Text;
using Carcass.Core;
using Carcass.Core.Extensions;
using Carcass.LocalStorage.Extensions;
using Carcass.LocalStorage.Models;
using Carcass.LocalStorage.Providers.Abstracts;

namespace Carcass.LocalStorage.Providers;

public sealed class LocalStorageProvider : ILocalStorageProvider
{
    public LocalTemporaryDirectory CreateTemporaryDirectory(string rootDirectoryPath, string? prefix = default)
    {
        ArgumentVerifier.NotNull(rootDirectoryPath, nameof(rootDirectoryPath));

        LocalTemporaryDirectory localTemporaryDirectory = new(rootDirectoryPath, prefix);
        Directory.CreateDirectory(localTemporaryDirectory.TemporaryDirectoryPath);

        return localTemporaryDirectory;
    }

    public async Task<LocalFile> CreateTemporaryFileAsync(
        LocalTemporaryDirectory localTemporaryDirectory,
        string? content,
        string? extension,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(localTemporaryDirectory, nameof(localTemporaryDirectory));

        string temporaryFilePath = Path.Combine(
            localTemporaryDirectory.TemporaryDirectoryPath,
            $"{ShortGuid.NewGuid().Value}{extension}"
        );
        await using (StreamWriter streamWriter = File.CreateText(temporaryFilePath))
            await streamWriter.WriteLineAsync(content);

        return new FileInfo(temporaryFilePath).AsLocalFile(localTemporaryDirectory.RootDirectoryPath);
    }

    public ReadOnlyCollection<LocalFile> GetAllFiles(string directoryPath)
    {
        ArgumentVerifier.NotNull(directoryPath, nameof(directoryPath));

        return new DirectoryInfo(directoryPath)
            .EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Select(fi => fi.AsLocalFile(directoryPath))
            .ToList()
            .AsReadOnlyCollection();
    }

    public async Task CopyFileAsync(
        string sourceFilePath,
        string destinationFilePath,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(sourceFilePath, nameof(sourceFilePath));
        ArgumentVerifier.NotNull(destinationFilePath, nameof(destinationFilePath));

        FileInfo fileInfo = new(destinationFilePath);
        Directory.CreateDirectory(fileInfo.Directory!.FullName);

        await using FileStream source = File.Open(sourceFilePath, FileMode.Open);
        await using FileStream destination = File.Create(destinationFilePath);

        await source.CopyToAsync(destination, cancellationToken);
    }

    public async Task CreateFileAsync(
        string? content,
        string destinationFilePath,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(destinationFilePath, nameof(destinationFilePath));

        FileInfo fileInfo = new(destinationFilePath);
        Directory.CreateDirectory(fileInfo.Directory!.FullName);

        await File.WriteAllTextAsync(destinationFilePath, content, cancellationToken);
    }

    public async Task<string> ReadFileAsTextAsync(
        string localFilePath,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(localFilePath, nameof(localFilePath));

        await using FileStream fileStream = new(
            localFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            true
        );
        using StreamReader streamReader = new(fileStream, Encoding.UTF8);

        return await streamReader.ReadToEndAsync();
    }

    public void DeleteDirectory(string directoryPath)
    {
        ArgumentVerifier.NotNull(directoryPath, nameof(directoryPath));

        if (Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, true);
    }
}