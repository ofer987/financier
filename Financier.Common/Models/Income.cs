using System;

namespace Financier.Common.Models
{
    public abstract class Income
    {
        public abstract decimal Value(DateTime from, DateTime to);
    }
}
