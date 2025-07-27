using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SqliteApplicationSettings.Migrations
{
    /// <inheritdoc />
    public partial class InitialEFCore900 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralSettingsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SettingName = table.Column<string>(type: "TEXT", nullable: false),
                    SettingValue = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InfoStringObject = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettingsTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlassesStockServiceUserSettingsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShouldCompareHeight = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowedHeightDifference = table.Column<double>(type: "REAL", nullable: false),
                    ShouldCompareLength = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowedLengthDifference = table.Column<double>(type: "REAL", nullable: false),
                    ShouldCompareThickness = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShouldCompareFinish = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConcernsModel = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InfoStringObject = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlassesStockServiceUserSettingsTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchOrdersViewSettingsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxResultsGetSmallOrders = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InfoStringObject = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchOrdersViewSettingsTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XlsSettingsGlassesTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    SettingsName = table.Column<string>(type: "TEXT", nullable: false),
                    WorksheetName = table.Column<string>(type: "TEXT", nullable: false),
                    FontSize = table.Column<double>(type: "REAL", nullable: false),
                    IsFontBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    FontFamily = table.Column<string>(type: "TEXT", nullable: false),
                    FontColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    FontColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    FontColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    FontColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    NormalRowHeight = table.Column<double>(type: "REAL", nullable: false),
                    TotalRows = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstRowIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstColumnIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    HorizontalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    VerticalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    TitlesRowHeight = table.Column<double>(type: "REAL", nullable: false),
                    TitlesBackgroundColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesBackgroundColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesBackgroundColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesBackgroundColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesFontColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesFontColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesFontColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    TitlesFontColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    IsTitlesFontBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    TableHeaderRowHeight = table.Column<double>(type: "REAL", nullable: false),
                    TableHeaderFontSize = table.Column<double>(type: "REAL", nullable: false),
                    TableHeaderIsFontBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    TableHeaderFontColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderFontColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderFontColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderFontColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderBackgroundColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderBackgroundColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderBackgroundColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderBackgroundColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    TableHeaderHorizontalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    TableHeaderVerticalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    GlassTableHorizontalBorderThickness = table.Column<string>(type: "TEXT", nullable: false),
                    GlassTableHorizontalBorderColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableHorizontalBorderColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableHorizontalBorderColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableHorizontalBorderColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableVerticalBorderThickness = table.Column<string>(type: "TEXT", nullable: false),
                    GlassTableVerticalBorderColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableVerticalBorderColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableVerticalBorderColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableVerticalBorderColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableAlternatingRowTableBackgroundColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableAlternatingRowTableBackgroundColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableAlternatingRowTableBackgroundColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTableAlternatingRowTableBackgroundColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTablePerimetricalBorderThickness = table.Column<string>(type: "TEXT", nullable: false),
                    GlassTablePerimetricalBorderColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTablePerimetricalBorderColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTablePerimetricalBorderColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    GlassTablePerimetricalBorderColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderRowHeight = table.Column<double>(type: "REAL", nullable: false),
                    GeneralHeaderFontSize = table.Column<double>(type: "REAL", nullable: false),
                    GeneralHeaderIsFontBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    GeneralHeaderHorizontalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    GeneralHeaderVerticalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    GeneralHeaderFontColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderFontColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderFontColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderFontColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderBackgroundColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderBackgroundColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderBackgroundColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralHeaderBackgroundColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxHorizontalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    TotalGlassesBoxVerticalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    TotalGlassesBoxFontSize = table.Column<double>(type: "REAL", nullable: false),
                    TotalGlassesBoxIsFontBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxFontColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxFontColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxFontColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxFontColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxBackgroundColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxBackgroundColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxBackgroundColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGlassesBoxBackgroundColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesHorizontalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    NotesVerticalAlignment = table.Column<string>(type: "TEXT", nullable: false),
                    NotesFontSize = table.Column<double>(type: "REAL", nullable: false),
                    NotesIsFontBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotesFontColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesFontColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesFontColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesFontColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesBackgroundColorA = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesBackgroundColorR = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesBackgroundColorG = table.Column<int>(type: "INTEGER", nullable: false),
                    NotesBackgroundColorB = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfRowsForNotes = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InfoStringObject = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XlsSettingsGlassesTable", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GeneralSettingsTable",
                columns: new[] { "Id", "Created", "InfoStringObject", "LastModified", "SettingName", "SettingValue" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SelectedTheme", "Dark" },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SelectedLanguage", "el-GR" },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ApplicationVersion", "Ver2.17-17-12-2023" }
                });

            migrationBuilder.InsertData(
                table: "GlassesStockServiceUserSettingsTable",
                columns: new[] { "Id", "AllowedHeightDifference", "AllowedLengthDifference", "ConcernsModel", "Created", "InfoStringObject", "IsDefault", "IsSelected", "LastModified", "ShouldCompareFinish", "ShouldCompareHeight", "ShouldCompareLength", "ShouldCompareThickness" },
                values: new object[,]
                {
                    { 1, 50.0, 50.0, 0, new DateTime(2024, 11, 17, 2, 3, 3, 179, DateTimeKind.Local).AddTicks(6112), "DefaultSettings-Model9A", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 2, 50.0, 50.0, 0, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6547), "UserSettings-Model9A", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 3, 50.0, 50.0, 1, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6694), "DefaultSettings-Model9S", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 4, 50.0, 50.0, 1, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6702), "UserSettings-Model9S", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 5, 50.0, 50.0, 2, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6707), "DefaultSettings-Model94", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 6, 50.0, 50.0, 2, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6711), "UserSettings-Model94", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 7, 50.0, 50.0, 3, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6718), "DefaultSettings-Model9F", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 8, 50.0, 50.0, 3, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6723), "UserSettings-Model9F", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 9, 50.0, 50.0, 4, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6727), "DefaultSettings-Model9B", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 10, 50.0, 50.0, 4, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6730), "UserSettings-Model9B", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 11, 50.0, 50.0, 5, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6735), "DefaultSettings-ModelW", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 12, 50.0, 50.0, 5, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6739), "UserSettings-ModelW", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 13, 50.0, 50.0, 6, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6743), "DefaultSettings-ModelHB", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 14, 50.0, 50.0, 6, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6746), "UserSettings-ModelHB", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 15, 50.0, 50.0, 7, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6750), "DefaultSettings-ModelNP", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 16, 50.0, 50.0, 7, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6754), "UserSettings-ModelNP", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 17, 50.0, 50.0, 8, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6758), "DefaultSettings-ModelVS", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 18, 50.0, 50.0, 8, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6761), "UserSettings-ModelVS", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 19, 50.0, 50.0, 9, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6766), "DefaultSettings-ModelVF", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 20, 50.0, 50.0, 9, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6770), "UserSettings-ModelVF", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 21, 50.0, 50.0, 10, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6774), "DefaultSettings-ModelV4", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 22, 50.0, 50.0, 10, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6777), "UserSettings-ModelV4", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 23, 50.0, 50.0, 11, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6781), "DefaultSettings-ModelVA", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 24, 50.0, 50.0, 11, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6784), "UserSettings-ModelVA", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 25, 50.0, 50.0, 12, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6788), "DefaultSettings-ModelWS", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 26, 50.0, 50.0, 12, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6791), "UserSettings-ModelWS", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 27, 50.0, 50.0, 13, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6795), "DefaultSettings-ModelE", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 28, 50.0, 50.0, 13, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6798), "UserSettings-ModelE", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 29, 50.0, 50.0, 14, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6802), "DefaultSettings-ModelWFlipper", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 30, 50.0, 50.0, 14, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6806), "UserSettings-ModelWFlipper", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 31, 50.0, 50.0, 15, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6809), "DefaultSettings-ModelDB", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 32, 50.0, 50.0, 15, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6813), "UserSettings-ModelDB", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 33, 50.0, 50.0, 16, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6817), "DefaultSettings-ModelNB", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 34, 50.0, 50.0, 16, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6820), "UserSettings-ModelNB", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 35, 50.0, 50.0, 17, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6825), "DefaultSettings-ModelNV", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 36, 50.0, 50.0, 17, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6829), "UserSettings-ModelNV", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 37, 50.0, 50.0, 18, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6832), "DefaultSettings-ModelMV2", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 38, 50.0, 50.0, 18, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6836), "UserSettings-ModelMV2", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 39, 50.0, 50.0, 19, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6839), "DefaultSettings-ModelNV2", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 40, 50.0, 50.0, 19, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6843), "UserSettings-ModelNV2", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 41, 50.0, 50.0, 20, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6846), "DefaultSettings-Model6WA", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 42, 50.0, 50.0, 20, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6850), "UserSettings-Model6WA", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 43, 50.0, 50.0, 21, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6854), "DefaultSettings-Model9C", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 44, 50.0, 50.0, 21, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6857), "UserSettings-Model9C", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 45, 50.0, 50.0, 22, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6861), "DefaultSettings-Model8W40", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 46, 50.0, 50.0, 22, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6864), "UserSettings-Model8W40", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 47, 50.0, 50.0, 23, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6868), "DefaultSettings-ModelGlassContainer", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 48, 50.0, 50.0, 23, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6872), "UserSettings-ModelGlassContainer", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 49, 50.0, 50.0, 24, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6876), "DefaultSettings-ModelQB", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 50, 50.0, 50.0, 24, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6885), "UserSettings-ModelQB", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 51, 50.0, 50.0, 25, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6889), "DefaultSettings-ModelQP", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false },
                    { 52, 50.0, 50.0, 25, new DateTime(2024, 11, 17, 2, 3, 3, 181, DateTimeKind.Local).AddTicks(6892), "UserSettings-ModelQP", false, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, true, false }
                });

            migrationBuilder.InsertData(
                table: "SearchOrdersViewSettingsTable",
                columns: new[] { "Id", "Created", "InfoStringObject", "IsDefault", "LastModified", "MaxResultsGetSmallOrders" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 17, 2, 3, 3, 174, DateTimeKind.Local).AddTicks(8868), "DefaultSettings", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10 },
                    { 2, new DateTime(2024, 11, 17, 2, 3, 3, 177, DateTimeKind.Local).AddTicks(4683), "UserSettings", false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10 }
                });

            migrationBuilder.InsertData(
                table: "XlsSettingsGlassesTable",
                columns: new[] { "Id", "Created", "FirstColumnIndex", "FirstRowIndex", "FontColorA", "FontColorB", "FontColorG", "FontColorR", "FontFamily", "FontSize", "GeneralHeaderBackgroundColorA", "GeneralHeaderBackgroundColorB", "GeneralHeaderBackgroundColorG", "GeneralHeaderBackgroundColorR", "GeneralHeaderFontColorA", "GeneralHeaderFontColorB", "GeneralHeaderFontColorG", "GeneralHeaderFontColorR", "GeneralHeaderFontSize", "GeneralHeaderHorizontalAlignment", "GeneralHeaderIsFontBold", "GeneralHeaderRowHeight", "GeneralHeaderVerticalAlignment", "GlassTableAlternatingRowTableBackgroundColorA", "GlassTableAlternatingRowTableBackgroundColorB", "GlassTableAlternatingRowTableBackgroundColorG", "GlassTableAlternatingRowTableBackgroundColorR", "GlassTableHorizontalBorderColorA", "GlassTableHorizontalBorderColorB", "GlassTableHorizontalBorderColorG", "GlassTableHorizontalBorderColorR", "GlassTableHorizontalBorderThickness", "GlassTablePerimetricalBorderColorA", "GlassTablePerimetricalBorderColorB", "GlassTablePerimetricalBorderColorG", "GlassTablePerimetricalBorderColorR", "GlassTablePerimetricalBorderThickness", "GlassTableVerticalBorderColorA", "GlassTableVerticalBorderColorB", "GlassTableVerticalBorderColorG", "GlassTableVerticalBorderColorR", "GlassTableVerticalBorderThickness", "HorizontalAlignment", "InfoStringObject", "IsFontBold", "IsSelected", "IsTitlesFontBold", "LastModified", "NormalRowHeight", "NotesBackgroundColorA", "NotesBackgroundColorB", "NotesBackgroundColorG", "NotesBackgroundColorR", "NotesFontColorA", "NotesFontColorB", "NotesFontColorG", "NotesFontColorR", "NotesFontSize", "NotesHorizontalAlignment", "NotesIsFontBold", "NotesVerticalAlignment", "NumberOfRowsForNotes", "SettingsName", "TableHeaderBackgroundColorA", "TableHeaderBackgroundColorB", "TableHeaderBackgroundColorG", "TableHeaderBackgroundColorR", "TableHeaderFontColorA", "TableHeaderFontColorB", "TableHeaderFontColorG", "TableHeaderFontColorR", "TableHeaderFontSize", "TableHeaderHorizontalAlignment", "TableHeaderIsFontBold", "TableHeaderRowHeight", "TableHeaderVerticalAlignment", "TitlesBackgroundColorA", "TitlesBackgroundColorB", "TitlesBackgroundColorG", "TitlesBackgroundColorR", "TitlesFontColorA", "TitlesFontColorB", "TitlesFontColorG", "TitlesFontColorR", "TitlesRowHeight", "TotalGlassesBoxBackgroundColorA", "TotalGlassesBoxBackgroundColorB", "TotalGlassesBoxBackgroundColorG", "TotalGlassesBoxBackgroundColorR", "TotalGlassesBoxFontColorA", "TotalGlassesBoxFontColorB", "TotalGlassesBoxFontColorG", "TotalGlassesBoxFontColorR", "TotalGlassesBoxFontSize", "TotalGlassesBoxHorizontalAlignment", "TotalGlassesBoxIsFontBold", "TotalGlassesBoxVerticalAlignment", "TotalRows", "VerticalAlignment", "WorksheetName" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 255, 106, 84, 68, "Calibri", 11.0, 0, 230, 230, 231, 255, 0, 0, 0, 22.0, "Center", true, 39.75, "Center", 255, 242, 242, 242, 255, 219, 169, 142, "Medium", 255, 0, 0, 0, "Medium", 255, 219, 169, 142, "None", "Center", "", true, true, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 19.5, 255, 242, 242, 242, 255, 0, 0, 0, 12.0, "Center", true, "Center", 5, "DefaultSettings", 255, 230, 230, 231, 255, 0, 0, 0, 16.0, "Left", false, 39.75, "Center", 255, 196, 114, 68, 255, 255, 255, 255, 20.0, 255, 242, 242, 242, 255, 0, 0, 0, 16.0, "Center", true, "Center", 2000, "Center", "GlassesOrderSheet" });

            migrationBuilder.CreateIndex(
                name: "IX_GlassesStockServiceUserSettingsTable_ConcernsModel_IsDefault",
                table: "GlassesStockServiceUserSettingsTable",
                columns: new[] { "ConcernsModel", "IsDefault" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_XlsSettingsGlassesTable_IsSelected",
                table: "XlsSettingsGlassesTable",
                column: "IsSelected");

            migrationBuilder.CreateIndex(
                name: "IX_XlsSettingsGlassesTable_SettingsName",
                table: "XlsSettingsGlassesTable",
                column: "SettingsName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSettingsTable");

            migrationBuilder.DropTable(
                name: "GlassesStockServiceUserSettingsTable");

            migrationBuilder.DropTable(
                name: "SearchOrdersViewSettingsTable");

            migrationBuilder.DropTable(
                name: "XlsSettingsGlassesTable");
        }
    }
}
