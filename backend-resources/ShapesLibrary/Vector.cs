using CommonHelpers.Comparers;
using ShapesLibrary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static ShapesLibrary.Services.MathCalculations;

namespace ShapesLibrary
{
    /// <summary>
    /// Represents a two-dimensional vector with X and Y components.
    /// </summary>
    public struct Vector2D : IEquatable<Vector2D>
    {
        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2D"/> struct with specified X and Y components.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2D"/> struct calculating the X and Y components from the Provided start and End Points.
        /// </summary>
        /// <param name="start">The starting Point of the Vector.</param>
        /// <param name="end">The end Point of the Vector.</param>
        public Vector2D(PointXY start , PointXY end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2D"/> struct with both components set to zero.
        /// </summary>
        public Vector2D() : this(0, 0)
        {
        }

        #region Operator Overloads

        /// <summary>
        /// Adds two vectors component-wise.
        /// </summary>
        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);

        /// <summary>
        /// Subtracts one vector from another component-wise.
        /// </summary>
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);

        /// <summary>
        /// Negates the vector.
        /// </summary>
        public static Vector2D operator -(Vector2D a) => new Vector2D(-a.X, -a.Y);

        /// <summary>
        /// Multiplies the vector by a scalar.
        /// </summary>
        public static Vector2D operator *(Vector2D a, double scalar) => new Vector2D(a.X * scalar, a.Y * scalar);

        /// <summary>
        /// Multiplies the vector by a scalar.
        /// </summary>
        public static Vector2D operator *(double scalar, Vector2D a) => a * scalar;

        /// <summary>
        /// Divides the vector by a scalar.
        /// </summary>
        public static Vector2D operator /(Vector2D a, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return new Vector2D(a.X / scalar, a.Y / scalar);
        }

        /// <summary>
        /// Determines whether two vectors are equal.
        /// </summary>
        public static bool operator ==(Vector2D a, Vector2D b) => a.Equals(b);

        /// <summary>
        /// Determines whether two vectors are not equal.
        /// </summary>
        public static bool operator !=(Vector2D a, Vector2D b) => !a.Equals(b);

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the magnitude (length) of the vector.
        /// </summary>
        /// <returns>The magnitude of the vector.</returns>
        public readonly double Magnitude() => Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// Returns the squared magnitude of the vector.
        /// </summary>
        /// <returns>The squared magnitude.</returns>
        public readonly double MagnitudeSquared() => X * X + Y * Y;

        /// <summary>
        /// Returns a normalized (unit) vector in the same direction.
        /// </summary>
        /// <returns>The normalized vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown when trying to normalize a zero vector.</exception>
        public readonly Vector2D Normalize()
        {
            double magnitude = Magnitude();
            if (magnitude == 0) return Vector2D.Zero;
            
            return this / magnitude;
        }

        /// <summary>
        /// Calculates the dot product of this vector with another.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        public readonly double Dot(Vector2D other) => X * other.X + Y * other.Y;
        /// <summary>
        /// Calculates the dot product of this vector with a point (a vector starting from 0,0 ending the point)
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product.</returns>
        public readonly double Dot(PointXY other) => X * other.X + Y * other.Y;

        /// <summary>
        /// Calculates the Cross product of this vector with another vector
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly double Cross(Vector2D other) => X * other.Y - Y * other.X;

        /// <summary>
        /// Calculates the distance between this vector and another.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        public readonly double DistanceTo(Vector2D other) => (this - other).Magnitude();

        /// <summary>
        /// Returns the angle in radians between this vector and another.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The angle in radians.</returns>
        /// <exception cref="InvalidOperationException">Thrown when one of the vectors is zero.</exception>
        public readonly double AngleBetween(Vector2D other)
        {
            double magA = Magnitude();
            double magB = other.Magnitude();

            if (magA == 0 || magB == 0)
                throw new InvalidOperationException("Cannot calculate angle with zero vector.");

            double dotProduct = Dot(other);
            // Clamp the cosine to the interval [-1,1] to avoid NaN due to floating point errors
            double cosTheta = Math.Max(-1.0, Math.Min(1.0, dotProduct / (magA * magB)));
            return Math.Acos(cosTheta);
        }

        /// <summary>
        /// Projects this vector onto another vector.
        /// </summary>
        /// <param name="other">The vector to project onto.</param>
        /// <returns>The projection vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown when projecting onto a zero vector.</exception>
        public readonly Vector2D ProjectOnto(Vector2D other)
        {
            double magSquared = other.MagnitudeSquared();
            if (magSquared == 0)
                throw new InvalidOperationException("Cannot project onto a zero vector.");
            double scalar = Dot(other) / magSquared;
            return other * scalar;
        }

