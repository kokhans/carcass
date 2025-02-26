// MIT License
//
// Copyright (c) 2022-2025 Serhii Kokhan
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

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides utility methods for file-related operations.
/// </summary>
public static class FileHelper
{
    /// <summary>
    ///     Retrieves the file extension from the provided file name.
    /// </summary>
    /// <param name="fileName">The full name of the file including the extension.</param>
    /// <returns>
    ///     The file extension as a string if found, an empty string if no valid extension exists, or null if the input
    ///     file name is null or empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the input fileName is null or an invalid string.</exception>
    public static string? GetFileExtension(this string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return fileName;

        int dotIndex = fileName.LastIndexOf('.');

        return dotIndex >= 0 && dotIndex < fileName.Length - 1
            ? fileName[(dotIndex + 1)..]
            : string.Empty;
    }
}