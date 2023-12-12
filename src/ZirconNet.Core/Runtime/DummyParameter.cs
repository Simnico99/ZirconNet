// <copyright file="DummyParameter.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Runtime;

/// <summary>
/// Dummy parameter to force one function or another SHOULD NOT BE USED.
/// </summary>
public abstract class DummyParameter
{
    public DummyParameter()
    {
        throw new InvalidOperationException("Dummy parameter shouldn't be used or initialized.");
    }
}