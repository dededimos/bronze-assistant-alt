using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    /// <summary>
    /// The Model of a Cabin (Numbers are saved to SQLite -- Strings Are Saved to Mongo so all the fileds CANNOT BE CHANGED only new ones can be added)
    /// </summary>
    public enum CabinModelEnum
    {
        [Description("9A-Γωνιακή Β6000-8000")]
        Model9A = 0,
        [Description("9S-Σταθερό Συρόμενο Β6000-8000")]
        Model9S = 1,
        [Description("94-Διπλό Συρόμενο Β6000-8000")]
        Model94 = 2,
        [Description("9F-Σταθερό Πλαϊνό Β6000-8000")]
        Model9F = 3,
        [Description("9B-Ανοιγόμενη Πόρτα Β6000-8000")]
        Model9B = 4,
        [Description("W-Σταθερό")]
        ModelW = 5,
        [Description("ΗΒ-Σταθερό με Ανοιγόμενη Πόρτα")]
        ModelHB = 6,
        [Description("NP-Αναδιπλούμενη Πόρτα")]
        ModelNP = 7,
        [Description("VS-Σταθερό Συρόμενο Inox304")]
        ModelVS = 8,
        [Description("VF-Σταθερό Πλαϊνό Inox304")]
        ModelVF = 9,
        [Description("V4-Διπλό Συρόμενο Inox304")]
        ModelV4 = 10,
        [Description("VΑ-Γωνιακή Inox304")]
        ModelVA = 11,
        [Description("WS-Smart Inox")]
        ModelWS = 12,
        [Description("Ε-Σταθερό Ελεύθερο")]
        ModelE = 13,
        [Description("WFlipper-Πρόσθετο Πάνελ")]
        ModelWFlipper = 14,
        [Description("DB-Ανοιγόμενη Πόρτα Μεντεσέδες")]
        ModelDB = 15,
        [Description("ΝB-Ανοιγόμενη Πόρτα Μηχαν.Αλουμινίου")]
        ModelNB = 16,
        ModelNV = 17,
        ModelMV2 = 18,
        ModelNV2 = 19,
        Model6WA = 20,
        [Description("9C-Ημικυκλική Β6000-8000")]
        Model9C = 21,
        [Description("Σταθερό Μπανιέρας")]
        Model8W40 =22,
        /// <summary>
        /// Represents a container masked as a Cabin to Hold a Glass
        /// </summary>
        ModelGlassContainer = 23,
        //Added On 15-12-2023 New Models
        ModelQB = 24,
        ModelQP = 25,

        
    }
}
