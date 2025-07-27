using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    /*
     * 1.Whenever we Add a Value assign a number This way it gets Sorted
     * 2.NEVER CHANGE THE NameString (ex. Draw9S) !!!!!! (This is what gets saved to the Database,
     changing it means we Must Change All the Records also to the database

     * 3.Change the Assinged Numbers to Change the Ordering in the ExcelFile Creation
     * 4.Change the Description to Change the View String
     * 5.WHENEVER ADDING A NEW ITEM ==> Its Appending String must be Also Added in the GlassClass (ex. Φ20mm - 4Τρυπες )
     */
    public enum GlassDrawEnum
    {
        DrawNotSet = 0,
        /// <summary>
        /// Fixed Glass No Holes
        /// </summary>
        [Description("F")]
        DrawF = 1,
        /// <summary>
        /// Door Bronze 6000 Sliding Models except 94
        /// </summary>
        [Description("9S")]
        Draw9S = 2,
        /// <summary>
        /// Draw of Door for 94 Model
        /// </summary>
        [Description("94")]
        Draw94 = 3,
        /// <summary>
        /// Door of 9B Model
        /// </summary>
        [Description("9Β")]
        Draw9B = 4,
        /// <summary>
        /// Draw of Inox 304 Sliding Models
        /// </summary>
        [Description("VS")]
        DrawVS = 5,
        /// <summary>
        /// Fixed of Inox304 Models except VF
        /// </summary>
        [Description("VA")]
        DrawVA = 6,
        /// <summary>
        /// Fixed of VF Model
        /// </summary>
        [Description("VF")]
        DrawVF = 7,
        /// <summary>
        /// Door of WS Model
        /// </summary>
        [Description("WS")]
        DrawWS = 8,
        /// <summary>
        /// Fixed of HB Model
        /// </summary>
        [Description("HB1")]
        DrawHB1 = 9,
        /// <summary>
        /// Door of HB Model
        /// </summary>
        [Description("ΗΒ2")]
        DrawHB2 = 10,
        /// <summary>
        /// Second Glass of NP Model
        /// </summary>
        [Description("DP1")]
        DrawDP1 = 11,
        /// <summary>
        /// First Glass of NP Model
        /// </summary>
        [Description("DP3")]
        DrawDP3 = 12,
        /// <summary>
        /// Door of NB Model
        /// </summary>
        [Description("NB")]
        DrawNB = 13,
        /// <summary>
        /// Door of DB Model
        /// </summary>
        [Description("DB")]
        DrawDB = 14,
        
        /// <summary>
        /// Fixed For W Models Without Profile
        /// </summary>
        [Description("H1")]
        DrawH1 = 15,
        /// <summary>
        /// Door of Flipper Panel
        /// </summary>
        [Description("FL")]
        DrawFL = 16,
        /// <summary>
        /// Door of 9C Models
        /// </summary>
        [Description("9C")]
        Draw9C = 17,
        /// <summary>
        /// Door or Fixed for NV Model
        /// </summary>
        [Description("NV")]
        DrawNV = 18,

    }
}
