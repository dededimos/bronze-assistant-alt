//using Moq;
//using ShowerEnclosuresModelsLibrary.Enums;
//using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
//using ShowerEnclosuresModelsLibrary.Factory;
//using ShowerEnclosuresModelsLibrary.Models;
//using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
//using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
//using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.RepositoryImplementations;
//using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
//using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit.Abstractions;
//using Xunit.Sdk;

//namespace ShowerEnclosuresModelsLibrary.Tests.FactoryTests
//{
//    public class CabinFactoryTests
//    {
//        private readonly ITestOutputHelper outputHelper;

//        //Data Generation
//        private static readonly CabinMemoryRepository repo = new();
//        private readonly CabinFactory factory;

//        public CabinFactoryTests(ITestOutputHelper outputHelper)
//        {
//            factory = new CabinFactory(repo);
//            this.outputHelper = outputHelper;
//        }

//        [Theory]
//        [MemberData(nameof(Data))]
//        public void CreateCabin_ShouldReturnProperConfiguration(
//            CabinDrawNumber drawNumberExpected, 
//            CabinSynthesisModel synthesisModelExpected,
//            CabinSettings expectedSettings,
//            ICabinParts expectedParts,
//            CabinConstraints expectedConstraints)
//        {
//            Cabin cabin = factory.CreateCabin(drawNumberExpected, synthesisModelExpected);

//            var actualParts = cabin.Parts;
//            var actualConstraints = cabin.Constraints;

//            Assert.NotNull(cabin);
//            Assert.Equal(drawNumberExpected, cabin.IsPartOfDraw);
//            Assert.Equal(synthesisModelExpected, cabin.SynthesisModel);
            
//            Assert.NotNull(cabin.Constraints);
//            Assert.NotNull(cabin.Parts);

//            Assert.Equal(expectedSettings.Model,            cabin.Model);
//            Assert.Equal(expectedSettings.MetalFinish,      cabin.MetalFinish);
//            Assert.Equal(expectedSettings.Thicknesses,      cabin.Thicknesses);
//            Assert.Equal(expectedSettings.GlassFinish,      cabin.GlassFinish);
//            Assert.Equal(expectedSettings.Height,           cabin.Height);
//            Assert.Equal(expectedSettings.NominalLength,    cabin.NominalLength);
//            Assert.Equal(expectedSettings.IsReversible,     cabin.IsReversible);

//            Assert.Equivalent(expectedParts, actualParts);
//            Assert.Equivalent(actualConstraints, expectedConstraints);
//        }

//        /// <summary>
//        /// Parametrized Data for the Above Theory Test
//        /// </summary>
//        public static IEnumerable<object[]> Data => new List<object[]>
//        {
//            #region 9S Data (3)
//            new object[]
//            {
//                CabinDrawNumber.Draw9S,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S,CabinSynthesisModel.Primary)] 
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9S9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S9F,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9S9F9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9S,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region 94 Data (3)
//            new object[]
//            {
//                CabinDrawNumber.Draw94,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model94,CabinDrawNumber.Draw94,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model94,CabinDrawNumber.Draw94,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model94,CabinDrawNumber.Draw94,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw949F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model94,CabinDrawNumber.Draw949F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model94,CabinDrawNumber.Draw949F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model94,CabinDrawNumber.Draw949F,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw949F9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model94,CabinDrawNumber.Draw949F9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model94,CabinDrawNumber.Draw949F9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model94,CabinDrawNumber.Draw949F9F,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region 9A Data (4)
//            new object[]
//            {
//                CabinDrawNumber.Draw9A,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9A,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9A9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9A9F,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9A,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region 9B Data (3)
//            new object[]
//            {
//                CabinDrawNumber.Draw9B,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9B9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B9F,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9B9F9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B9F9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B9F9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9B,CabinDrawNumber.Draw9B9F9F,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region 9F Data (9)
//            new object[]
//            {
//                CabinDrawNumber.Draw9S9F,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9S9F9F,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9S9F9F,
//                CabinSynthesisModel.Tertiary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Tertiary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Tertiary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9S9F9F,CabinSynthesisModel.Tertiary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw949F,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw949F,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw949F,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw949F,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw949F9F,
//                CabinSynthesisModel.Tertiary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw949F9F,CabinSynthesisModel.Tertiary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw949F9F,CabinSynthesisModel.Tertiary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw949F9F,CabinSynthesisModel.Tertiary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9A9F,
//                CabinSynthesisModel.Tertiary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Tertiary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Tertiary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9A9F,CabinSynthesisModel.Tertiary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9B9F,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9B9F,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9B9F,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9B9F,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9B9F9F,
//                CabinSynthesisModel.Tertiary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9B9F9F,CabinSynthesisModel.Tertiary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9B9F9F,CabinSynthesisModel.Tertiary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9B9F9F,CabinSynthesisModel.Tertiary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9C9F,
//                CabinSynthesisModel.Tertiary,
//                repo.AllSettings[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Tertiary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Tertiary)],
//                repo.AllConstraints[(CabinModelEnum.Model9F,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Tertiary)]
//            },
//            #endregion

