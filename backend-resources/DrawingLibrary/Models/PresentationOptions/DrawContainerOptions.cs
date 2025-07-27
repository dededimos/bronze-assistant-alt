using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions
{
    public class DrawContainerOptions : IDeepClonable<DrawContainerOptions>
    {
        /// <summary>
        /// The Length of the Container that Holds the Main Draw
        /// </summary>
        public double ContainerLength { get; set; } = 420;
        /// <summary>
        /// The Height of the Container that Holds the Main Draw
        /// </summary>
        public double ContainerHeight { get; set; } = 420;
        /// <summary>
        /// The Margin Top-Left-Right-Bottom of the Container
        /// <para>Dimensions are not getting scaled or Calculated in Bounding Boxes . So if the Draw gets Scaled to the Container</para>
        /// <para>and it Actually gets equal to it when scaled , the Dimensions and any applied shadows get Clipped</para>
        /// <para>So when scaling its better to leave Padding for those things to be Visible</para>
        /// <para>Default is 80</para>
        /// <para>This Margin should be actually set to the UI As padding of the container containing the Draw and with ClipToBounds = true</para>
        /// </summary>
        public double[] ContainerMargin { get => [ContainerMarginLeft, ContainerMarginTop, ContainerMarginRight, ContainerMarginBottom]; }
        public double ContainerMarginTop { get; set; } = 80;
        public double ContainerMarginBottom { get; set; } = 80;
        public double ContainerMarginLeft { get; set; } = 80;
        public double ContainerMarginRight { get; set; } = 80;
        /// <summary>
        /// The Maximum Dimension after which the Draw appears the same 
        /// <para>Ex.A 600mm Dimension as maximum means that all draws after 600mm will look the same in the Container</para>
        /// <para>This is the Maximum for either Length or Height of the Container</para>
        /// <para>0 , Value means the maximum dimension is those of the Container</para>
        /// <para>Default = 1000 </para>
        /// </summary>
        public double MaxDimensionDepictedToScale { get; set; } = 1000;

        public static DrawContainerOptions GetDefaults()
        {
            return new()
            {
                ContainerLength = 500,
                ContainerHeight = 500,
                ContainerMarginTop = 80,
                ContainerMarginBottom = 80,
                ContainerMarginLeft = 80,
                ContainerMarginRight = 80,
                MaxDimensionDepictedToScale = 1000,
            };
        }
        public DrawContainerOptions GetDeepClone()
        {
            return (DrawContainerOptions)this.MemberwiseClone();
        }
    }
}
