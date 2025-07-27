using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.Helpers
{
    public static class StylesStaticInfoMirrors
    {
        /// <summary>
        /// The Variant Used in the Boxes of the Application (Numeric Fields/ComboBoxes E.t.c.)
        /// </summary>
        public static readonly Variant ComboBoxesVariant = Variant.Filled;

        /// <summary>
        /// The Css Style of all the Dialog Containers
        /// </summary>
        public static readonly string DialogContainerCssStyle = "max-height:70vh;overflow-y:auto;";

        /// <summary>
        /// The Dialog Action Buttons Variant and Color
        /// </summary>
        public static readonly Variant ActionButtonVariant = Variant.Filled;
        public static readonly Color ActionButtonColor = Color.Primary;

        /// <summary>
        /// The CheckBoxes Color
        /// </summary>
        public static readonly Color CheckBoxesColor = Color.Primary;
    }
}
