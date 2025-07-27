using Moq;
using ShowerEnclosuresModelsLibrary.Builder;
using ShowerEnclosuresModelsLibrary.Builder.GlassesConcreteBuilders.B6000Glasses;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.ProfilesModels;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.ConstraintsModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Tests.BuilderTests.GlassesConcreteBuildersTests.B6000GlassesTests
{
    public class DoorGlass94BuilderTests
    {
        public DoorGlass94BuilderTests()
        {
            Mock<DoorGlass94Builder> mockBuilder = new();
        }

        [Fact]
        public void DoorGlass94Builder_ShouldReturnCorrectGlass()
        {
            ////Contains all Defaults
            

            //int expectedHeight = 1900;
            
            //DoorGlass94Builder builder = new(cabin);
            
            //builder.SetGlassDraw();
            //builder.SetGlassFinish();
            //builder.SetGlassHeight();
            //builder.SetGlassLength();
            //Glass glass = builder.GetGlass();

            //Assert.NotNull(glass);
            //Assert.Equal(GlassDrawEnum.Draw94, glass.Draw);
            //Assert.Equal(GlassFinishEnum.Fume, glass.Finish);
            //Assert.Equal(expectedHeight, glass.Height);
            //Assert.Equal()
        }


    }
}

