﻿// MIT License
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

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Carcass.Data.EntityFrameworkCore.Values.Comparers;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides a comparer for the <see cref="TimeOnly" /> type, enabling proper comparison and hashing
///     within Entity Framework Core for properties of this type.
/// </summary>
/// <remarks>
///     This comparer is designed to compare <see cref="TimeOnly" /> objects based on their tick values
///     and ensures a consistent hash code is generated.
/// </remarks>
public sealed class TimeOnlyComparer() : ValueComparer<TimeOnly>(
    (timeOnly1, timeOnly2) => timeOnly1.Ticks == timeOnly2.Ticks,
    timeOnly => timeOnly.GetHashCode());