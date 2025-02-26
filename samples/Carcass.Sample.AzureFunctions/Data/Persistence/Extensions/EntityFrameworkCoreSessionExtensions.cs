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

using Carcass.Core;
using Carcass.Data.EntityFrameworkCore.Sessions.Abstracts;
using Carcass.Sample.AzureFunctions.Data.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Carcass.Sample.AzureFunctions.Data.Persistence.Extensions;

public static class EntityFrameworkCoreSessionExtensions
{
    public static Task<User> GetUserByFirebaseId(
        this IEntityFrameworkCoreSession session,
        string firebaseId,
        bool asNoTracking = false,
        Func<IQueryable<User>, IQueryable<User>>? configure = null)
    {
        ArgumentVerifier.NotNull(session, nameof(session));
        ArgumentVerifier.NotNull(firebaseId, nameof(firebaseId));

        IQueryable<User> query = session
            .Query<User>(asNoTracking);

        if (configure is not null)
            query = configure(query);

        return query.SingleAsync(u => u.FirebaseId == firebaseId);
    }
}