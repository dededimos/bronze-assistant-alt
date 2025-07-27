using ClosedXML.Excel;
using CommonInterfacesBronze;

namespace XMLClosedReporting.AdvancedReportModels
{
    public class ReportColumnOptionsAdvanced : IDeepClonable<ReportColumnOptionsAdvanced>
    {
        /// <summary>
        /// The Styles of the Cells that contain the values of the Column 
        /// </summary>
        public StylesInfo ValueCellsStyle { get; set; } = StylesProvider.DefaultValueCellStyle();
        /// <summary>
        /// The Styles of the Header Cell of the Column
        /// </summary>
        public StylesInfo HeaderCellStyle { get; set; } = StylesProvider.DefaultColumnHeaderStyle();
        /// <summary>
        /// The Width of the Column
        /// </summary>
        public double ColumnWidth { get; set; } = double.NaN;
        /// <summary>
        /// The Row Height of the Header 
        /// </summary>
        public double HeaderRowHeight { get; set; } = double.NaN;
        /// <summary>
        /// The Row Height of the Cells containing the Values of the Column
        /// </summary>
        public double ValueCellsRowHeight { get; set; } = double.NaN;
        /// <summary>
        /// The Number of Actual Columns this Column occupies
        /// <para>If more than 1 then the Column will merge the cells on the right until it occupies the desired number</para>
        /// <para>If a specific Width is used in the Column , then the Width will be equally split between the Occupied Columns</para>
        /// </summary>
        public int NumberOfOccupiedColumns { get; set; } = 1;

        public ReportColumnOptionsAdvanced GetDeepClone()
        {
            var clone = (ReportColumnOptionsAdvanced)MemberwiseClone();
            clone.ValueCellsStyle = ValueCellsStyle.GetDeepClone();
            clone.HeaderCellStyle = HeaderCellStyle.GetDeepClone();
            return clone;
        }
        public static ReportColumnOptionsAdvanced DefaultOptions() => new();
    }
}