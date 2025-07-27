using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum GlassFinishEnum
    {
        GlassFinishNotSet = 0, //ADDED ON 09-12-2022 , IF NEEDS REVERTING JUST REMOVE AND PUT 0 VALUE IN TRANSPARENT AND -1 TO ALL THE OTHERS
        [Description("Διάφανο")]
        Transparent = 1,
        [Description("Σατινέ")]
        Satin = 2,
        [Description("Σεριγραφεία")]
        Serigraphy = 3,
        [Description("Φιμέ")]
        Fume = 4,
        [Description("Frosted")]
        Frosted = 5,
        [Description("Ειδικό")]
        Special = 6
        
       //(OLD WPF APP NOTE) DO NOT CHANGE NUMBERING , IT IS STORED IN THE DATABASE . IT IS ALSO USED TO SORT THE GLASSES BY THIS ORDER.
    }
}
