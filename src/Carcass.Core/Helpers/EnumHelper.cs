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

using System.ComponentModel;
using System.Reflection;

namespace Carcass.Core.Helpers;

public static class EnumHelper
{
    public static TEnum? GetValueFromDescription<TEnum>(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        Type type = typeof(TEnum);
        if (!type.IsEnum)
            throw new InvalidOperationException($"Type {typeof(TEnum).Name} is not enum.");

        foreach (FieldInfo field in type.GetFields())
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (string.Equals(attribute.Description, value, StringComparison.InvariantCultureIgnoreCase))
                    return (TEnum?) field.GetValue(null);
            }
            else if (string.Equals(field.Name, value, StringComparison.InvariantCultureIgnoreCase))
                return (TEnum?) field.GetValue(null);

        return default;
    }
}