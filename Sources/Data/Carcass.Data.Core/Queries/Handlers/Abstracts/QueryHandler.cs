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
using Carcass.Data.Core.Queries.Abstracts;
using Carcass.Data.Core.Queries.ResultModels.Abstracts;
using Carcass.Data.Core.Sessions.Abstracts;

namespace Carcass.Data.Core.Queries.Handlers.Abstracts;

public interface IQueryHandler<in TQuery, TQueryResultModel>
    where TQuery : class, IQuery<TQueryResultModel>
    where TQueryResultModel : class, IQueryResultModel
{
    Task<TQueryResultModel> HandleQueryAsync(TQuery query, CancellationToken cancellationToken = default);
}

public abstract class QueryHandler<TQuery, TQueryResultModel, TSession> : IQueryHandler<TQuery, TQueryResultModel>
    where TQuery : class, IQuery<TQueryResultModel>
    where TQueryResultModel : class, IQueryResultModel
    where TSession : class, ISession
{
    protected QueryHandler(TSession session)
    {
        ArgumentVerifier.NotNull(session, nameof(session));

        Session = session;
    }

    protected readonly TSession Session;

    public abstract Task<TQueryResultModel> HandleQueryAsync(
        TQuery query,
        CancellationToken cancellationToken = default
    );
}