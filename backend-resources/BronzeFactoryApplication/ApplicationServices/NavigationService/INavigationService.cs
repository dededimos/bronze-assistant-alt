using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService
{
    public interface INavigationService
    {
        /// <summary>
        /// Executes a Navigation Action
        /// </summary>
        Task NavigateAsync();
    }
}
