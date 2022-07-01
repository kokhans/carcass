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
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;

namespace Carcass.FrontMatter.Markdown.Parsers;

public sealed class MarkdownFrontMatterParser : IFrontMatterParser
{
    private readonly IYamlProvider _yamlProvider;
    private readonly MarkdownPipeline _markdownPipeline;

    public MarkdownFrontMatterParser(IYamlProvider yamlProvider, MarkdownPipeline markdownPipeline)
    {
        ArgumentVerifier.NotNull(yamlProvider, nameof(yamlProvider));
        ArgumentVerifier.NotNull(markdownPipeline, nameof(markdownPipeline));

        _yamlProvider = yamlProvider;
        _markdownPipeline = markdownPipeline;
    }

    public T? Parse<T>(string data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        MarkdownDocument markdownDocument = Markdig.Markdown.Parse(data, _markdownPipeline);
        YamlFrontMatterBlock? yamlFrontMatterBlock = markdownDocument
            .Descendants<YamlFrontMatterBlock>()
            .FirstOrDefault();

        return yamlFrontMatterBlock?.Lines.Count > 0
            ? _yamlProvider.Deserialize<T>(yamlFrontMatterBlock.Lines.ToString())
            : null;
    }
}