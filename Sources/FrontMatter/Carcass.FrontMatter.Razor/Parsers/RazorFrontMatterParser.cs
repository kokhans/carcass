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

using Carcass.Core;
using Carcass.FrontMatter.Core.Parsers.Abstracts;
using Carcass.Yaml.Core.Providers.Abstracts;

namespace Carcass.FrontMatter.Razor.Parsers;

public sealed class RazorFrontMatterParser : IFrontMatterParser
{
    private const string FrontMatterStartPrefix = "@*front-matter";
    private const string FrontMatterEndPrefix = "*@";

    private readonly IYamlProvider _yamlProvider;

    public RazorFrontMatterParser(IYamlProvider yamlProvider)
    {
        ArgumentVerifier.NotNull(yamlProvider, nameof(yamlProvider));

        _yamlProvider = yamlProvider;
    }

    public T? Parse<T>(string data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        int startIndex = data.IndexOf(FrontMatterStartPrefix, StringComparison.InvariantCultureIgnoreCase);
        if (startIndex < 0)
            return null;

        startIndex += FrontMatterStartPrefix.Length;
        int length = data[startIndex..].IndexOf(FrontMatterEndPrefix, StringComparison.InvariantCultureIgnoreCase);
        if (length < 0)
            return null;

        string yamlString = data.Substring(startIndex, length);

        return _yamlProvider.Deserialize<T>(yamlString);
    }
}