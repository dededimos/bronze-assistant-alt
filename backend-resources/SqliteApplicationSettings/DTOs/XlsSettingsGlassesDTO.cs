using ShowerEnclosuresModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteApplicationSettings.DTOs
{
    public class XlsSettingsGlassesDTO : DTO
    {
        /// <summary>
        /// Weather this setting is Selected
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// The Custom Name Given to the Setting
        /// </summary>
        public string SettingsName { get; set; } = string.Empty;
        
        /// <summary>
        /// The Name of the Generated Worksheet
        /// </summary>
        public string WorksheetName { get; set; } = string.Empty;
        /// <summary>
        /// The General Font Size
        /// </summary>
        public double FontSize { get; set; }
        /// <summary>
        /// Weather the Font is Bold
        /// </summary>
        public bool IsFontBold { get; set; }
        public string FontFamily { get; set; } = string.Empty;
        public int FontColorA { get; set; }
        public int FontColorR { get; set; }
        public int FontColorG { get; set; }
        public int FontColorB { get; set; }
        public double NormalRowHeight { get; set; }
        public int TotalRows { get; set; }
        public int FirstRowIndex { get; set; }
        public int FirstColumnIndex { get; set; }
        public string HorizontalAlignment { get; set; } = string.Empty;
        public string VerticalAlignment { get; set; } = string.Empty;
        public double TitlesRowHeight { get; set; }
        public  int TitlesBackgroundColorA { get; set; }
        public int TitlesBackgroundColorR { get; set; }
        public int TitlesBackgroundColorG { get; set; }
        public int TitlesBackgroundColorB { get; set; }
        public int TitlesFontColorA { get; set; }
        public int TitlesFontColorR { get; set; }
        public int TitlesFontColorG { get; set; }
        public int TitlesFontColorB { get; set; }
        public bool IsTitlesFontBold { get; set; }

        //TABLE HEADER SETTINGS
        public double   TableHeaderRowHeight { get; set; }
        public double   TableHeaderFontSize { get; set; }
        public bool     TableHeaderIsFontBold { get; set; }
        public int      TableHeaderFontColorA { get; set; }
        public int      TableHeaderFontColorR { get; set; }
        public int      TableHeaderFontColorG { get; set; }
        public int      TableHeaderFontColorB { get; set; }
        public int      TableHeaderBackgroundColorA { get; set; }
        public int      TableHeaderBackgroundColorR { get; set; }
        public int      TableHeaderBackgroundColorG { get; set; }
        public int      TableHeaderBackgroundColorB { get; set; }
        public string   TableHeaderHorizontalAlignment { get; set; } = string.Empty;
        public string   TableHeaderVerticalAlignment { get; set; } = string.Empty;

        //GLASS TABLE SETTINGS
        public string GlassTableHorizontalBorderThickness { get; set; } = string.Empty;
        public int GlassTableHorizontalBorderColorA { get; set; }
        public int GlassTableHorizontalBorderColorR { get; set; }
        public int GlassTableHorizontalBorderColorG { get; set; }
        public int GlassTableHorizontalBorderColorB { get; set; }
        public string GlassTableVerticalBorderThickness { get; set; } = string.Empty;
        public int GlassTableVerticalBorderColorA { get; set; }
        public int GlassTableVerticalBorderColorR { get; set; }
        public int GlassTableVerticalBorderColorG { get; set; }
        public int GlassTableVerticalBorderColorB { get; set; }
        public int GlassTableAlternatingRowTableBackgroundColorA { get; set; }
        public int GlassTableAlternatingRowTableBackgroundColorR { get; set; }
        public int GlassTableAlternatingRowTableBackgroundColorG { get; set; }
        public int GlassTableAlternatingRowTableBackgroundColorB { get; set; }
        public string GlassTablePerimetricalBorderThickness { get; set; } = string.Empty;
        public int GlassTablePerimetricalBorderColorA { get; set; }
        public int GlassTablePerimetricalBorderColorR { get; set; }
        public int GlassTablePerimetricalBorderColorG { get; set; }
        public int GlassTablePerimetricalBorderColorB { get; set; }

        //General Header Settings
        public double GeneralHeaderRowHeight { get; set; }
        public double GeneralHeaderFontSize { get; set; }
        public bool GeneralHeaderIsFontBold { get; set; }
        public string GeneralHeaderHorizontalAlignment { get; set; } = string.Empty;
        public string GeneralHeaderVerticalAlignment { get; set; } = string.Empty;
        public int GeneralHeaderFontColorA { get; set; }
        public int GeneralHeaderFontColorR { get; set; }
        public int GeneralHeaderFontColorG { get; set; }
        public int GeneralHeaderFontColorB { get; set; }
        public int GeneralHeaderBackgroundColorA { get; set; }
        public int GeneralHeaderBackgroundColorR { get; set; }
        public int GeneralHeaderBackgroundColorG { get; set; }
        public int GeneralHeaderBackgroundColorB { get; set; }

        //TotalGlasses Box Settings
        public string TotalGlassesBoxHorizontalAlignment { get; set; } = string.Empty;
        public string TotalGlassesBoxVerticalAlignment { get; set; } = string.Empty;
        public double TotalGlassesBoxFontSize { get; set; }
        public bool TotalGlassesBoxIsFontBold { get; set; }
        public int TotalGlassesBoxFontColorA { get; set; }
        public int TotalGlassesBoxFontColorR { get; set; }
        public int TotalGlassesBoxFontColorG { get; set; }
        public int TotalGlassesBoxFontColorB { get; set; }
        public int TotalGlassesBoxBackgroundColorA { get; set; }
        public int TotalGlassesBoxBackgroundColorR { get; set; }
        public int TotalGlassesBoxBackgroundColorG { get; set; }
        public int TotalGlassesBoxBackgroundColorB { get; set; }

        //NotesBox Settings
        public string NotesHorizontalAlignment { get; set; } = string.Empty;
        public string NotesVerticalAlignment { get; set; } = string.Empty;
        public double NotesFontSize { get; set;}
        public bool NotesIsFontBold { get; set; }
        public int NotesFontColorA { get; set; }
        public int NotesFontColorR { get; set; }
        public int NotesFontColorG { get; set; }
        public int NotesFontColorB { get; set; }
        public int NotesBackgroundColorA { get; set; }
        public int NotesBackgroundColorR { get; set; }
        public int NotesBackgroundColorG { get; set; }
        public int NotesBackgroundColorB { get; set; }
        public int NumberOfRowsForNotes { get; set; }



    }


    #region DEPRECATED DICTIONARIES

    #region DEPRECATED DICTIONARIES
    //DEPRECATED MAYBE WE ADD THEM IN ANOTHER WAY ITS TOO COMPLICATED FOR NO ACTUAL BENEFIT IN THE USER
    //BUILT IN AS DEFAULTS IN THE APP
    //public List<ColumnNameDTO> ColumnNames { get; set; } = new();
    //public Dictionary<GlassDrawEnum, string> DrawsDescriptions { get; set; } = new();
    //public Dictionary<GlassThicknessEnum, string> ThicknessDescriptions { get; set; } = new();
    //public Dictionary<GlassFinishEnum, string> FinishDescriptionsGR { get; set; } = new();
    //public Dictionary<GlassFinishEnum, string> FinishDescriptionsEN { get; set; } = new();
    //public Dictionary<GlassDrawEnum, string> HolesDescriptionsGR { get; set; } = new();
    //public Dictionary<GlassDrawEnum, string> HolesDescriptionsEN { get; set; } = new(); 
    #endregion
    //public class ColumnNameDTO : DTO 
    //{
    //    public string NameGR { get; set; } = "N/A";
    //    public string NameEN { get; set; } = "N/A";
    //}

    //DEPRECATED MAYBE WE ADD THEM IN ANOTHER WAY ITS TOO COMPLICATED FOR NO ACTUAL BENEFIT IN THE USER
    //BUILT IN AS DEFAULTS IN THE APP
    //public class XlsGlassDescriptionsDTO
    //{
    //    public Dictionary<GlassDrawEnum, string> DrawsDescriptions { get; set; } = new();
    //    public Dictionary<GlassThicknessEnum, string> ThicknessDescriptions { get; set; } = new();
    //    public Dictionary<GlassFinishEnum, string> FinishDescriptionsGR { get; set; } = new();
    //    public Dictionary<GlassFinishEnum, string> FinishDescriptionsEN { get; set; } = new();
    //    public Dictionary<GlassDrawEnum, string> HolesDescriptionsGR { get; set; } = new();
    //    public Dictionary<GlassDrawEnum, string> HolesDescriptionsEN { get; set; } = new();
    //} 
    #endregion


}
