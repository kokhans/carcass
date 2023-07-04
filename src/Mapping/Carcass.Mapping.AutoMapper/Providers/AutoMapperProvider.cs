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

using AutoMapper;
using Carcass.Core;
using Carcass.Mapping.AutoMapper.Providers.Abstracts;

namespace Carcass.Mapping.AutoMapper.Providers;

public sealed class AutoMapperProvider : IAutoMapperProvider
{
    private readonly IMapper _mapper;

    public AutoMapperProvider(IMapper mapper)
    {
        ArgumentVerifier.NotNull(mapper, nameof(mapper));

        _mapper = mapper;
    }

    public TDestination Map<TSource, TDestination>(
        TSource source,
        IDictionary<string, object>? metadata = default
    )
    {
        if (metadata is null)
            return _mapper.Map<TSource, TDestination>(source);

        return _mapper.Map<TSource, TDestination>(source, moo =>
            {
                foreach (KeyValuePair<string, object> item in metadata)
                    moo.Items.Add(item.Key, item.Value);
            }
        );
    }
}