        /// <summary>
        /// Rotates the vector by the specified angle in radians.
        /// </summary>
        /// <param name="radians">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public readonly Vector2D Rotate(double radians)
        {
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            double newX = X * cos - Y * sin;
            double newY = X * sin + Y * cos;
            return new Vector2D(newX, newY);
        }

        /// <summary>
        /// Determines weather two vectors lie in the same line
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool IsCollinear(Vector2D other)
        {
            return Math.Abs(CrossProduct(this, other)) < DoubleSafeEqualityComparer.DefaultEpsilon; //to avoid floating point errors
        }
        /// <summary>
        /// Returns a vector with the same direction but with a magnitude of 1.
        /// </summary>
        /// <returns>The normalized vector.</returns>
        public readonly Vector2D UnitVector => Normalize();
        /// <summary>
        /// Returns the left-hand normal (perpendicular vector) of the current vector.
        /// </summary>
        /// <returns>The left-hand normal vector.</returns>
        public readonly Vector2D GetNormalCounterClockwise() => new(-Y, X);

        /// <summary>
        /// Returns the right-hand normal (perpendicular vector) of the current vector.
        /// </summary>
        /// <returns>The right-hand normal vector.</returns>
        public readonly Vector2D GetNormalClockwise() => new(Y, -X);
        /// <summary>
        /// Returns the normal (perpendicular vector) of the current vector to the desired orientation.
        /// </summary>
        /// <returns>The normal vector</returns>
        public readonly Vector2D GetNormal(bool clockwise = true) => clockwise ? GetNormalClockwise() : GetNormalCounterClockwise();

        /// <summary>
        /// Returns a string that represents the current vector.
        /// </summary>
        /// <returns>A string in the format (X, Y).</returns>
        public override readonly string ToString() => $"({X:0.###}, {Y:0.###})";

        /// <summary>
        /// Determines whether the specified object is equal to the current vector.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>true if equal; otherwise, false.</returns>
        public override readonly bool Equals(object? obj)
        {
            if (obj is Vector2D other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this vector.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override readonly int GetHashCode() => HashCode.Combine(X, Y);

        #endregion

        #region IEquatable<Vector2D> Members

        /// <summary>
        /// Determines whether the specified vector is equal to the current vector.
        /// </summary>
        /// <param name="other">The vector to compare with.</param>
        /// <returns>true if equal; otherwise, false.</returns>
        public readonly bool Equals(Vector2D other) => 
            Math.Abs(X - other.X) < DoubleSafeEqualityComparer.DefaultEpsilon && 
            Math.Abs(Y - other.Y) < DoubleSafeEqualityComparer.DefaultEpsilon;

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns a zero vector (0, 0).
        /// </summary>
        public static Vector2D Zero => new Vector2D(0, 0);

        /// <summary>
        /// Returns a unit vector in the X direction (1, 0).
        /// </summary>
        public static Vector2D UnitX => new Vector2D(1, 0);

        /// <summary>
        /// Returns a unit vector in the Y direction (0, 1).
        /// </summary>
        public static Vector2D UnitY => new Vector2D(0, 1);

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector2D a, Vector2D b) => a.Dot(b);

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>The distance between the vectors.</returns>
        public static double Distance(Vector2D a, Vector2D b) => a.DistanceTo(b);

        /// <summary>
        /// Returns the angle in radians between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>The angle in radians.</returns>
        public static double AngleBetween(Vector2D a, Vector2D b) => a.AngleBetween(b);

        /// <summary>
        /// Projects vector a onto vector b.
        /// </summary>
        /// <param name="a">The vector to project.</param>
        /// <param name="b">The vector to project onto.</param>
        /// <returns>The projection of a onto b.</returns>
        public static Vector2D Project(Vector2D a, Vector2D b) => a.ProjectOnto(b);

        /// <summary>
        /// Rotates a vector by a given angle in radians.
        /// </summary>
        /// <param name="v">The vector to rotate.</param>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2D Rotate(Vector2D v, double radians) => v.Rotate(radians);
        /// <summary>
        /// Defines a way to determine the position of B relative to A using the 2D cross product.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// CrossProduct > 0 : B is left of A (counterclockwise rotation from A to B)
        /// CrossProduct < 0 : B is right of A (clockwise rotation from A to B)
        /// CrossProduct = 0 : A and B are collinear (on the same line)
        /// </returns>
        public static double CrossProduct(Vector2D a, Vector2D b)
        {
            return a.Cross(b);
        }

        #endregion
    }

    /// <summary>
    /// The Complete Information of a Vector including its start and End Points
    /// </summary>
    public struct PositionVector
    {
        public readonly PointXY Start { get; }
        public readonly Vector2D Vector { get; }
        public readonly PointXY End { get; }

