using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    public interface IRestorable<TRestorator>
    {
        /// <summary>
        /// Restores to a State defined by the <typeparamref name="TRestorator"/>
        /// </summary>
        /// <param name="restorator">The Restorator Object holding information about the Restore</param>
        void Restore(TRestorator restorator);
    }
}
