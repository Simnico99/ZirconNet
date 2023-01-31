using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Delegates;
public delegate bool TryParseHandler<T>(ReadOnlySpan<char> value, out T result);