        public PositionVector(PointXY start, PointXY end)
        {
            Start = start;
            End = end;
            Vector = new(start, end);
        }

        /// <summary>
        /// Returns weather a Vector intersects with another Vector
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public readonly bool Intersects(PositionVector other)
        {
            return MathCalculations.Vectors.DoVectorsIntersect(this, other);
        }
        /// <summary>
        /// Returns the intersection point of this Vector with another Vector
        /// </summary>
        /// <param name="other"></param>
        /// <param name="intersectionPoint"></param>
        /// <returns></returns>
        public readonly bool TryGetIntersectionPoint(PositionVector other, out PointXY? intersectionPoint)
        {
            intersectionPoint = null;

            if (this.Intersects(other))
            {
                PointXY? intersection = GetIntersection(other);
                if (intersection != null)
                {
                    intersectionPoint = intersection.Value;
                    return true;
                }
                else
                {
                    intersectionPoint = HandleCollinearVectorsOverlap(this, other);
                    if (intersectionPoint != null) return true; // overlapping collinear vectors
                    else return false; //non overlapping collinear vectors
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the intersection point of this Vector with another Vector
        /// This method does not actually check if the Vectors intersect , it just calculates the intersection point with the lines that the Vectors lie in 
        /// So if the vectors do not intersect but are not parallel , the method will return the intersection point of the lines that the vectors lie in (which would be out of them)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private readonly PointXY? GetIntersection(PositionVector other)
        {
            //Denominator Calculation: This part calculates the denom value, which is crucial for determining whether the line segments intersect.
            //The expression calculates the determinant of the matrix formed by the direction vectors of the two segments/vectors.
            double denominator = other.Vector.Y * this.Vector.X - other.Vector.X * this.Vector.Y;
            if (Math.Abs(denominator) < DoubleSafeEqualityComparer.DefaultEpsilon) return null; // Parallel Lines

            //These parameters help find the exact intersection point along the two line segments.
            //t represents the position along the first segment (v1), where the intersection occurs.
            //u represents the position along the second segment (v2), where the intersection occurs.
            double t = (other.Vector.Y * (Start.X - other.Start.X) - other.Vector.X * (Start.Y - other.Start.Y)) / denominator;
            double u = (this.Vector.X * (this.Start.Y - other.Start.Y) - this.Vector.Y * (this.Start.X - other.Start.X)) / denominator;

            //Here, we check if the calculated parameters t and u are within the range [0, 1].
            //If t and u are both between 0 and 1, it means that the intersection point lies within the bounds of both line segments.
            if (t >= - DoubleSafeEqualityComparer.DefaultEpsilon && 
                t <= 1 + DoubleSafeEqualityComparer.DefaultEpsilon && 
                u >= -DoubleSafeEqualityComparer.DefaultEpsilon && 
                u <= 1 + DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                //Intersection Point
                double intersectionX = Start.X + t * this.Vector.X;
                double intersectionY = Start.Y + t * this.Vector.Y;
                return new PointXY(intersectionX, intersectionY);
            }
            return null;
        }
        /// <summary>
        /// Returns the first point of overlap bewtween the two vectors (its always on of the edges) 
        /// Returns null of the Vectors do not Overlap
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static PointXY? HandleCollinearVectorsOverlap(PositionVector v1, PositionVector v2)
        {
            // Find the overlap between the two collinear segments
            double v1MinX = Math.Min(v1.Start.X, v1.End.X);
            double v1MaxX = Math.Max(v1.Start.X, v1.End.X);
            double v2MinX = Math.Min(v2.Start.X, v2.End.X);
            double v2MaxX = Math.Max(v2.Start.X, v2.End.X);

            // If the projections on the x-axis don't overlap, return null
            if (v1MaxX < v2MinX - DoubleSafeEqualityComparer.DefaultEpsilon || v2MaxX < v1MinX - -DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return null;
            }

            // Find the min and max of y coordinates for both segments
            double v1MinY = Math.Min(v1.Start.Y, v1.End.Y);
            double v1MaxY = Math.Max(v1.Start.Y, v1.End.Y);
            double v2MinY = Math.Min(v2.Start.Y, v2.End.Y);
            double v2MaxY = Math.Max(v2.Start.Y, v2.End.Y);

            // If the projections on the y-axis don't overlap, return null
            if (v1MaxY < v2MinY - DoubleSafeEqualityComparer.DefaultEpsilon || v2MaxY < v1MinY - DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return null;
            }

            // Calculate the first point of overlap (the maximum of the two starting points)
            double firstOverlapX = Math.Max(v1MinX, v2MinX);
            double firstOverlapY = Math.Max(v1MinY, v2MinY);

            // Return the first point of overlap
            return new PointXY(firstOverlapX, firstOverlapY);
        }
    }

}
