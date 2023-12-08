using System;
using System.Collections.Generic;
using System.Text;

namespace Parallax.WPF;
internal class ActionDisposable : IDisposable
{
    private readonly Action _action;
    private bool _disposed;

    public ActionDisposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _action();
    }
}

