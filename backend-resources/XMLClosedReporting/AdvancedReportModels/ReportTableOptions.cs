using ClosedXML.Excel;
using CommonInterfacesBronze;
using System.Linq.Expressions;

namespace XMLClosedReporting.AdvancedReportModels
{
    public class ReportTableOptions<T>() : IDeepClonable<ReportTableOptions<T>>
    {
        /// <summary>
        /// The Options of the Columns of the Table , if not set the Report Builder will apply the default or table-wide options
        /// </summary>
        public ReportColumnOptionsAdvanced ColumnsOptions { get; set; } = ReportColumnOptionsAdvanced.DefaultOptions();
        /// <summary>
        /// The Style of the Table Header
        /// </summary>
        public StylesInfo TableTitleStyle { get; set; } = StylesProvider.DefaultTableHeaderStyle();

        public BorderStyles TableValuesOutsideBorderStyle { get; set; } = StylesProvider.DefaultBordersTableValues();

        /// <summary>
        /// The Row Height of the Table Header
        /// </summary>
        public double TableHeaderRowHeight { get; set; } = 39.75d;

        //The Functions are initilized as x=>0 to avoid null reference exceptions , and even if they are not set they will return the order they where before

        /// <summary>
        /// The Function that Orders the Items of the Table
        /// </summary>
        public Func<T, IComparable> OrderItemsBy { get; set; } = x => 0;
        /// <summary>
        /// The Function that Orders the Items of the Table after the first Order
        /// </summary>
        public Func<T, IComparable> OrderItemsThenBy { get; set; } = x => 0;
        /// <summary>
        /// The Function that Orders the Items of the Table after the second Order
        /// </summary>
        public Func<T, IComparable> OrderItemsThenBy2 { get; set; } = x => 0;
        public static ReportTableOptions<T> DefaultOptions() => new();

        public ReportTableOptions<T> GetDeepClone()
        {
            var clone = (ReportTableOptions<T>)MemberwiseClone();
            clone.ColumnsOptions = ColumnsOptions.GetDeepClone();
            clone.TableTitleStyle = TableTitleStyle.GetDeepClone();
            return clone;
        }
    }
    
}