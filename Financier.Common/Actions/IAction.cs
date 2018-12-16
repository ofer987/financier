using System;

namespace Financier.Common.Actions
{
    public interface IAction
    {
        DateTime At { get; }
    }
}
