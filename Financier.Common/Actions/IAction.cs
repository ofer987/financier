using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public interface IAction
    {
        DateTime At { get; }

        IProduct Product { get; }

        decimal TotalBy(DateTime at);
    }
}
