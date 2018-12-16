using System;

namespace Financier.Common.Actions
{
    public interface IAction
    {
        DateTime At { get; }

        decimal TotalBy(DateTime at);
    }
}
