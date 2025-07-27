
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Supports
{
    public class MirrorSupportInstructions : IEqualityComparerCreator<MirrorSupportInstructions>, IDeepClonable<MirrorSupportInstructions>
    {
        public SupportLengthOption LengthOption { get; set; }
        public double LengthOptionValue { get; set; }
        public SupportVerticalDistanceOption VerticalDistanceOption { get; set; }
        public double VerticalDistanceOptionValue { get; set; }
        public DistanceBetweenSupportsOption DistanceBetweenOption { get; set; }
        public double DistanceBetweenOptionValue { get; set; }
        public int SupportsNumber { get; set; }
        public double Thickness { get; set; }
        public double Depth { get; set; }

        /// <summary>
        /// Gets the Length of the support according to the instructions
        /// </summary>
        /// <param name="parentBox">The Parent's BoundingBox that the support is placed into</param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public double GetSupportLength(RectangleInfo parentBox)
        {
            var length = LengthOption switch
            {
                SupportLengthOption.FixedLengthOption => LengthOptionValue,
                SupportLengthOption.AsPercentageOfParentsLengthOption => LengthOptionValue/100 * parentBox.Length,
                _ => throw new EnumValueNotSupportedException(LengthOption),
            };
            return length;
        }

        /// <summary>
        /// Gets the Y Location of the support placed inside a certain parent
        /// </summary>
        /// <param name="parentBox">The Parent's BoundingBox that the support is placed into</param>
        private double GetYLocation(RectangleInfo parentBox)
        {
            var y = VerticalDistanceOption switch
            {
                SupportVerticalDistanceOption.FixedDistanceFromParentTop => parentBox.TopY + (VerticalDistanceOptionValue + Thickness / 2d),
                SupportVerticalDistanceOption.FixedDistanceFromParentBottom => parentBox.BottomY - (VerticalDistanceOptionValue + Thickness / 2d),
                SupportVerticalDistanceOption.FixedDistanceFromParentCenterTop => parentBox.LocationY - (VerticalDistanceOptionValue + Thickness / 2d),
                SupportVerticalDistanceOption.FixedDistanceFromParentCenterBottom => parentBox.LocationY + (VerticalDistanceOptionValue + Thickness / 2d),
                SupportVerticalDistanceOption.DistanceFromTopAsPercentageOfParentHeight => parentBox.TopY + (VerticalDistanceOptionValue / 100 * parentBox.Height + Thickness / 2d),
                SupportVerticalDistanceOption.DistanceFromBottomAsPercentageOfParentHeight => parentBox.BottomY - (VerticalDistanceOptionValue / 100 * parentBox.Height + Thickness / 2d),
                SupportVerticalDistanceOption.DistanceFromCenterTopAsPercentageOfParentHeight => parentBox.LocationY - (VerticalDistanceOptionValue / 100 * parentBox.Height + Thickness / 2d),
                SupportVerticalDistanceOption.DistanceFromCenterBottomAsPercentageOfParentHeight => parentBox.LocationY + (VerticalDistanceOptionValue / 100 * parentBox.Height + Thickness / 2d),
                _ => throw new EnumValueNotSupportedException(VerticalDistanceOption),
            };
            return y;
        }

        /// <summary>
        /// Calculates the distance that horizontal supports deriving from these instructions can have between them
        /// </summary>
        /// <param name="parentBox">The Parent's BoundingBox that the support is placed into</param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        private double GetBetweenDistance(RectangleInfo parentBox)
        {
            //Calculate the distance between each Support
            var distance = DistanceBetweenOption switch
            {
                DistanceBetweenSupportsOption.FixedDistanceBetweenSupports => DistanceBetweenOptionValue,
                DistanceBetweenSupportsOption.AsPercentageOfParentLength => DistanceBetweenOptionValue / 100 * parentBox.Length,
                _ => throw new EnumValueNotSupportedException(DistanceBetweenOption),
            };
            return distance;
        }

        /// <summary>
        /// Returns the X Coordinate of each support deriving from these instructions
        /// </summary>
        /// <param name="parentBox"></param>
        /// <returns></returns>
        private List<double> GetXLocations(RectangleInfo parentBox, double supportLength, double betweenDistance)
        {
            List<double> xLocations = new();

            if (SupportsNumber == 1)
            {
                xLocations.Add(parentBox.LocationX);
            }
            else if (SupportsNumber > 1)
            {
                //Create a rectangle that represents the bounding box containing all the supports horizontally
                //To Make this find the total length that the Supports and their distances form 
                //The Formula would be NumberOfSupports * lengthsOfSupports + (NumberOfSupports - 1) * distanceBetweenSupports
                var totalLength = SupportsNumber * supportLength + (SupportsNumber - 1) * betweenDistance;
                var container = new RectangleInfo(totalLength, 1, 0, parentBox.LocationX, parentBox.LocationY);
                
                //the first X is the containers left + half the support length
                double currentX = container.LeftX + supportLength/2d;
                xLocations.Add(currentX);
                
                //then each subsequent support has an X of the previous + supportLength + betweenDistance
                for (int i = 1; i <= SupportsNumber; i++)
                {
                    if (i == 1) continue;
                    currentX += supportLength + betweenDistance;
                    xLocations.Add(currentX);
                }
            }
            return xLocations;
        }

        public List<RectangleInfo> GetSupportShapes(RectangleInfo parentBox)
        {
            List<RectangleInfo> supports = [];
            //Get the length of each Support
            var length = GetSupportLength(parentBox);
            //Get the distance between each Support if There are more than 1 supports
            var betweenDistance = SupportsNumber > 1 ? GetBetweenDistance(parentBox) : 0;
            // Get the X Coordinates of each support
            var xLocations = GetXLocations(parentBox, length, betweenDistance);
            // Get the Y Location of each support (all have the same)
            var y = GetYLocation(parentBox);
            // Create the Support rectangle shapes and return them
            foreach (var x in xLocations)
            {
                var rect = new RectangleInfo(length, Thickness, 0, x, y);
                supports.Add(rect);
            }
            return supports;
        }

        public static IEqualityComparer<MirrorSupportInstructions> GetComparer()
        {
            return new MirrorSupportInstructionsEqualityComparer();
        }

        public MirrorSupportInstructions GetDeepClone()
        {
            return (MirrorSupportInstructions)MemberwiseClone();
        }
    }
    public class MirrorSupportInstructionsEqualityComparer : IEqualityComparer<MirrorSupportInstructions>
    {
        public bool Equals(MirrorSupportInstructions? x, MirrorSupportInstructions? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.LengthOption == y.LengthOption &&
                x.LengthOptionValue == y.LengthOptionValue &&
                x.VerticalDistanceOption == y.VerticalDistanceOption &&
                x.VerticalDistanceOptionValue == y.VerticalDistanceOptionValue &&
                x.DistanceBetweenOption == y.DistanceBetweenOption &&
                x.DistanceBetweenOptionValue == y.DistanceBetweenOptionValue &&
                x.SupportsNumber == y.SupportsNumber &&
                x.Thickness == y.Thickness &&
                x.Depth == y.Depth;
        }

        public int GetHashCode([DisallowNull] MirrorSupportInstructions obj)
        {
            int hash = HashCode.Combine(obj.LengthOption, obj.LengthOptionValue, obj.VerticalDistanceOption, obj.VerticalDistanceOptionValue, obj.DistanceBetweenOption);
            return HashCode.Combine(hash, obj.DistanceBetweenOptionValue, obj.SupportsNumber, obj.Thickness, obj.Depth);
        }
    }

}

