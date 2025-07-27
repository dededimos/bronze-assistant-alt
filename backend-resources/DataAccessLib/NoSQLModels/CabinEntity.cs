using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;

namespace DataAccessLib.NoSQLModels
{
    /// <summary>
    /// An Entity representing a Cabin inside a GlassesOrder
    /// The Entity does not Save any Glasses , only the Rest Relevant properties of the Cabin
    /// The Glasses are added only during object Construction in its Parent CabinRow
    /// </summary>
    public class CabinEntity : DbEntity
    {
        public CabinModelEnum Model { get; set; }
        public CabinFinishEnum MetalFinish { get; set; }
        public CabinThicknessEnum Thicknesses { get; set; }
        public GlassFinishEnum GlassFinish { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsCodeOverriden { get; set; } = false;
        public double Opening { get; set; }
        public bool HasStep { get; set; }
        public CabinSeries Series { get; set; }
        public List<CabinExtra> Extras { get; set; } = new();
        public CabinDrawNumber IsPartOfDraw { get; set; }
        public CabinSynthesisModel SynthesisModel { get; set; }
        public CabinDirection Direction { get; set; }
        public bool IsReversible { get; set; }
        public int NominalLength { get; set; }
        public int Height { get; set; }
        public CabinConstraints Constraints { get; set; } = CabinConstraints.Empty();
        public CabinPartsListEntity PartsList { get; set; } = CabinPartsListEntity.Empty();
        public string TypeDiscriminator { get; set; } = string.Empty;

        public CabinEntity()
        {

        }

        public CabinEntity(Cabin cabin)
        {
            Model = cabin.Model ?? throw new InvalidOperationException($"Cannot Create entity with Null {nameof(CabinModelEnum)}");
            MetalFinish = cabin.MetalFinish ?? CabinFinishEnum.NotSet;
            Thicknesses = cabin.Thicknesses ?? CabinThicknessEnum.NotSet;
            GlassFinish = cabin.GlassFinish ?? GlassFinishEnum.GlassFinishNotSet;
            Code = cabin.Code;
            IsCodeOverriden = cabin.IsCodeOverriden;
            Opening = cabin.Opening;
            HasStep = cabin.HasStep;
            Series = cabin.Series;
            Extras = new(cabin.Extras);
            IsPartOfDraw = cabin.IsPartOfDraw;
            SynthesisModel = cabin.SynthesisModel;
            Direction = cabin.Direction;
            IsReversible = cabin.IsReversible;
            NominalLength = cabin.NominalLength;
            Height = cabin.Height;
            Constraints = cabin.Constraints;
            PartsList = new(cabin.Parts);
            TypeDiscriminator = cabin.GetType().AssemblyQualifiedName ?? "InvalidType";
        }

        public Cabin ToCabin()
        {
            CabinPartsList parts = this.PartsList.ToPartsList();
            Type cabinType = Type.GetType(TypeDiscriminator) 
                ?? throw new Exception($"TypeDiscriminator Value: '{TypeDiscriminator}' does not match any known type -- -- thrown at {nameof(CabinEntity)}-Method :{nameof(ToCabin)}");

            //Create an Instance of the Requested Type
            if (Activator.CreateInstance(cabinType,Constraints,parts) is not Cabin cabin)
            {
                throw new Exception($"Inconsistent TypeDiscriminator , Expected a derived Type of{nameof(CabinPartsList)} , Actual {TypeDiscriminator}");

            }
            cabin.Model = Model;
            cabin.MetalFinish = MetalFinish;
            cabin.Thicknesses = Thicknesses;
            cabin.GlassFinish = GlassFinish;
            cabin.IsPartOfDraw = IsPartOfDraw;
            cabin.SynthesisModel = SynthesisModel;
            cabin.Direction = Direction;
            cabin.IsReversible = IsReversible;
            cabin.NominalLength = NominalLength;
            cabin.Height = Height;

            //If the Code is Overriden => override it again to the Model
            if (IsCodeOverriden) { cabin.OverrideCode(Code); }

            foreach (var extra in Extras)
            {
                cabin.Extras.Add(extra);
            }
            return cabin;
        }

    }
}
