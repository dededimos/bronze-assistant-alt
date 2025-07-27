using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Factory;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Tests.FactoryTests
{
    public class CabinExtrasFactoryTests
    {

        [Theory]
        [InlineData(CabinExtraType.StepCut)]
        [InlineData(CabinExtraType.BronzeClean)]
        [InlineData(CabinExtraType.SafeKids)]
        public void CreateCabinExtra_ShouldReturnProperType(CabinExtraType type)
        {
            CabinExtra actualExtra = CabinExtrasFactory.CreateCabinExtra(type);
            Assert.Equal(type, actualExtra.ExtraType);
        }
    }
}
