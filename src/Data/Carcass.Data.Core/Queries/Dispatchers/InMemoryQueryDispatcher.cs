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
using Carcass.Data.Core.Queries.Abstracts;
using Carcass.Data.Core.Queries.Dispatchers.Abstracts;
using Carcass.Data.Core.Queries.Handlers.Abstracts;
using Carcass.Data.Core.Queries.Results.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Carcass.Data.Core.Queries.Dispatchers;

public sealed class InMemoryQueryDispatcher : IQueryDispatcher
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public InMemoryQueryDispatcher(IServiceScopeFactory serviceScopeFactory)
    {
        ArgumentVerifier.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TQueryResult> DispatchQueryAsync<TQuery, TQueryResult>(
        TQuery query,
        CancellationToken cancellationToken = default
    )
        where TQuery : class, IQuery<TQueryResult>
        where TQueryResult : class, IQueryResult
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(query, nameof(query));

        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        IQueryHandler<TQuery, TQueryResult> queryHandler =
            serviceScope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();

        return await queryHandler.HandleQueryAsync(query, cancellationToken);
    }
}