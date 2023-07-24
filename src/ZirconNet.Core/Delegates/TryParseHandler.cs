// <copyright file="TryParseHandler.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Delegates;

/// <summary>
/// This is a <see cref="TryParseHandler"/> delegate.
/// </summary>
/// <typeparam name="T">Return type.</typeparam>
/// <param name="value">The span.</param>
/// <param name="result">The result.</param>
/// <returns>The parsed result.</returns>
public delegate bool TryParseHandler<T>(ReadOnlySpan<char> value, out T result);