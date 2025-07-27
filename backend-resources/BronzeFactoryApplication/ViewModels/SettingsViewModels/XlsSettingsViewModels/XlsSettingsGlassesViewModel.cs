using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels
{
    public partial class XlsSettingsGlassesViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool isNewSetting;
        [ObservableProperty]
        private int id;
        [ObservableProperty]
        private string settingsName = string.Empty;
        [ObservableProperty]
        private bool isSelected;
        /// <summary>
        /// The Worksheet Name
        /// </summary>
        [ObservableProperty]
        private string worksheetName = string.Empty;
        /// <summary>
        /// The Font Size
        /// </summary>
        [ObservableProperty]
        private double fontSize ;
        /// <summary>
        /// Weather the Sheet Main Font will be Bold or Not
        /// </summary>
        [ObservableProperty]
        private bool isFontBold ;
        /// <summary>
        /// The Font Family to be Used by the Excel Document
        /// </summary>
        [ObservableProperty]
        private string fontFamily = string.Empty;
        /// <summary>
        /// The Color of the Fonts (The Default is 68,84,106 as the Initial First Excel Made)
        /// </summary>
        [ObservableProperty]
        private System.Windows.Media.Brush fontColor = new SolidColorBrush();
        /// <summary>
        /// The Height of each NormalRow
        /// </summary>
        [ObservableProperty]
        private double normalRowHeight ;
        
        /// <summary>
        /// The Total Columns that will be Used , Default for Glasses is 13
        /// </summary>
        public int TotalColumns { get => ColumnsTitles.Count; }
        
        /// <summary>
        /// The Maximum Number of Rows that might be Used , Just a big Number as Default
        /// </summary>
        [ObservableProperty]
        private int totalRows ;
        /// <summary>
        /// The Index of the First Row to be Used
        /// </summary>
        [ObservableProperty]
        private int firstRowIndex ;
        /// <summary>
        /// The Index of the First Column to be Used
        /// </summary>
        [ObservableProperty]
        private int firstColumnIndex ;
        
        /// <summary>
        /// The Last Row Index Number
        /// </summary>
        public int LastRowIndex { get => TotalRows + FirstRowIndex; }
        /// <summary>
        /// The Last Column Index Number
        /// </summary>
        public int LastColumnIndex { get => TotalColumns + FirstColumnIndex; }

        /// <summary>
        /// The Horizontal Alignment of the Cells
        /// </summary>
        [ObservableProperty]
        private XLAlignmentHorizontalValues horizontalAlignment ;
        /// <summary>
        /// The Vertical Alignment of the Cells
        /// </summary>
        [ObservableProperty]
        private XLAlignmentVerticalValues verticalAlignment ;

        /// <summary>
        /// The Titles of the Used Columns , in Order
        /// </summary>
        public ObservableCollection<string> ColumnsTitles { get; private set; } = new();
        
        /// <summary>
        /// The Row Height of the Titles in the Glasses Table
        /// </summary>
        [ObservableProperty]
        private double titleRowHeight ;
        /// <summary>
        /// The Background Color of the Titles
        /// </summary>
        [ObservableProperty]
        private System.Windows.Media.Brush titleBackgroundColor = new SolidColorBrush();
        /// <summary>
        /// The Font Color of the Titles
        /// </summary>
        [ObservableProperty]
        private System.Windows.Media.Brush titleFontColor = new SolidColorBrush();
        /// <summary>
        /// Weather the Titles are in Blod Font
        /// </summary>
        [ObservableProperty]
        private bool isTitleFontBold ;

        //Tables Headers
        [ObservableProperty]
        private double tablesHeaderRowHeight;
        [ObservableProperty]
        private double tablesFontSize;
        [ObservableProperty]
        private bool tablesIsFontBold;
        [ObservableProperty]
        private System.Windows.Media.Brush tablesFontColor = new SolidColorBrush();
        [ObservableProperty]
        private System.Windows.Media.Brush tablesBackgroundColor = new SolidColorBrush();
        [ObservableProperty]
        private XLAlignmentHorizontalValues tablesHorizontalAlignment;
        [ObservableProperty]
        private XLAlignmentVerticalValues tablesVerticalAlignment;

        //GlassTablesVarious
        [ObservableProperty]
        private XLBorderStyleValues tablesHorizontalBorderThickness;
        [ObservableProperty]
        private XLBorderStyleValues tablesVerticalBorderThickness;
        [ObservableProperty]
        private XLBorderStyleValues tablesPerimetricalBorderThickness;
        [ObservableProperty]
        private System.Windows.Media.Brush tablesHorizontalBorderColor = new SolidColorBrush();
        [ObservableProperty]
        private System.Windows.Media.Brush tablesVerticalBorderColor = new SolidColorBrush();
        [ObservableProperty]
        private System.Windows.Media.Brush alternatingTableRowBackground = new SolidColorBrush();
        [ObservableProperty]
        private System.Windows.Media.Brush tablesPerimetricalBorderColor = new SolidColorBrush();

        //General Header
        [ObservableProperty]
        private double generalHeaderRowHeight;
        [ObservableProperty]
        private double generalHeaderFontSize;
        [ObservableProperty]
        private bool generalHeaderIsFontBold;
        [ObservableProperty]
        private XLAlignmentHorizontalValues generalHeaderHorizontalAlignment;
        [ObservableProperty]
        private XLAlignmentVerticalValues generalHeaderVerticalAlignment;
        [ObservableProperty]
        private System.Windows.Media.Brush generalHeaderFontColor = new SolidColorBrush();
        [ObservableProperty]
        private System.Windows.Media.Brush generalHeaderBackgroundColor = new SolidColorBrush();

        //Glass Box Settings
        [ObservableProperty]
        private XLAlignmentHorizontalValues glassBoxHorizontalAlignment;
        [ObservableProperty]
        private XLAlignmentVerticalValues glassBoxVerticalAlignment;
        [ObservableProperty]
        private System.Windows.Media.Brush glassBoxBackgroundColor = new SolidColorBrush();
        [ObservableProperty]
        private double glassBoxFontSize;
        [ObservableProperty]
        private bool glassBoxIsFontBold;
        [ObservableProperty]
        private System.Windows.Media.Brush glassBoxFontColor = new SolidColorBrush();

        //Notes Box
        [ObservableProperty]
        private XLAlignmentHorizontalValues notesHorizontalAlignment;
        [ObservableProperty]
        private XLAlignmentVerticalValues notesVerticalAlignment;
        [ObservableProperty]
        private System.Windows.Media.Brush notesBackgroundColor = new SolidColorBrush();
        [ObservableProperty]
        private double notesFontSize;
        [ObservableProperty]
        private bool notesIsFontBold;
        [ObservableProperty]
        private System.Windows.Media.Brush notesFontColor = new SolidColorBrush();
        [ObservableProperty]
        private int numberOfRowsForNotes;


        public XlsSettingsGlassesViewModel()
        {
            
        }

        /// <summary>
        /// Sets new Settings to the ViewModel
        /// </summary>
        /// <param name="s"></param>
        /// /// <param name="isNewSetting">Weather this is a new Setting</param>
        public void SetSettings(XlsSettingsGlasses s , bool isNewSetting = false)
        {
            IsNewSetting = isNewSetting;
            Id = s.Id;
            IsSelected = s.IsSelected;
            SettingsName = s.SettingsName;
            WorksheetName = s.WorksheetName;
            FontSize = s.FontSize;
            IsFontBold = s.IsFontBold;
            FontFamily = s.FontFamily;
            FontColor = new SolidColorBrush(s.FontColor.ToMediaColor());
            NormalRowHeight = s.NormalRowHeight;
            TotalRows = s.TotalRows;
            FirstRowIndex = s.FirstRowIndex;
            FirstColumnIndex = s.FirstColumnIndex;
            HorizontalAlignment = s.HorizontalAlignment;
            VerticalAlignment = s.VerticalAlignment;
            TitleRowHeight = s.TitlesRowHeight;
            TitleBackgroundColor = new SolidColorBrush(s.TitlesBackgroundColor.ToMediaColor());
            TitleFontColor = new SolidColorBrush(s.TitlesFontColor.ToMediaColor());
            IsTitleFontBold = s.IsTitlesFontBold;

            // Tables
            TablesHeaderRowHeight = s.TableHeaderSettings.TableHeaderRowHeight;
            TablesFontSize = s.TableHeaderSettings.FontSize;
            TablesIsFontBold = s.TableHeaderSettings.IsFontBold;
            TablesFontColor = new SolidColorBrush(s.TableHeaderSettings.FontColor.ToMediaColor());
            TablesBackgroundColor = new SolidColorBrush(s.TableHeaderSettings.BackgroundColor.ToMediaColor());
            TablesHorizontalAlignment = s.TableHeaderSettings.HorizontalAlignment;
            TablesVerticalAlignment = s.TableHeaderSettings.VerticalAlignment;
            // Glass Tables
            TablesHorizontalBorderColor = new SolidColorBrush(s.GlassTableSettings.HorizontalBorderColor.ToMediaColor());
            TablesVerticalBorderColor = new SolidColorBrush(s.GlassTableSettings.VerticalBorderColor.ToMediaColor());
            TablesHorizontalBorderThickness = s.GlassTableSettings.HorizontalBorderThickness;
            TablesVerticalBorderThickness = s.GlassTableSettings.VerticalBorderThickness;
            AlternatingTableRowBackground = new SolidColorBrush(s.GlassTableSettings.AlternatingTableRowBackground.ToMediaColor());
            TablesPerimetricalBorderThickness = s.GlassTableSettings.TablePerimetricalBorderThickness;
            TablesPerimetricalBorderColor = new SolidColorBrush(s.GlassTableSettings.TablePerimetricalBorderColor.ToMediaColor());
            // General Header
            GeneralHeaderRowHeight = s.GeneralHeaderSettings.RowHeight;
            GeneralHeaderFontSize = s.GeneralHeaderSettings.FontSize;
            GeneralHeaderIsFontBold = s.GeneralHeaderSettings.IsFontBold;
            GeneralHeaderHorizontalAlignment = s.GeneralHeaderSettings.HorizontalAlignment;
            GeneralHeaderVerticalAlignment = s.GeneralHeaderSettings.VerticalAlignment;
            GeneralHeaderFontColor = new SolidColorBrush(s.GeneralHeaderSettings.FontColor.ToMediaColor());
            GeneralHeaderBackgroundColor = new SolidColorBrush(s.GeneralHeaderSettings.BackgroundColor.ToMediaColor());
            // Glass Box
            GlassBoxHorizontalAlignment = s.TotalGlassesBoxSettings.HorizontalAlignment;
            GlassBoxVerticalAlignment = s.TotalGlassesBoxSettings.VerticalAlignment;
            GlassBoxBackgroundColor = new SolidColorBrush(s.TotalGlassesBoxSettings.BackgroundColor.ToMediaColor());
            GlassBoxFontSize = s.TotalGlassesBoxSettings.FontSize;
            GlassBoxIsFontBold = s.TotalGlassesBoxSettings.IsFontBold;
            GlassBoxFontColor = new SolidColorBrush(s.TotalGlassesBoxSettings.FontColor.ToMediaColor());
            //Notes
            NotesHorizontalAlignment = s.NotesBoxSettings.HorizontalAlignment;
            NotesVerticalAlignment = s.NotesBoxSettings.VerticalAlignment;
            NotesBackgroundColor = new SolidColorBrush(s.NotesBoxSettings.BackgroundColor.ToMediaColor());
            NotesFontSize = s.NotesBoxSettings.FontSize;
            NotesIsFontBold = s.NotesBoxSettings.IsFontBold;
            NotesFontColor = new SolidColorBrush(s.NotesBoxSettings.FontColor.ToMediaColor());
            NumberOfRowsForNotes = s.NotesBoxSettings.NumberOfRowsForNotes;

            //Call Notify PropChange so that all the above Get Notified only with a single Call
            OnPropertyChanged("");
        }

        /// <summary>
        /// Retrieves XlsSettings represented by this ViewModel
        /// </summary>
        /// <returns></returns>
        public XlsSettingsGlasses ToXlsSettingsGlasses()
        {
            return new XlsSettingsGlasses()
            {
                Id= this.Id,
                IsSelected = this.IsSelected,
                SettingsName = SettingsName,
                WorksheetName = WorksheetName,
                FontSize = FontSize,
                IsFontBold = IsFontBold,
                FontFamily = FontFamily,
                FontColor = FontColor.ToXLColor(),
                NormalRowHeight = NormalRowHeight,
                TotalRows = TotalRows,
                FirstRowIndex = FirstRowIndex,
                FirstColumnIndex= FirstColumnIndex,
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment,
                TitlesRowHeight = TitleRowHeight,
                TitlesBackgroundColor = TitleBackgroundColor.ToXLColor(),
                TitlesFontColor= TitleFontColor.ToXLColor(),
                IsTitlesFontBold = IsTitleFontBold,

                TableHeaderSettings = new()
                {
                    TableHeaderRowHeight = TablesHeaderRowHeight,
                    FontSize = TablesFontSize,
                    IsFontBold= TablesIsFontBold,
                    FontColor = TablesFontColor.ToXLColor(),
                    BackgroundColor = TablesBackgroundColor.ToXLColor(),
                    HorizontalAlignment = TablesHorizontalAlignment,
                    VerticalAlignment = TablesVerticalAlignment,
                },

                GlassTableSettings = new()
                {
                    AlternatingTableRowBackground = AlternatingTableRowBackground.ToXLColor(),
                    HorizontalBorderColor = TablesHorizontalBorderColor.ToXLColor(),
                    VerticalBorderColor = TablesVerticalBorderColor.ToXLColor(),
                    HorizontalBorderThickness = TablesHorizontalBorderThickness,
                    VerticalBorderThickness = TablesVerticalBorderThickness,
                    TablePerimetricalBorderColor = TablesPerimetricalBorderColor.ToXLColor(),
                    TablePerimetricalBorderThickness = TablesPerimetricalBorderThickness,
                },

                GeneralHeaderSettings = new()
                {
                    BackgroundColor = GeneralHeaderBackgroundColor.ToXLColor(),
                    FontColor = GeneralHeaderFontColor.ToXLColor(),
                    FontSize = GeneralHeaderFontSize,
                    HorizontalAlignment = GeneralHeaderHorizontalAlignment,
                    VerticalAlignment = GeneralHeaderVerticalAlignment,
                    IsFontBold = GeneralHeaderIsFontBold,
                    RowHeight = GeneralHeaderRowHeight,
                },

                TotalGlassesBoxSettings = new()
                {
                    BackgroundColor = GlassBoxBackgroundColor.ToXLColor(),
                    FontColor = GlassBoxFontColor.ToXLColor(),
                    FontSize = GlassBoxFontSize,
                    HorizontalAlignment= GlassBoxHorizontalAlignment,
                    VerticalAlignment= GlassBoxVerticalAlignment,
                    IsFontBold= GlassBoxIsFontBold,
                },

                NotesBoxSettings = new()
                {
                    BackgroundColor = NotesBackgroundColor.ToXLColor(),
                    FontColor = NotesFontColor.ToXLColor(),
                    FontSize = NotesFontSize,
                    HorizontalAlignment = NotesHorizontalAlignment,
                    VerticalAlignment = NotesVerticalAlignment,
                    IsFontBold= NotesIsFontBold,
                    NumberOfRowsForNotes = NumberOfRowsForNotes,
                },
            };
        }

    }
}
