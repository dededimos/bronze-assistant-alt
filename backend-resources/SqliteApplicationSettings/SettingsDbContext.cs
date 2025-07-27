using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using ShowerEnclosuresModelsLibrary.Enums;
using SqliteApplicationSettings.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqliteApplicationSettings;

public class SettingsDbContext : DbContext
{
    public DbSet<XlsSettingsGlassesDTO> XlsSettingsGlassesTable { get; set; } //Has IsSelected Property to Define which is selected , Defaults Cannot be Deleted
    public DbSet<GeneralApplicationSettingDTO> GeneralSettingsTable { get; set; } //Has a single Table 
    public DbSet<SearchOrdersViewSettingsDTO> SearchOrdersViewSettingsTable { get; set; } //Has two tables the Defaults and the Ones Used (Defaults are only for restoration)
    
    /// <summary>
    /// The User Settings , the ones used in the Application 
    /// </summary>
    public DbSet<GlassesStockServiceSettingsDTO> GlassesStockServiceSettingsTable { get; set; }
    
    //Pass the options to the Base Constructor and will create the DB/Migration For us
    public SettingsDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Set the Name as Unique!
        modelBuilder.Entity<XlsSettingsGlassesDTO>().HasIndex(s => s.SettingsName).IsUnique();
        modelBuilder.Entity<XlsSettingsGlassesDTO>().HasIndex(s => s.IsSelected);

        //Write the Seed Data to the Database
        modelBuilder.Entity<XlsSettingsGlassesDTO>().HasData(GetSeedValuesXlsSettingsGlasses());
        modelBuilder.Entity<GeneralApplicationSettingDTO>().HasData
            (
                new GeneralApplicationSettingDTO() 
                { 
                    Created = DateTime.MinValue,
                    Id = 1,
                    LastModified = DateTime.MinValue,
                    InfoStringObject = string.Empty,
                    SettingName = "SelectedTheme",
                    SettingValue = "Dark"
                },
                new GeneralApplicationSettingDTO()
                {
                    Created = DateTime.MinValue,
                    Id = 2,
                    LastModified = DateTime.MinValue,
                    InfoStringObject = string.Empty,
                    SettingName = "SelectedLanguage",
                    SettingValue = "el-GR"
                },
                new GeneralApplicationSettingDTO()
                {
                    Created = DateTime.MinValue,
                    Id = 3,
                    LastModified = DateTime.MinValue,
                    InfoStringObject = string.Empty,
                    SettingName = "ApplicationVersion",
                    SettingValue = "Ver2.17-17-12-2023"
                }
            );
        modelBuilder.Entity<SearchOrdersViewSettingsDTO>().HasData(GetSeedValuesSearchOrdersViewSettings());

