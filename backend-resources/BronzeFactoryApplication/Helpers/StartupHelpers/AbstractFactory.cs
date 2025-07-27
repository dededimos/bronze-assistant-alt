using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Helpers.StartupHelpers
{
    public class AbstractFactory<T> : IAbstractFactory<T>
    {
        private readonly Func<T> factory;

        /// <summary>
        /// Passes a Method that Returns a T instance
        /// </summary>
        /// <param name="factory"></param>
        public AbstractFactory(Func<T> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Returns a T instance
        /// </summary>
        /// <returns></returns>
        public T Create()
        {
            return factory();
        }
    }
}
