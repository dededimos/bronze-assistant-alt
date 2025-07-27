using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteApplicationSettings.DTOs
{
    public class SearchOrdersViewSettingsDTO :DTO
    {
        /// <summary>
        /// The Maximum Results when retrievin the Small Orders Result
        /// </summary>
        public int MaxResultsGetSmallOrders { get; set; }
        /// <summary>
        /// Weather this is the Default Setting
        /// </summary>
        public bool IsDefault { get; set; }

        public void CopySettings(SearchOrdersViewSettingsDTO other)
        {
            MaxResultsGetSmallOrders= other.MaxResultsGetSmallOrders;
        }
    }
}