        modelBuilder.Entity<GlassesStockServiceSettingsDTO>(entity =>
        {
            entity.ToTable("GlassesStockServiceUserSettingsTable");
            entity.HasIndex(e => new{ e.ConcernsModel ,e.IsDefault}).IsUnique();
            entity.HasData(GetSeedValuesGlassesStockServiceSettings());
        });
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// The Default/Seed Values of the XlsSettingsGlasses
    /// </summary>
    /// <returns></returns>
    private XlsSettingsGlassesDTO GetSeedValuesXlsSettingsGlasses()
    {
        return new XlsSettingsGlassesDTO()
        {
            Id = 1,
            Created = DateTime.MinValue,
            LastModified = DateTime.MinValue,
            IsSelected = true,
            SettingsName = "DefaultSettings",
            WorksheetName = "GlassesOrderSheet",
            FontSize = 11,
            IsFontBold = true,
            FontFamily = "Calibri",
            FontColorA = 255,
            FontColorR = 68,
            FontColorG = 84,
            FontColorB = 106,
            NormalRowHeight = 19.5d,
            TotalRows = 2000,
            FirstRowIndex = 1,
            FirstColumnIndex = 1,
            HorizontalAlignment = "Center",
            VerticalAlignment = "Center",
            TitlesRowHeight = 20d,
            TitlesBackgroundColorA = 255,
            TitlesBackgroundColorR = 68,
            TitlesBackgroundColorG = 114,
            TitlesBackgroundColorB = 196,
            TitlesFontColorA = 255,
            TitlesFontColorR = 255,
            TitlesFontColorG = 255,
            TitlesFontColorB = 255,
            IsTitlesFontBold = true,

            //Table Header Settings
            TableHeaderRowHeight = 39.75d,
            TableHeaderFontSize = 16,
            TableHeaderIsFontBold = false,
            TableHeaderFontColorA = 255,
            TableHeaderFontColorR = 0,
            TableHeaderFontColorG = 0,
            TableHeaderFontColorB = 0,
            TableHeaderBackgroundColorA = 255,
            TableHeaderBackgroundColorR = 231,
            TableHeaderBackgroundColorG = 230,
            TableHeaderBackgroundColorB = 230,
            TableHeaderHorizontalAlignment = "Left",
            TableHeaderVerticalAlignment = "Center",

            //Glass Table Settings
            GlassTableHorizontalBorderThickness = "Medium",
            GlassTableHorizontalBorderColorA = 255,
            GlassTableHorizontalBorderColorR = 142,
            GlassTableHorizontalBorderColorG = 169,
            GlassTableHorizontalBorderColorB = 219,
            GlassTableVerticalBorderThickness = "None",
            GlassTableVerticalBorderColorA = 255,
            GlassTableVerticalBorderColorR = 142,
            GlassTableVerticalBorderColorG = 169,
            GlassTableVerticalBorderColorB = 219,
            GlassTableAlternatingRowTableBackgroundColorA = 255,
            GlassTableAlternatingRowTableBackgroundColorR = 242,
            GlassTableAlternatingRowTableBackgroundColorG = 242,
            GlassTableAlternatingRowTableBackgroundColorB = 242,
            GlassTablePerimetricalBorderThickness = "Medium",
            GlassTablePerimetricalBorderColorA = 255,
            GlassTablePerimetricalBorderColorR = 0,
            GlassTablePerimetricalBorderColorG = 0,
            GlassTablePerimetricalBorderColorB = 0,

            //GENERAL HEADER
            GeneralHeaderRowHeight = 39.75d,
            GeneralHeaderFontSize = 22,
            GeneralHeaderIsFontBold = true,
            GeneralHeaderHorizontalAlignment = "Center",
            GeneralHeaderVerticalAlignment = "Center",
            GeneralHeaderFontColorA = 255,
            GeneralHeaderFontColorR = 0,
            GeneralHeaderFontColorG = 0,
            GeneralHeaderFontColorB = 0,
            GeneralHeaderBackgroundColorA = 0,
            GeneralHeaderBackgroundColorR = 231,
            GeneralHeaderBackgroundColorG = 230,
            GeneralHeaderBackgroundColorB = 230,

            //Total Glaases Box Settings
            TotalGlassesBoxHorizontalAlignment = "Center",
            TotalGlassesBoxVerticalAlignment = "Center",
            TotalGlassesBoxFontSize = 16,
            TotalGlassesBoxIsFontBold = true,
            TotalGlassesBoxFontColorA = 255,
            TotalGlassesBoxFontColorR = 0,
            TotalGlassesBoxFontColorG = 0,
            TotalGlassesBoxFontColorB = 0,
            TotalGlassesBoxBackgroundColorA = 255,
            TotalGlassesBoxBackgroundColorR = 242,
            TotalGlassesBoxBackgroundColorG = 242,
            TotalGlassesBoxBackgroundColorB = 242,

            //Notes Settings
            NotesHorizontalAlignment = "Center",
            NotesVerticalAlignment = "Center",
            NotesFontSize = 12,
            NotesIsFontBold = true,
            NotesFontColorA = 255,
            NotesFontColorR = 0,
            NotesFontColorG = 0,
            NotesFontColorB = 0,
            NotesBackgroundColorA = 255,
            NotesBackgroundColorR = 242,
            NotesBackgroundColorG = 242,
            NotesBackgroundColorB = 242,
            NumberOfRowsForNotes = 5,
        };
    }
    /// <summary>
    /// The Default/Seed Values of the SearchOrdersViewSettings
    /// </summary>
    /// <returns></returns>
    private SearchOrdersViewSettingsDTO[] GetSeedValuesSearchOrdersViewSettings()
    {
        return new SearchOrdersViewSettingsDTO[]{
            //A Row to Store Defaults
            new SearchOrdersViewSettingsDTO()
            {
                Id = 1,
                Created = DateTime.Now,
                LastModified = DateTime.MinValue,
                InfoStringObject = "DefaultSettings",
                IsDefault = true,
                MaxResultsGetSmallOrders = 10,
            },
            //A Row used by the User in an application
            new SearchOrdersViewSettingsDTO()
            {
                Id = 2,
                Created = DateTime.Now,
                LastModified = DateTime.MinValue,
                InfoStringObject = "UserSettings",
                IsDefault = false,
                MaxResultsGetSmallOrders = 10,
            },
        };
    }
    /// <summary>
    /// Generates the Default Starting Settings for the GlassesStockService Comparer
    /// </summary>
    /// <returns></returns>
    private GlassesStockServiceSettingsDTO[] GetSeedValuesGlassesStockServiceSettings()
    {
        IEnumerable<CabinModelEnum> models = Enum.GetValues(typeof(CabinModelEnum)).Cast<CabinModelEnum>();
        List<GlassesStockServiceSettingsDTO> settings = new();
        //Create defaults and User Settings , one for each model
        int incrementor = 1;
        foreach (var model in models)
        {
            var settingDefault = new GlassesStockServiceSettingsDTO()
            {
                Id = incrementor,
                Created = DateTime.Now,
                LastModified = DateTime.MinValue,
                InfoStringObject = $"DefaultSettings-{model}",
                IsDefault = true,
                AllowedHeightDifference = 50d,
                AllowedLengthDifference = 50d,
                IsSelected = false,
                ShouldCompareFinish = true,
                ShouldCompareHeight = true,
                ShouldCompareLength = true,
                ShouldCompareThickness = false,
                ConcernsModel = model,
            };
            incrementor++;
            var settingUser = new GlassesStockServiceSettingsDTO()
            {
                Id = incrementor,
                Created = DateTime.Now,
                LastModified = DateTime.MinValue,
                InfoStringObject = $"UserSettings-{model}",
                IsDefault = false,
                AllowedHeightDifference = 50d,
                AllowedLengthDifference = 50d,
                IsSelected = true,
                ShouldCompareFinish = true,
                ShouldCompareHeight = true,
                ShouldCompareLength = true,
                ShouldCompareThickness = false,
                ConcernsModel = model,
            };
            settings.Add(settingDefault);
            settings.Add(settingUser);
            incrementor++;
        }
        return settings.ToArray();
    }

