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
using Carcass.Yaml.Core.Providers.Abstracts;
using YamlDotNet.Serialization;

// ReSharper disable ReturnTypeCanBeNotNullable

namespace Carcass.Yaml.YamlDotNet.Providers;

public sealed class YamlDotNetProvider : IYamlProvider
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlDotNetProvider(ISerializer serializer, IDeserializer deserializer)
    {
        ArgumentVerifier.NotNull(serializer, nameof(serializer));
        ArgumentVerifier.NotNull(deserializer, nameof(deserializer));

        _serializer = serializer;
        _deserializer = deserializer;
    }

    public T? Deserialize<T>(string data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        return _deserializer.Deserialize<T>(data);
    }

    public object? Deserialize(string data, Type type)
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        return _deserializer.Deserialize(data, type);
    }

    public string? Serialize<T>(T data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        return _serializer.Serialize(data);
    }
}