using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Events;
public interface IWeakEvent
{
    void Publish();
    ConfiguredTaskAwaitable PublishAsync(bool configureAwait = false);
    Subscription Subscribe(Action action);
}