//            #region 9C Data (4)
//            new object[]
//            {
//                CabinDrawNumber.Draw9C,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9C,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9C9F,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw9C9F,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.Model9C,CabinDrawNumber.Draw9C9F,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region DB Data (9)
//            new object[]
//            {
//                CabinDrawNumber.DrawDB51,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawDB51,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawDB51,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawDB51,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerDB8W52,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawCornerDB8W52,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawCornerDB8W52,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawCornerDB8W52,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerDB8W52,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerDB8W52,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerDB8W52,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerDB8W52,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerDB53,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2CornerDB53,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2CornerDB53,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2CornerDB53,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerDB53,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2CornerDB53,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2CornerDB53,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2CornerDB53,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightDB8W59,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawStraightDB8W59,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawStraightDB8W59,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.DrawStraightDB8W59,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightDB8W59,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightDB8W59,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightDB8W59,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightDB8W59,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightDB61,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2StraightDB61,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2StraightDB61,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2StraightDB61,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightDB61,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2StraightDB61,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2StraightDB61,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelDB,CabinDrawNumber.Draw2StraightDB61,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region E Data (1)
//            new object[]
//            {
//                CabinDrawNumber.DrawE,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelE,CabinDrawNumber.DrawE,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelE,CabinDrawNumber.DrawE,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelE,CabinDrawNumber.DrawE,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region W - W40 Data (11)
//            new object[]
//            {
//                CabinDrawNumber.Draw8W,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw8W,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw8W,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw8W,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw8W40,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.Model8W40,CabinDrawNumber.Draw8W40,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.Model8W40,CabinDrawNumber.Draw8W40,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.Model8W40,CabinDrawNumber.Draw8W40,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerStraight8W88,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerStraight8W88,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerStraight8W88,
//                CabinSynthesisModel.Tertiary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Tertiary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Tertiary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2CornerStraight8W88,CabinSynthesisModel.Tertiary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw1Corner8W84,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw1Corner8W84,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw1Corner8W84,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw1Corner8W84,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw1Corner8W84,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw1Corner8W84,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw1Corner8W84,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw1Corner8W84,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2Corner8W82,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Corner8W82,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Corner8W82,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Corner8W82,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2Corner8W82,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Corner8W82,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Corner8W82,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Corner8W82,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2Straight8W85,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Straight8W85,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Straight8W85,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Straight8W85,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2Straight8W85,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Straight8W85,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Straight8W85,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw2Straight8W85,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region WFLIPPER (2)
//            new object[]
//            {
//                CabinDrawNumber.Draw8WFlipper81,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.Draw8WFlipper81,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.Draw8WFlipper81,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.Draw8WFlipper81,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw8WFlipper81,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelWFlipper,CabinDrawNumber.Draw8WFlipper81,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelWFlipper,CabinDrawNumber.Draw8WFlipper81,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelWFlipper,CabinDrawNumber.Draw8WFlipper81,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region HB Data (9)
//            new object[]
//            {
//                CabinDrawNumber.DrawHB34,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawHB34,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawHB34,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawHB34,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerHB8W35,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawCornerHB8W35,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawCornerHB8W35,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawCornerHB8W35,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerHB8W35,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerHB8W35,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerHB8W35,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerHB8W35,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerHB37,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2CornerHB37,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2CornerHB37,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2CornerHB37,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerHB37,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2CornerHB37,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2CornerHB37,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2CornerHB37,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightHB8W40,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawStraightHB8W40,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawStraightHB8W40,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.DrawStraightHB8W40,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightHB8W40,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightHB8W40,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightHB8W40,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightHB8W40,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightHB43,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2StraightHB43,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2StraightHB43,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2StraightHB43,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightHB43,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2StraightHB43,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2StraightHB43,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelHB,CabinDrawNumber.Draw2StraightHB43,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region VS Data (2)
//            new object[]
//            {
//                CabinDrawNumber.DrawVS,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelVS,CabinDrawNumber.DrawVS,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelVS,CabinDrawNumber.DrawVS,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelVS,CabinDrawNumber.DrawVS,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawVSVF,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelVS,CabinDrawNumber.DrawVSVF,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelVS,CabinDrawNumber.DrawVSVF,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelVS,CabinDrawNumber.DrawVSVF,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region V4 Data (2)
//            new object[]
//            {
//                CabinDrawNumber.DrawV4,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelV4,CabinDrawNumber.DrawV4,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelV4,CabinDrawNumber.DrawV4,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelV4,CabinDrawNumber.DrawV4,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawV4VF,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelV4,CabinDrawNumber.DrawV4VF,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelV4,CabinDrawNumber.DrawV4VF,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelV4,CabinDrawNumber.DrawV4VF,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region VA Data (1)
//            new object[]
//            {
//                CabinDrawNumber.DrawVA,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelVA,CabinDrawNumber.DrawVA,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelVA,CabinDrawNumber.DrawVA,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelVA,CabinDrawNumber.DrawVA,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region VF Data (2)
//            new object[]
//            {
//                CabinDrawNumber.DrawVSVF,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelVF,CabinDrawNumber.DrawVSVF,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelVF,CabinDrawNumber.DrawVSVF,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelVF,CabinDrawNumber.DrawVSVF,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawV4VF,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelVF,CabinDrawNumber.DrawV4VF,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelVF,CabinDrawNumber.DrawV4VF,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelVF,CabinDrawNumber.DrawV4VF,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region NB Data (9)
//            new object[]
//            {
//                CabinDrawNumber.DrawNB31,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawNB31,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawNB31,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawNB31,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerNB6W32,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawCornerNB6W32,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawCornerNB6W32,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawCornerNB6W32,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerNB6W32,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerNB6W32,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerNB6W32,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerNB6W32,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerNB33,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2CornerNB33,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2CornerNB33,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2CornerNB33,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerNB33,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2CornerNB33,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2CornerNB33,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2CornerNB33,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightNB6W38,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawStraightNB6W38,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawStraightNB6W38,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.DrawStraightNB6W38,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightNB6W38,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightNB6W38,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightNB6W38,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightNB6W38,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightNB41,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2StraightNB41,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2StraightNB41,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2StraightNB41,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightNB41,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2StraightNB41,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2StraightNB41,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNB,CabinDrawNumber.Draw2StraightNB41,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region NP Data - (9)
//            new object[]
//            {
//                CabinDrawNumber.DrawNP44,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawNP44,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawNP44,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawNP44,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerNP46,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2CornerNP46,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2CornerNP46,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2CornerNP46,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2CornerNP46,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2CornerNP46,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2CornerNP46,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2CornerNP46,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightNP48,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2StraightNP48,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2StraightNP48,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2StraightNP48,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.Draw2StraightNP48,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2StraightNP48,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2StraightNP48,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.Draw2StraightNP48,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerNP6W45,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawCornerNP6W45,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawCornerNP6W45,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawCornerNP6W45,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightNP6W47,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawStraightNP6W47,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawStraightNP6W47,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNP,CabinDrawNumber.DrawStraightNP6W47,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawCornerNP6W45,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerNP6W45,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerNP6W45,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawCornerNP6W45,CabinSynthesisModel.Secondary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawStraightNP6W47,
//                CabinSynthesisModel.Secondary,
//                repo.AllSettings[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightNP6W47,CabinSynthesisModel.Secondary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightNP6W47,CabinSynthesisModel.Secondary)],
//                repo.AllConstraints[(CabinModelEnum.ModelW,CabinDrawNumber.DrawStraightNP6W47,CabinSynthesisModel.Secondary)]
//            },
//            #endregion

