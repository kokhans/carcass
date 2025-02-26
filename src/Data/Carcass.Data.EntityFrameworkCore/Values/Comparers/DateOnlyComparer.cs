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
///     Provides a value comparer for <see cref="DateOnly" /> instances that compares them based on their day numbers.
/// </summary>
/// <remarks>
///     This comparer is designed to ensure that <see cref="DateOnly" /> objects are compared consistently in
///     Entity Framework Core, focusing on logical equality rather than reference equality.
/// </remarks>
/// <exception cref="ArgumentNullException">Thrown when a null instance is provided to the comparison methods.</exception>
public class DateOnlyComparer() : ValueComparer<DateOnly>(
    (dateOnly1, dateOnly2) => dateOnly1.DayNumber == dateOnly2.DayNumber,
    dateOnly => dateOnly.GetHashCode());