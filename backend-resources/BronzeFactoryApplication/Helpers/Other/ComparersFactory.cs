using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using MirrorsRepositoryMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Helpers.Other
{
    /// <summary>
    /// NOT IMPLEMENTED 
    /// </summary>
    [Obsolete("Not Implemented ... Idea to Replace IEqualityComparerCreator<>")]
    public class ComparerRegistry
    {
        private static readonly Dictionary<Type, object> Comparers = [];

        /// <summary>
        /// Registers a comparer for a specific type.
        /// </summary>
        public static void RegisterComparer<T>(IEqualityComparer<T> comparer)
            where T : class
        {
            Comparers[typeof(T)] = comparer;
        }

        public static IEqualityComparer<T> GetComparer<T>()
            where T : class
        {
            if (Comparers.TryGetValue(typeof(T), out var comparer))
            {
                return (IEqualityComparer<T>)comparer;
            }

            throw new InvalidOperationException($"No comparer registered for type {typeof(T)}");
        }

        /// <summary>
        /// Bulk register comparers for initialization.
        /// </summary>
        public static void InitializeComparers()
        {

            RegisterComparer(new LocalizedStringEqualityComparer());
            RegisterComparer(new LocalizedDescriptionEqualityComparer());
            RegisterComparer(new IPRatingEqualityComparer());
            RegisterComparer(new MirrorConstraintsEqualityComparer());
            RegisterComparer(new CustomMirrorElementEntityEqualityComparer());
            RegisterComparer(new MirrorElementEntityBaseEqualityComparer());
            RegisterComparer(new MirrorFinishElementEqualityComparer());
            RegisterComparer(new MirrorLightElementEqualityComparer());
            RegisterComparer(new MirrorLightInfoEqualityComparer());
            RegisterComparer(new MirrorAdditionalLightInfoEqualityComparer());
            RegisterComparer(new MirrorLightEqualityComparer());
            RegisterComparer(new MirrorModuleEqualityComparer());
            RegisterComparer(new MirrorModuleInfoEqualityComparer());
                        
            // Add the remaining 122 comparers here
        }
    }
}
