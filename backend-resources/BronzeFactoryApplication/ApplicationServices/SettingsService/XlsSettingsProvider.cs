using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using ClosedXML.Excel;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SqliteApplicationSettings.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.SettingsService
{
    public class XlsSettingsProvider : IXlsSettingsProvider
    {
        private readonly SettingsDbContextFactory _dbContextFactory;

        public XlsSettingsProvider(SettingsDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Returns all Xls Settings , or throws if not Found
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When not Found</exception>
        public async Task<IEnumerable<XlsSettingsGlasses>> GetXlsGlassesSettingsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var settingsDTO = await context.XlsSettingsGlassesTable.ToListAsync();
            if (settingsDTO is null || settingsDTO.Count is 0)
            {
                throw new Exception("No Settings where Found");
            }
            return settingsDTO.Select(dto => ConvertToSettings(dto));
        }
        /// <summary>
        /// Returns a Setting by Name
        /// </summary>
        /// <param name="settingsName">The Name of the Setting</param>
        /// <returns></returns>
        /// <exception cref="Exception">When nothing is found</exception>
        public async Task<XlsSettingsGlasses> GetXlsSettingAsync(string settingsName)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var settings = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s => s.SettingsName == settingsName);
            if (settings is not null) return ConvertToSettings(settings);
            else throw new Exception($"Settings with Name : {settingsName} not Found");
        }
        /// <summary>
        /// Returns the Selected Setting or the Default Setting if no Selected Setting is Found , or throws if nothing of the two works
        /// </summary>
        /// <returns></returns>
        public async Task<XlsSettingsGlasses> GetSelectedSettingsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var selected = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s => s.IsSelected == true);
            if (selected is null)
            {
                selected = await context.XlsSettingsGlassesTable.FindAsync(1);
                if (selected is null) throw new Exception("No Defaults and no Selected Settings where Found");
            }
            return ConvertToSettings(selected);
        }
        /// <summary>
        /// Gets All the Names of the Available Settings or throws if nothing is found
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When no settings are found</exception>
        public async Task<IEnumerable<string>> GetAvailableSettingsNamesAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var settingsNames = await context.XlsSettingsGlassesTable.Select(s => s.SettingsName).ToListAsync();
            if (settingsNames is null || settingsNames.Count is 0)
            {
                throw new Exception("No Settings where Found");
            }
            return settingsNames;
        }
        /// <summary>
        /// Creates new Settings 
        /// </summary>
        /// <param name="settings">The New Settings to Create</param>
        /// <returns>The Inserted Id</returns>
        public async Task<int> AddNewXlsSettingsAsync(XlsSettingsGlasses settings)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var settingsDTO = ConvertToDTO(settings);
                settingsDTO.Created = DateTime.Now;
                settingsDTO.LastModified = DateTime.MinValue;
                var entry = context.XlsSettingsGlassesTable.Add(settingsDTO);
                await context.SaveChangesAsync();
                return settingsDTO.Id; //Automatically generated Id
            }
        }
        /// <summary>
        /// Deletes a Setting by Id
        /// </summary>
        /// <param name="id">The Id of the setting to Delete</param>
        /// <returns></returns>
        /// <exception cref="Exception">When no setting is found with that Id</exception>
        public async Task DeleteXlsSettingsAsync(int id)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var toRemove = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s => s.Id == id);
            if (toRemove?.Id == 1) throw new NotSupportedException("Cannot Delete Default Settings...");
            if (toRemove != null)
            {
                context.Remove(toRemove);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Id : {id} Not Found in the Database");
            }
        }
        /// <summary>
        /// Deletes a Setting By Name
        /// </summary>
        /// <param name="settingsName">The Name of the Setting</param>
        /// <returns></returns>
        /// <exception cref="Exception">When no Setting is Found with that Name</exception>
        public async Task DeleteXlsSettingsAsync(string settingsName)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var toRemove = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s => s.SettingsName == settingsName);
            if (toRemove?.Id == 1) throw new NotSupportedException("Cannot Delete Default Settings");
            if (toRemove != null)
            {
                context.Remove(toRemove);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Settings with Name : {settingsName} Not Found in the Database");
            }
        }
        /// <summary>
        /// Updates an Existing Setting
        /// </summary>
        /// <param name="settingsToUpdate">The Setting to Update</param>
        /// <returns></returns>
        public async Task UpdateXlsSettingsAsync(XlsSettingsGlasses settingsToUpdate)
        {
            if (settingsToUpdate.Id == 1) throw new NotSupportedException("Default Settings Cannot be Changed...");
            using var context = _dbContextFactory.CreateDbContext();
            context.XlsSettingsGlassesTable.Update(ConvertToDTO(settingsToUpdate));
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// Selects a Setting
        /// </summary>
        /// <param name="settingsName">The Name of the Setting to Select</param>
        /// <returns></returns>
        public async Task SelectSettingAsync(string settingsName) 
        {
            using var context = _dbContextFactory.CreateDbContext();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                //Find which is Currently Selected and turn it to false
                var currentSelectedSetting = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s => s.IsSelected);
                if (currentSelectedSetting != null) 
                {
                    currentSelectedSetting.IsSelected = false;
                    context.XlsSettingsGlassesTable.Update(currentSelectedSetting); 
                }

                //Find and Select the new Setting
                var newSelectedSetting = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s=>s.SettingsName == settingsName);
                if (newSelectedSetting is not null)
                {
                    newSelectedSetting.IsSelected = true;
                    context.XlsSettingsGlassesTable.Update(newSelectedSetting);
                }
                else
                {
                    throw new Exception($"Setting {settingsName} Was Not Found for Selection");
                }
                
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Warning("Sqlite Settings Transaction Failed");
                Log.Error(ex, "{message}", ex.Message);
                throw;
            }
        }
        /// <summary>
        /// Returns the Default Settings
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">When there are no Default Settings</exception>
        public async Task<XlsSettingsGlasses> GetDefaultSettings()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var settings = await context.XlsSettingsGlassesTable.FirstOrDefaultAsync(s => s.Id == 1);
            if (settings is not null) return ConvertToSettings(settings);
            else throw new Exception($"Default Settings not Found... ???");
        }


        /// <summary>
        /// Converts the DTO Object into Settings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static XlsSettingsGlasses ConvertToSettings(XlsSettingsGlassesDTO s)
        {
            var cultureString = ((App)App.Current).SelectedLanguage;
            XlsSettingsGlasses settings = new(cultureString)
            {
                Id = s.Id,
                SettingsName = s.SettingsName,
                WorksheetName = s.WorksheetName,
                FontSize = s.FontSize,
                IsFontBold = s.IsFontBold,
                FontFamily = s.FontFamily,
                FontColor = XLColor.FromArgb(s.FontColorA, s.FontColorR, s.FontColorG, s.FontColorB),
                NormalRowHeight = s.NormalRowHeight,
                TotalRows = s.TotalRows,
                FirstRowIndex = s.FirstRowIndex,
                FirstColumnIndex = s.FirstColumnIndex,
                HorizontalAlignment = Enum.TryParse(s.HorizontalAlignment, true, out XLAlignmentHorizontalValues result1) ? result1 : XLAlignmentHorizontalValues.Center,
                VerticalAlignment = Enum.TryParse(s.VerticalAlignment, true, out XLAlignmentVerticalValues result2) ? result2 : XLAlignmentVerticalValues.Center,
                TitlesRowHeight = s.TitlesRowHeight,
                TitlesBackgroundColor = XLColor.FromArgb(s.TitlesBackgroundColorA, s.TitlesBackgroundColorR, s.TitlesBackgroundColorG, s.TitlesBackgroundColorB),
                TitlesFontColor = XLColor.FromArgb(s.TitlesFontColorA, s.TitlesFontColorR, s.TitlesFontColorG, s.TitlesFontColorB),
                IsTitlesFontBold = s.IsTitlesFontBold,

                TableHeaderSettings = new()
                {
                    TableHeaderRowHeight = s.TableHeaderRowHeight,
                    FontSize = s.TableHeaderFontSize,
                    IsFontBold = s.TableHeaderIsFontBold,
                    FontColor = XLColor.FromArgb(s.FontColorA, s.FontColorR, s.FontColorG, s.FontColorB),
                    BackgroundColor = XLColor.FromArgb(s.TableHeaderBackgroundColorA, s.TableHeaderBackgroundColorR, s.TableHeaderBackgroundColorG, s.TableHeaderBackgroundColorB),
                    HorizontalAlignment = Enum.TryParse(s.TableHeaderHorizontalAlignment, true, out XLAlignmentHorizontalValues result3) ? result3 : XLAlignmentHorizontalValues.Left,
                    VerticalAlignment = Enum.TryParse(s.TableHeaderVerticalAlignment, true, out XLAlignmentVerticalValues result4) ? result4 : XLAlignmentVerticalValues.Center,
                },

                GlassTableSettings = new()
                {
                    HorizontalBorderThickness = Enum.TryParse(s.GlassTableHorizontalBorderThickness, true, out XLBorderStyleValues result5) ? result5 : XLBorderStyleValues.Medium,
                    HorizontalBorderColor = XLColor.FromArgb(s.GlassTableHorizontalBorderColorA, s.GlassTableHorizontalBorderColorR, s.GlassTableHorizontalBorderColorG, s.GlassTableHorizontalBorderColorB),
                    VerticalBorderThickness = Enum.TryParse(s.GlassTableVerticalBorderThickness, true, out XLBorderStyleValues result6) ? result6 : XLBorderStyleValues.None,
                    VerticalBorderColor = XLColor.FromArgb(s.GlassTableVerticalBorderColorA, s.GlassTableVerticalBorderColorR, s.GlassTableVerticalBorderColorG, s.GlassTableVerticalBorderColorB),
                    AlternatingTableRowBackground = XLColor.FromArgb(s.GlassTableAlternatingRowTableBackgroundColorA, s.GlassTableAlternatingRowTableBackgroundColorR, s.GlassTableAlternatingRowTableBackgroundColorG, s.GlassTableAlternatingRowTableBackgroundColorB),
                    TablePerimetricalBorderThickness = Enum.TryParse(s.GlassTablePerimetricalBorderThickness, true, out XLBorderStyleValues result7) ? result7 : XLBorderStyleValues.Medium,
                    TablePerimetricalBorderColor = XLColor.FromArgb(s.GlassTablePerimetricalBorderColorA, s.GlassTablePerimetricalBorderColorR, s.GlassTablePerimetricalBorderColorG, s.GlassTablePerimetricalBorderColorB),
                },

                GeneralHeaderSettings = new()
                {
                    RowHeight = s.GeneralHeaderRowHeight,
                    FontSize = s.GeneralHeaderFontSize,
                    IsFontBold = s.GeneralHeaderIsFontBold,
                    HorizontalAlignment = Enum.TryParse(s.GeneralHeaderHorizontalAlignment, true, out XLAlignmentHorizontalValues result8) ? result8 : XLAlignmentHorizontalValues.Center,
                    VerticalAlignment = Enum.TryParse(s.GeneralHeaderVerticalAlignment, true, out XLAlignmentVerticalValues result9) ? result9 : XLAlignmentVerticalValues.Center,
                    FontColor = XLColor.FromArgb(s.GeneralHeaderFontColorA, s.GeneralHeaderFontColorR, s.GeneralHeaderFontColorG, s.GeneralHeaderFontColorB),
                    BackgroundColor = XLColor.FromArgb(s.GeneralHeaderBackgroundColorA, s.GeneralHeaderBackgroundColorR, s.GeneralHeaderBackgroundColorG, s.GeneralHeaderBackgroundColorB),
                },

                TotalGlassesBoxSettings = new()
                {
                    HorizontalAlignment = Enum.TryParse(s.TotalGlassesBoxHorizontalAlignment, true, out XLAlignmentHorizontalValues result10) ? result10 : XLAlignmentHorizontalValues.Center,
                    VerticalAlignment = Enum.TryParse(s.TotalGlassesBoxVerticalAlignment, true, out XLAlignmentVerticalValues result11) ? result11 : XLAlignmentVerticalValues.Center,
                    BackgroundColor = XLColor.FromArgb(s.TotalGlassesBoxBackgroundColorA, s.TotalGlassesBoxBackgroundColorR, s.TotalGlassesBoxBackgroundColorG, s.TotalGlassesBoxBackgroundColorB),
                    FontSize = s.TotalGlassesBoxFontSize,
                    IsFontBold = s.TotalGlassesBoxIsFontBold,
                    FontColor = XLColor.FromArgb(s.TotalGlassesBoxFontColorA, s.TotalGlassesBoxFontColorR, s.TotalGlassesBoxFontColorG, s.TotalGlassesBoxFontColorB),
                },

                NotesBoxSettings = new()
                {
                    HorizontalAlignment = Enum.TryParse(s.NotesHorizontalAlignment, true, out XLAlignmentHorizontalValues result12) ? result12 : XLAlignmentHorizontalValues.Center,
                    VerticalAlignment = Enum.TryParse(s.NotesVerticalAlignment, true, out XLAlignmentVerticalValues result13) ? result13 : XLAlignmentVerticalValues.Center,
                    BackgroundColor = XLColor.FromArgb(s.NotesBackgroundColorA, s.NotesBackgroundColorR, s.NotesBackgroundColorG, s.NotesBackgroundColorB),
                    FontSize = s.NotesFontSize,
                    IsFontBold = s.NotesIsFontBold,
                    FontColor = XLColor.FromArgb(s.NotesFontColorA, s.NotesFontColorR, s.NotesFontColorG, s.NotesFontColorB),
                    NumberOfRowsForNotes = s.NumberOfRowsForNotes
                },
            };
            return settings;
        }
        /// <summary>
        /// Converts an XLS SettingsGlasses to a dto object , only the passed culture strings are passed
        /// </summary>
        /// <param name="s"></param>
        /// <param name="cultureString"></param>
        /// <returns></returns>
        private static XlsSettingsGlassesDTO ConvertToDTO(XlsSettingsGlasses s)
        {
            XlsSettingsGlassesDTO dto = new()
            {
                Id = s.Id,
                IsSelected = s.IsSelected,
                SettingsName = s.SettingsName,
                LastModified = DateTime.MinValue,
                Created = DateTime.MinValue,
                WorksheetName = s.WorksheetName,
                FontSize = s.FontSize,
                IsFontBold = s.IsFontBold,
                FontFamily = s.FontFamily,
                FontColorA = s.FontColor.Color.A,
                FontColorR = s.FontColor.Color.R,
                FontColorG = s.FontColor.Color.G,
                FontColorB = s.FontColor.Color.B,
                NormalRowHeight = s.NormalRowHeight,
                TotalRows = s.TotalRows,
                FirstRowIndex = s.FirstRowIndex,
                FirstColumnIndex = s.FirstColumnIndex,
                HorizontalAlignment = s.HorizontalAlignment.ToString(),
                VerticalAlignment = s.VerticalAlignment.ToString(),
                TitlesRowHeight = s.TitlesRowHeight,
                TitlesBackgroundColorA = s.TitlesBackgroundColor.Color.A,
                TitlesBackgroundColorR = s.TitlesBackgroundColor.Color.R,
                TitlesBackgroundColorG = s.TitlesBackgroundColor.Color.G,
                TitlesBackgroundColorB = s.TitlesBackgroundColor.Color.B,
                TitlesFontColorA = s.TitlesFontColor.Color.A,
                TitlesFontColorR = s.TitlesFontColor.Color.R,
                TitlesFontColorG = s.TitlesFontColor.Color.G,
                TitlesFontColorB = s.TitlesFontColor.Color.B,
                IsTitlesFontBold = s.IsTitlesFontBold,

                //Table - Header
                TableHeaderRowHeight = s.TableHeaderSettings.TableHeaderRowHeight,
                TableHeaderFontSize = s.TableHeaderSettings.FontSize,
                TableHeaderIsFontBold = s.TableHeaderSettings.IsFontBold,
                TableHeaderFontColorA = s.TableHeaderSettings.FontColor.Color.A,
                TableHeaderFontColorR = s.TableHeaderSettings.FontColor.Color.R,
                TableHeaderFontColorG = s.TableHeaderSettings.FontColor.Color.G,
                TableHeaderFontColorB = s.TableHeaderSettings.FontColor.Color.B,
                TableHeaderBackgroundColorA = s.TableHeaderSettings.BackgroundColor.Color.A,
                TableHeaderBackgroundColorR = s.TableHeaderSettings.BackgroundColor.Color.R,
                TableHeaderBackgroundColorG = s.TableHeaderSettings.BackgroundColor.Color.G,
                TableHeaderBackgroundColorB = s.TableHeaderSettings.BackgroundColor.Color.B,
                TableHeaderHorizontalAlignment = s.TableHeaderSettings.HorizontalAlignment.ToString(),
                TableHeaderVerticalAlignment = s.TableHeaderSettings.VerticalAlignment.ToString(),

                //Glass Table
                GlassTableHorizontalBorderThickness = s.GlassTableSettings.HorizontalBorderThickness.ToString(),
                GlassTableHorizontalBorderColorA = s.GlassTableSettings.HorizontalBorderColor.Color.A,
                GlassTableHorizontalBorderColorR = s.GlassTableSettings.HorizontalBorderColor.Color.R,
                GlassTableHorizontalBorderColorG = s.GlassTableSettings.HorizontalBorderColor.Color.G,
                GlassTableHorizontalBorderColorB = s.GlassTableSettings.HorizontalBorderColor.Color.B,
                GlassTableVerticalBorderThickness = s.GlassTableSettings.VerticalBorderThickness.ToString(),
                GlassTableVerticalBorderColorA = s.GlassTableSettings.VerticalBorderColor.Color.A,
                GlassTableVerticalBorderColorR = s.GlassTableSettings.VerticalBorderColor.Color.R,
                GlassTableVerticalBorderColorG = s.GlassTableSettings.VerticalBorderColor.Color.G,
                GlassTableVerticalBorderColorB = s.GlassTableSettings.VerticalBorderColor.Color.B,
                GlassTableAlternatingRowTableBackgroundColorA = s.GlassTableSettings.AlternatingTableRowBackground.Color.A,
                GlassTableAlternatingRowTableBackgroundColorR = s.GlassTableSettings.AlternatingTableRowBackground.Color.R,
                GlassTableAlternatingRowTableBackgroundColorG = s.GlassTableSettings.AlternatingTableRowBackground.Color.G,
                GlassTableAlternatingRowTableBackgroundColorB = s.GlassTableSettings.AlternatingTableRowBackground.Color.B,
                GlassTablePerimetricalBorderThickness = s.GlassTableSettings.TablePerimetricalBorderThickness.ToString(),
                GlassTablePerimetricalBorderColorA = s.GlassTableSettings.TablePerimetricalBorderColor.Color.A,
                GlassTablePerimetricalBorderColorR = s.GlassTableSettings.TablePerimetricalBorderColor.Color.R,
                GlassTablePerimetricalBorderColorG = s.GlassTableSettings.TablePerimetricalBorderColor.Color.G,
                GlassTablePerimetricalBorderColorB = s.GlassTableSettings.TablePerimetricalBorderColor.Color.B,

                //General Header Settings
                GeneralHeaderRowHeight = s.GeneralHeaderSettings.RowHeight,
                GeneralHeaderFontSize = s.GeneralHeaderSettings.FontSize,
                GeneralHeaderIsFontBold = s.GeneralHeaderSettings.IsFontBold,
                GeneralHeaderHorizontalAlignment = s.GeneralHeaderSettings.HorizontalAlignment.ToString(),
                GeneralHeaderVerticalAlignment = s.GeneralHeaderSettings.VerticalAlignment.ToString(),
                GeneralHeaderFontColorA = s.GeneralHeaderSettings.FontColor.Color.A,
                GeneralHeaderFontColorR = s.GeneralHeaderSettings.FontColor.Color.R,
                GeneralHeaderFontColorG = s.GeneralHeaderSettings.FontColor.Color.G,
                GeneralHeaderFontColorB = s.GeneralHeaderSettings.FontColor.Color.B,
                GeneralHeaderBackgroundColorA = s.GeneralHeaderSettings.BackgroundColor.Color.A,
                GeneralHeaderBackgroundColorR = s.GeneralHeaderSettings.BackgroundColor.Color.R,
                GeneralHeaderBackgroundColorG = s.GeneralHeaderSettings.BackgroundColor.Color.G,
                GeneralHeaderBackgroundColorB = s.GeneralHeaderSettings.BackgroundColor.Color.B,

                //Glasses Box
                TotalGlassesBoxHorizontalAlignment = s.TotalGlassesBoxSettings.HorizontalAlignment.ToString(),
                TotalGlassesBoxVerticalAlignment = s.TotalGlassesBoxSettings.VerticalAlignment.ToString(),
                TotalGlassesBoxBackgroundColorA = s.TotalGlassesBoxSettings.BackgroundColor.Color.A,
                TotalGlassesBoxBackgroundColorR = s.TotalGlassesBoxSettings.BackgroundColor.Color.R,
                TotalGlassesBoxBackgroundColorG = s.TotalGlassesBoxSettings.BackgroundColor.Color.G,
                TotalGlassesBoxBackgroundColorB = s.TotalGlassesBoxSettings.BackgroundColor.Color.B,
                TotalGlassesBoxFontSize = s.TotalGlassesBoxSettings.FontSize,
                TotalGlassesBoxIsFontBold = s.TotalGlassesBoxSettings.IsFontBold,
                TotalGlassesBoxFontColorA = s.TotalGlassesBoxSettings.FontColor.Color.A,
                TotalGlassesBoxFontColorR = s.TotalGlassesBoxSettings.FontColor.Color.R,
                TotalGlassesBoxFontColorG = s.TotalGlassesBoxSettings.FontColor.Color.G,
                TotalGlassesBoxFontColorB = s.TotalGlassesBoxSettings.FontColor.Color.B,

                //Notes
                NotesHorizontalAlignment = s.NotesBoxSettings.HorizontalAlignment.ToString(),
                NotesVerticalAlignment = s.NotesBoxSettings.VerticalAlignment.ToString(),
                NotesBackgroundColorA = s.NotesBoxSettings.BackgroundColor.Color.A,
                NotesBackgroundColorR = s.NotesBoxSettings.BackgroundColor.Color.R,
                NotesBackgroundColorG = s.NotesBoxSettings.BackgroundColor.Color.G,
                NotesBackgroundColorB = s.NotesBoxSettings.BackgroundColor.Color.B,
                NotesFontSize = s.NotesBoxSettings.FontSize,
                NotesIsFontBold = s.NotesBoxSettings.IsFontBold,
                NotesFontColorA = s.NotesBoxSettings.FontColor.Color.A,
                NotesFontColorR = s.NotesBoxSettings.FontColor.Color.R,
                NotesFontColorG = s.NotesBoxSettings.FontColor.Color.G,
                NotesFontColorB = s.NotesBoxSettings.FontColor.Color.B,
                NumberOfRowsForNotes = s.NotesBoxSettings.NumberOfRowsForNotes,
            };
            return dto;
        }
    }

}
