using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Enums
{
    /// <summary>
    /// An option to determine which Anchorline to prefer between two parallel lines
    /// </summary>
    public enum AnchorLinePreferenceOption
    {
        PreferGreaterXAnchorline,
        PreferGreaterYAnchorline,
        PreferSmallerXAnchorline,
        PreferSmallerYAnchorline,
        //For double cases
        PreferSmallerXGreaterYAnchorline,
        PreferGreaterXSmallerYAnchorline,
        PreferSmallerXSmallerYAnchorline,
        PreferGreaterXGreaterYAnchorline,
    }
}
