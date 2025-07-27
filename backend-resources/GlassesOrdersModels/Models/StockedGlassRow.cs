using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlassesOrdersModels.Models
{
    public class StockedGlassRow
    {
        public Glass Glass { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string RowId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;

        /// <summary>
        /// How much Quantity of this is currently in Use
        /// </summary>
        public int InUseQuantity { get; set; }

        public StockedGlassRow(string rowId, 
            Glass glass , 
            int quantity , 
            string notes , 
            DateTime created , 
            DateTime lastModified)
        {
            Glass = glass;
            RowId = rowId;
            Quantity = quantity;
            Notes = notes;
            Created = created;
            LastModified = lastModified;
        }
    }
}