//            #region WS Data (1)
//            new object[]
//            {
//                CabinDrawNumber.DrawWS,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelWS,CabinDrawNumber.DrawWS,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelWS,CabinDrawNumber.DrawWS,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelWS,CabinDrawNumber.DrawWS,CabinSynthesisModel.Primary)]
//            },
//            #endregion

//            #region NV - MV

//            new object[]
//            {
//                CabinDrawNumber.DrawNV,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNV,CabinDrawNumber.DrawNV,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNV,CabinDrawNumber.DrawNV,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNV,CabinDrawNumber.DrawNV,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawMV2,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelMV2,CabinDrawNumber.DrawMV2,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelMV2,CabinDrawNumber.DrawMV2,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelMV2,CabinDrawNumber.DrawMV2,CabinSynthesisModel.Primary)]
//            },
//            new object[]
//            {
//                CabinDrawNumber.DrawNV2,
//                CabinSynthesisModel.Primary,
//                repo.AllSettings[(CabinModelEnum.ModelNV2,CabinDrawNumber.DrawNV2,CabinSynthesisModel.Primary)],
//                repo.AllPartsLists[(CabinModelEnum.ModelNV2,CabinDrawNumber.DrawNV2,CabinSynthesisModel.Primary)],
//                repo.AllConstraints[(CabinModelEnum.ModelNV2,CabinDrawNumber.DrawNV2,CabinSynthesisModel.Primary)]
//            },

//            #endregion
//        };


//    }
//}


