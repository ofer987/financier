using System;

namespace Financier.Common.Models.Income
{
    public abstract class Base
    {
        public abstract decimal Value(DateTime from, DateTime to);
    }
}
