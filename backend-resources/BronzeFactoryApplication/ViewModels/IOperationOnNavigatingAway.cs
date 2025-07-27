using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels
{
    /// <summary>
    /// A View which Executes an Operation while Navigating away from it
    /// </summary>
    public interface IOperationOnNavigatingAway
    {
        /// <summary>
        /// Executes when navigating away from this View
        /// </summary>
        /// <returns></returns>
        Task OnNavigatingAwayOperation();
    }
}
