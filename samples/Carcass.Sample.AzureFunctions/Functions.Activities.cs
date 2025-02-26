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

using Carcass.Sample.AzureFunctions.Contracts.Requests.Users;
using Carcass.Sample.AzureFunctions.Contracts.Responses.Users;
using Carcass.Sample.AzureFunctions.Data.Domain.Users;
using FirebaseAdmin.Auth;
using Microsoft.Azure.Functions.Worker;

namespace Carcass.Sample.AzureFunctions;

public sealed partial class Functions
{
    [Function(nameof(CreateFirebaseUserActivity))]
    public async Task<string> CreateFirebaseUserActivity(
        [ActivityTrigger] CreateFirebaseUserActivityInput input)
    {
        FirebaseAuth firebaseAuth = FirebaseAuth.GetAuth(_firebaseApp);

        UserRecordArgs userRecordArgs = new()
        {
            Email = input.Email,
            EmailVerified = false,
            Password = input.Password,
            Disabled = false,
            DisplayName = input.FullName
        };
        if (!string.IsNullOrWhiteSpace(input.PhoneNumber))
            userRecordArgs.PhoneNumber = input.PhoneNumber;

        UserRecord userRecord = await firebaseAuth.CreateUserAsync(userRecordArgs);

        await firebaseAuth.GenerateEmailVerificationLinkAsync(input.Email);

        return userRecord.Uid;
    }

    [Function(nameof(CreateDbUserActivity))]
    public async Task<CreateDbUserActivityOutput> CreateDbUserActivity(
        [ActivityTrigger] CreateDbUserActivityInput input)
    {
        User user = new()
        {
            FirebaseId = input.FirebaseId,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            FirstName = input.FirstName,
            LastName = input.LastName
        };
        await _session.CreateAsync(user);
        await _session.SaveChangesAsync();

        return _mapper.Map<User, CreateDbUserActivityOutput>(user);
    }
}