// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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
using Carcass.Json.NewtonsoftJson.Providers.Abstracts;
using Newtonsoft.Json;

namespace Carcass.Json.NewtonsoftJson.Providers;

public sealed class NewtonsoftJsonProvider : INewtonsoftJsonProvider
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly JsonSerializer _jsonSerializer;

    public NewtonsoftJsonProvider(JsonSerializerSettings jsonSerializerSettings)
    {
        ArgumentVerifier.NotNull(jsonSerializerSettings, nameof(jsonSerializerSettings));

        _jsonSerializerSettings = jsonSerializerSettings;
        _jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
    }

    public T? TryDeserialize<T>(string json) where T : class
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
    }

    public object? TryDeserialize(string json, Type type)
    {
        ArgumentVerifier.NotNull(json, nameof(json));
        ArgumentVerifier.NotNull(type, nameof(type));

        return JsonConvert.DeserializeObject(json, type, _jsonSerializerSettings);
    }

    public T Deserialize<T>(string json) where T : class
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return TryDeserialize<T>(json) ?? throw new InvalidOperationException("Data is null.");
    }

    public object Deserialize(string json, Type type)
    {
        ArgumentVerifier.NotNull(json, nameof(json));

        return TryDeserialize(json, type) ?? throw new InvalidOperationException("Data is null.");
    }

    public string Serialize<T>(T data) where T : class
    {
        ArgumentVerifier.NotNull(data, nameof(data));

        TextWriter textWriter = new StringWriter();
        _jsonSerializer.Serialize(textWriter, data);

        string? json = textWriter.ToString();

        return json ?? throw new InvalidOperationException("JSON is null.");
    }
}