    #region DEPRECATED

    #region DEPRECATED ON MODEL CREATING WHEN WE NEED TO ADD DICTIONARIES ETC
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{

    //    //var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);

    //    //Define that Settings has Many Columns and each Column is owned by one setting
    //    //Even if we omit this Ef-Core will Use scafold it , but its good to be here so we can remember!!

    //    //modelBuilder.Entity<XlsSettingsGlassesDTO>().HasMany(s => s.ColumnNames).WithOne();

    //    #region JSON SERILIZING INTO DB ekseretiko
    //    //Register the Properties that are Dictionaries to be Serilized as Json Strings inside Database
    //    //Must Declare how to Serilize and Deserilize them
    //    //The Values are retrieved altogether , no single value is chosen

    //    modelBuilder.Entity<XlsSettingsGlassesDTO>()
    //        .Property(s => s.DrawsDescriptions)
    //        .HasColumnName("DrawsDescriptions")
    //        .HasColumnType("BLOB")
    //        .HasConversion(
    //            //How to Serilize the Dictionary of Draws Descriptions => into json string
    //            drawsDescriptions => JsonSerializer.Serialize(drawsDescriptions, jsonOptions),
    //            //How to retrieve the json string from the Db
    //            jsonString => JsonSerializer.Deserialize<Dictionary<GlassDrawEnum, string>>(jsonString, jsonOptions)!,
    //            //Not sure why a comparer is needed ...
    //            ValueComparer.CreateDefault(typeof(Dictionary<GlassDrawEnum, string>), true)
    //            );

    //    modelBuilder.Entity<XlsSettingsGlassesDTO>()
    //        .Property(s => s.ThicknessDescriptions)
    //        .HasColumnName("ThicknessDescriptions")
    //        .HasColumnType("BLOB")
    //        .HasConversion(
    //            //How to Serilize the Dictionary of Draws Descriptions => into json string
    //            thicknessDescriptions => JsonSerializer.Serialize(thicknessDescriptions, jsonOptions),
    //            //How to retrieve the json string from the Db
    //            jsonString => JsonSerializer.Deserialize<Dictionary<GlassThicknessEnum, string>>(jsonString, jsonOptions)!,
    //            ValueComparer.CreateDefault(typeof(Dictionary<GlassThicknessEnum, string>), true)
    //            );

    //    modelBuilder.Entity<XlsSettingsGlassesDTO>()
    //        .Property(s => s.FinishDescriptionsGR)
    //        .HasColumnName("FinishDescriptionsGR")
    //        .HasColumnType("BLOB")
    //        .HasConversion(
    //            //How to Serilize the Dictionary of Draws Descriptions => into json string
    //            finishDescriptionsGR => JsonSerializer.Serialize(finishDescriptionsGR, jsonOptions),
    //            //How to retrieve the json string from the Db
    //            jsonString => JsonSerializer.Deserialize<Dictionary<GlassFinishEnum, string>>(jsonString, jsonOptions)!,
    //            ValueComparer.CreateDefault(typeof(Dictionary<GlassFinishEnum, string>), true)
    //            );

    //    modelBuilder.Entity<XlsSettingsGlassesDTO>()
    //        .Property(s => s.FinishDescriptionsEN)
    //        .HasColumnName("FinishDescriptionsEN")
    //        .HasColumnType("BLOB")
    //        .HasConversion(
    //            //How to Serilize the Dictionary of Draws Descriptions => into json string
    //            finishDescriptionsEN => JsonSerializer.Serialize(finishDescriptionsEN, jsonOptions),
    //            //How to retrieve the json string from the Db
    //            jsonString => JsonSerializer.Deserialize<Dictionary<GlassFinishEnum, string>>(jsonString, jsonOptions)!,
    //            ValueComparer.CreateDefault(typeof(Dictionary<GlassFinishEnum, string>), true)
    //            );
    //    modelBuilder.Entity<XlsSettingsGlassesDTO>()
    //        .Property(s => s.HolesDescriptionsGR)
    //        .HasColumnName("HolesDescriptionsGR")
    //        .HasColumnType("BLOB")
    //        .HasConversion(
    //            //How to Serilize the Dictionary of Draws Descriptions => into json string
    //            holesDescriptionsGR => JsonSerializer.Serialize(holesDescriptionsGR, jsonOptions),
    //            //How to retrieve the json string from the Db
    //            jsonString => JsonSerializer.Deserialize<Dictionary<GlassDrawEnum, string>>(jsonString, jsonOptions)!,
    //            ValueComparer.CreateDefault(typeof(Dictionary<GlassDrawEnum, string>), true)
    //            );
    //    modelBuilder.Entity<XlsSettingsGlassesDTO>()
    //        .Property(s => s.HolesDescriptionsEN)
    //        .HasColumnName("HolesDescriptionsEN")
    //        .HasColumnType("BLOB")
    //        .HasConversion(
    //            //How to Serilize the Dictionary of Draws Descriptions => into json string
    //            holesDescriptionsEN => JsonSerializer.Serialize(holesDescriptionsEN, jsonOptions),
    //            //How to retrieve the json string from the Db
    //            jsonString => JsonSerializer.Deserialize<Dictionary<GlassDrawEnum, string>>(jsonString, jsonOptions)!,
    //            ValueComparer.CreateDefault(typeof(Dictionary<GlassDrawEnum, string>), true)
    //            );
    //    #endregion

    //    //Inserts Values To Exist when creating the Database ,
    //    //modelBuilder.Entity<XlsSettingsGlassesDTO>().HasData( GetSeedValuesXlsSettingsGlasses() );

    //    //The Foreign key must be mentioned (Because the object does not have such a key , we create an anonymous object NamingConvention is the OwnersName followed by Id)
    //    //modelBuilder.Entity<ColumnNameDTO>().HasData(
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 1, NameGR = "A/A", NameEN = "Line No", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 2, NameGR = "Σχέδιο", NameEN = "Draw", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 3, NameGR = "Μήκος(mm)", NameEN = "Length(mm)", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 4, NameGR = "Ύψος(mm)", NameEN = "Height(mm)", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 5, NameGR = "Πάχος(mm)", NameEN = "Thickness(mm)", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 6, NameGR = "Τύπος", NameEN = "Type", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 7, NameGR = "CUT Μήκος(mm)", NameEN = "CUT Length(mm)", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 8, NameGR = "CUT Ύψος(mm)", NameEN = "CUT Height(mm)", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 9, NameGR = "Τεμάχια", NameEN = "Pieces", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 10, NameGR = "BronzeUse PA0", NameEN = "BronzeUse PA0", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 11, NameGR = "BronzeUse Item", NameEN = "BronzeUse Item", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 12, NameGR = "Σημειώσεις", NameEN = "Notes", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" },
    //    //            new { XlsSettingsGlassesDTOId = 1, Id = 13, NameGR = "Τιμή EUR", NameEN = "Price EUR", Created = DateTime.MinValue, LastModified = DateTime.MinValue, InfoStringObject = "" }
    //    //    );

    //    base.OnModelCreating(modelBuilder); 

    //}
    #endregion

    //public static Dictionary<GlassDrawEnum, string> GetDefaultDrawDescriptions()
    //{
    //    return new Dictionary<GlassDrawEnum, string>()
    //    {
    //        { GlassDrawEnum.Draw9S ,    "9S"},
    //        { GlassDrawEnum.DrawF,      "F"},
    //        { GlassDrawEnum.Draw9B,     "94"},
    //        { GlassDrawEnum.DrawVS,     "VS"},
    //        { GlassDrawEnum.DrawVA,     "VA"},
    //        { GlassDrawEnum.DrawVF,     "VF"},
    //        { GlassDrawEnum.DrawHB1,    "HB1"},
    //        { GlassDrawEnum.DrawHB2,    "HB2"},
    //        { GlassDrawEnum.DrawDP1,    "DP1"},
    //        { GlassDrawEnum.DrawDP3,    "DP3"},
    //        { GlassDrawEnum.DrawNB ,    "NB"},
    //        { GlassDrawEnum.DrawDB ,    "DB"},
    //        { GlassDrawEnum.DrawWS ,    "WS"},
    //        { GlassDrawEnum.DrawH1 ,    "H1"},
    //        { GlassDrawEnum.DrawFL ,    "FL"},
    //        { GlassDrawEnum.Draw9C ,    "9C"},
    //    };
    //}
    //public static Dictionary<GlassThicknessEnum, string> GetDefaultThicknessDescriptions()
    //{
    //    return new Dictionary<GlassThicknessEnum, string>()
    //    {
    //        {GlassThicknessEnum.Thick5mm , "5mm" },
    //        {GlassThicknessEnum.Thick6mm , "6mm" },
    //        {GlassThicknessEnum.Thick8mm , "8mm" },
    //        {GlassThicknessEnum.Thick10mm, "10mm" },
    //        {GlassThicknessEnum.ThickTenplex10mm , "10mm Tenplex" },
    //        {GlassThicknessEnum.GlassThicknessNotSet , "Undefined" }
    //    };
    //}
    //public static Dictionary<GlassDrawEnum, string> GetDefaultHoleDescriptionsGR()
    //{
    //    return new Dictionary<GlassDrawEnum, string>()
    //    {
    //        { GlassDrawEnum.Draw9S ,    "10-Τρύπες--Φ10mm" },
    //        { GlassDrawEnum.DrawF,      "Χωρίς Τρύπες" },
    //        { GlassDrawEnum.Draw9B,     "4-Τρύπες--Φ10mm & 2-Κοψίματα" },
    //        { GlassDrawEnum.DrawVS,     "2-Τρύπες--Φ16mm & 2-Τρύπες--Φ10mm & 1-Τρύπα--Φ50mm"},
    //        { GlassDrawEnum.DrawVA,     "2-Τρύπες--Φ20mm & 2-Τρύπες Φ14mm" },
    //        { GlassDrawEnum.DrawVF,     "1-Τρύπα--Φ14mm & 2-Τρύπες Φ25mm" },
    //        { GlassDrawEnum.DrawHB1,    "2-CUT--Μεντεσέ"},
    //        { GlassDrawEnum.DrawHB2,    "2-CUT--Μεντεσέ & 1-Τρύπα--Φ12mm" },
    //        { GlassDrawEnum.DrawDP1,    "2-Φρεζάτες--Φ24/Φ32mm & 1-Τρύπα--Φ12mm"},
    //        { GlassDrawEnum.DrawDP3,    "2-Φρεζάτες--Φ24/Φ32mm"},
    //        { GlassDrawEnum.DrawNB ,    "1-Τρύπα--Φ10mm"},
    //        { GlassDrawEnum.DrawDB ,    "2-DG--CUT & 1-Τρύπα--Φ10mm"},
    //        { GlassDrawEnum.DrawWS ,    "2-Τρύπες--Φ15mm & 1-Τρύπα--Φ50mm"},
    //        { GlassDrawEnum.DrawH1 ,    "2-Τρύπες--Φ20mm"},
    //        { GlassDrawEnum.DrawFL ,    "??-Tρυπες--Φ??mm" },
    //        { GlassDrawEnum.Draw9C ,    "??-Τρυπες--Φ??mm"},
    //    };
    //}
    //public static Dictionary<GlassDrawEnum, string> GetDefaultHoleDescriptionsEN()
    //{
    //    return new Dictionary<GlassDrawEnum, string>()
    //    {
    //        { GlassDrawEnum.Draw9S ,    "10-Holes--Φ10mm" },
    //        { GlassDrawEnum.DrawF,      "Without Holes" },
    //        { GlassDrawEnum.Draw9B,     "4-Holes--Φ10mm & 2-CUTS" },
    //        { GlassDrawEnum.DrawVS,     "2-Holes--Φ16mm & 2-Holes--Φ10mm & 1-Hole--Φ50mm"},
    //        { GlassDrawEnum.DrawVA,     "2-Holes--Φ20mm & 2-Holes Φ14mm" },
    //        { GlassDrawEnum.DrawVF,     "1-Hole--Φ14mm & 2-HolesΦ25mm" },
    //        { GlassDrawEnum.DrawHB1,    "2-CUT--Hinge"},
    //        { GlassDrawEnum.DrawHB2,    "2-CUT--Hinge & 1-Hole--Φ12mm" },
    //        { GlassDrawEnum.DrawDP1,    "2-Conical Holes--Φ24/Φ32mm & 1-Hole--Φ12mm"},
    //        { GlassDrawEnum.DrawDP3,    "2-Conical Holes--Φ24/Φ32mm"},
    //        { GlassDrawEnum.DrawNB ,    "1-Hole--Φ10mm"},
    //        { GlassDrawEnum.DrawDB ,    "2-DG--CUT & 1-Hole--Φ10mm"},
    //        { GlassDrawEnum.DrawWS ,    "2-Holes--Φ15mm & 1-Hole--Φ50mm"},
    //        { GlassDrawEnum.DrawH1 ,    "2-Holes--Φ20mm"},
    //        { GlassDrawEnum.DrawFL ,    "??-Holes--Φ??mm" },
    //        { GlassDrawEnum.Draw9C ,    "??-Holes--Φ??mm"},
    //    };
    //}
    //public static Dictionary<GlassFinishEnum, string> GetDefaultFinishDescriptionsGR()
    //{
    //    return new Dictionary<GlassFinishEnum, string>()
    //    {
    //        {GlassFinishEnum.Fume , "Φιμέ" },
    //        {GlassFinishEnum.Frosted, "Frosted" },
    //        {GlassFinishEnum.GlassFinishNotSet , "Undefined" },
    //        {GlassFinishEnum.Satin , "Σατινέ" },
    //        {GlassFinishEnum.Serigraphy, "Σεριγραφεία" },
    //        {GlassFinishEnum.Transparent, "Διάφανο" },
    //        {GlassFinishEnum.Special , "Ειδικό" }
    //    };
    //}
    //public static Dictionary<GlassFinishEnum, string> GetDefaultFinishDescriptionsEN()
    //{
    //    return new Dictionary<GlassFinishEnum, string>()
    //    {
    //        {GlassFinishEnum.Fume , "Fume" },
    //        {GlassFinishEnum.Frosted, "Frosted" },
    //        {GlassFinishEnum.GlassFinishNotSet , "Undefined" },
    //        {GlassFinishEnum.Satin , "Satin" },
    //        {GlassFinishEnum.Serigraphy, "Serigraphy" },
    //        {GlassFinishEnum.Transparent, "Transparent" },
    //        {GlassFinishEnum.Special , "Special" }
    //    };
    //} 

    #endregion

}

