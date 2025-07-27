using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Interfaces
{
    public interface IButtonRegulator
    {
        public int NumberOfButtons { get; }
        List<string> RegulatedItems { get; }
    }
}
