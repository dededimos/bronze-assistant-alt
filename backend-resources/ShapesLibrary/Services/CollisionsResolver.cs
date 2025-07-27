using CommonHelpers.Comparers;
using Microsoft.Extensions.Logging;
using ShapesLibrary.Interfaces;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.Services
{
    public static class CollisionsResolver
    {
        /// <summary>
        /// Resolves a collision between a moving shape and a list of fixed shapes
        /// </summary>
        /// <param name="movingShape">The shape that will be moved if its location intersects with other shapes/boundary or if its out of the boundary</param>
        /// <param name="fixedShapes">The various shapes already inside the boundary</param>
        /// <param name="target">The Target point towards which we need to move the shape to resolve the collision</param>
        /// <param name="boundary">The shape of the boundary</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool ResolveCollisionWithBoundaries(ShapeInfo movingShape, List<ShapeInfo> fixedShapes, PointXY target, ShapeInfo boundary, ILogger logger)
        {
            logger.LogInformation("Placing shape {shape} into {moveShapeLocation} , on boundary {boundary} with location {boundaryLocation}", movingShape.ShapeType, movingShape.GetLocation(), boundary.ShapeType, boundary.GetLocation());

            //Check if the moving shape is within boundary
            if (!boundary.Contains(movingShape))
            {
                logger.LogInformation("Shape is outside or is intersecting the Boundary");
                //Move the Shape into the Boundary
                //Find the Vector of the direction (shape center => point)
                Vector2D directionVector = new Vector2D(movingShape.GetCentroid(), target).Normalize();
                double translation = ComputeMinimalTranslationShapeIntoShape(boundary, movingShape,directionVector);
                if (translation != double.PositiveInfinity)
                {
                    movingShape.Translate(directionVector * translation);
                }
                else throw new Exception("Cannot move into Boundary");
                //MoveShapeIntoBoundary(movingShape, boundary, target, logger);
            }

            bool initialCollision = fixedShapes.Any(shape =>
            {
                bool intersects = shape.IntersectsWithShape(movingShape);
                if (intersects) logger.LogInformation("COLLISION DETECTED with {shape} at {location}", shape.ShapeType, shape.GetLocation());
                return intersects;
            });

            //If no collisions return as resolved;
            if (!initialCollision) return true;

            //Directions to try and resolve collision
            List<Vector2D> directionsToTry = [];
            //Add initial direction as pointed by the argument
            var initialDirection = new Vector2D(movingShape.GetCentroid(), target).Normalize();
            directionsToTry.Add(initialDirection);

            //Add also the inverse and normal directions
            directionsToTry.Add(-initialDirection);
            directionsToTry.Add(initialDirection.GetNormalClockwise());
            directionsToTry.Add(initialDirection.GetNormalCounterClockwise());

            //Try each direction
            foreach (var direction in directionsToTry)
            {
                logger.LogInformation("Resolving Collision on Direction : {direction}", direction);
                bool collisionResolved = TryResolveCollisionExact(movingShape, fixedShapes, direction, boundary, logger);
                if (collisionResolved)
                {
                    logger.LogInformation("Collision Resolved");
                    return true;
                }
            }
            //if none of the directions work , the shape cannot be placed
            logger.LogInformation("COLLISION RESOLVE FAILED");
            logger.LogInformation("Could not Resolve Collision in any direction...");
            return false;
        }

        /// <summary>
        /// Moves a shape into another shape , if the shape is not contained already in it
        /// </summary>
        /// <param name="movingShape">The Shape to move inside the boundary</param>
        /// <param name="boundaryShape">The shape of the Boundary</param>
        /// <param name="targetPoint">The point towards which to move the Shape to enter the boundary</param>
        /// <exception cref="InvalidOperationException">When there is no valid movement towards the selected direction to contain the shape into the boundary</exception>
        private static void MoveShapeIntoBoundary(ShapeInfo movingShape, ShapeInfo boundaryShape, PointXY targetPoint, ILogger logger)
        {
            //Find the Vector of the direction (shape center => point)
            Vector2D direction = new Vector2D(movingShape.GetCentroid(), targetPoint).Normalize();

            //Get and Estimated Size of movement for the Shape (10% of its size)
            double stepSize = movingShape.GetSizeEstimate() / 10;
            int maxIterations = 100;
            int iterations = 0;

            while (!boundaryShape.Contains(movingShape) && iterations < maxIterations)
            {
                iterations++;
                movingShape.Translate(direction * stepSize);
            }

            if (!boundaryShape.Contains(movingShape))
            {
                logger.LogInformation("Could not move the shape into the boundary after max {iterations} iterations , and direction {direction}", iterations, direction);
            }
            else
            {
                var totalDisplacement = direction * (stepSize * iterations);
                logger.LogInformation("Moving Shape into boundary with total iterations {iterations}", iterations);
                logger.LogInformation("New Shape Position : {newLocation} , Total Displacement : {totalDisplacement}", movingShape.GetLocation(), totalDisplacement);
            }
        }

        private static bool TryResolveCollisionExact(ShapeInfo movingShape, List<ShapeInfo> fixedShapes, Vector2D direction, ShapeInfo boundary, ILogger logger)
        {
            double maxMovementAllowed = boundary.GetSizeEstimate();
            double requiredMovement = 0;

            foreach (var fixedShape in fixedShapes)
            {
                // Find the Minimal Translation
                double translation = ComputeMinimalTranslation(fixedShape, movingShape, direction);
                if (translation == double.PositiveInfinity) return false; //Cannot resolve collision in this direction

                //If the translation is greater than the movment already computed by the other shapes then update the required movement
                //Means the shape needs to move more in the current direction to resolve collisions
                if (translation > requiredMovement) requiredMovement = translation;

                //If the required movement is greater than the maximum allowed movement then return false
                if (requiredMovement > maxMovementAllowed)
                {
                    logger.LogInformation("Cannot resolve collision in this direction");
                    logger.LogInformation("Required Movement: { requiredMovement} > { maxMovementAllowed} :MaxAllowed", requiredMovement, maxMovementAllowed);
                    return false; // Cannot resolve collision in this direction , movement too much
                }
            }

            //Otherwise Move the shape and resolve collisions
            Vector2D displacement = direction * requiredMovement;
            movingShape.Translate(displacement);
            logger.LogInformation("Resolved Collision)");
            logger.LogInformation("New Location: {location} , Displacement: { displacement} ,Direction: {direction}, Required Movement: {reqMovement}", movingShape.GetLocation(), displacement, direction, requiredMovement);

            //Final Collision Check
            var containedInBoundary = boundary.Contains(movingShape);
            bool intersectsFixedShapes = false;

            //Run the check only if is inside boundary
            if (containedInBoundary)
            {
                intersectsFixedShapes = fixedShapes.Any(shape =>
                {
                    var intersects = movingShape.IntersectsWithShape(shape);
                    if (intersects) logger.LogInformation("COLLISION DETECTED AFTER INITIAL MOVEMENT , Intersecting with {shape} at {location}", shape.ShapeType, shape.GetLocation());
                    return intersects;
                });
            }
            else
            {
                logger.LogInformation("OUT OF BOUNDS , Shape is outside the boundary after movement");
            }

            if (!containedInBoundary || intersectsFixedShapes)
            {
                //move back
                movingShape.Translate(-displacement);
                logger.LogInformation("Moving back to previous Location : {location}", movingShape.GetLocation());
                return false;
            }



            //Collision Resolved
            return true;
        }

        /// <summary>
        /// Computes the Minimal Translation FACTOR required to resolve the collision between two shapes in a certain direction
        /// <para>This factor will be multiplied by the desired direction to provide the final translation</para>
        /// </summary>
        /// <param name="fixedShape">The Shape that is not moving</param>
        /// <param name="movingShape">The Shape that is moving</param>
        /// <param name="direction">The desired direction of movement</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedShapeInCalculationException"></exception>
        private static double ComputeMinimalTranslation(ShapeInfo fixedShape, ShapeInfo movingShape, Vector2D direction)
        {
            if (fixedShape is IRingShape ring)
            {
                //Attempt to move into the inner hole first
                double translationIntoHole = double.PositiveInfinity;

                //Identify the inner Shape
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                ShapeInfo innerShape = ring.GetInnerRingWholeShape() as ShapeInfo;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8604 // Possible null reference argument.
                translationIntoHole = ComputeMinimalTranslationShapeIntoShape(innerShape, movingShape, direction);
#pragma warning restore CS8604 // Possible null reference argument.
                if (translationIntoHole != double.PositiveInfinity)
                {
                    return translationIntoHole;
                }
                // If moving into the hole is not possible, resolve collision with the outer boundary as normal
                else
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    ShapeInfo outerShape = ring.GetOuterRingWholeShape() as ShapeInfo;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    //rerun the method as if the shape was not a ring!
#pragma warning disable CS8604 // Possible null reference argument.
                    return ComputeMinimalTranslation(outerShape, movingShape, direction);
#pragma warning restore CS8604 // Possible null reference argument.
                }
            }
            else if (fixedShape is PolygonInfo fixedPolygon)
            {
                return movingShape switch
                {
                    PolygonInfo movingPolygon => ComputeMinimalTranslationPolygonPolygon(fixedPolygon, movingPolygon, direction),
                    CircleInfo movingCircle => ComputeMinimalTranslationPolygonCircle(fixedPolygon, movingCircle, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationPolygonPolygon(fixedPolygon, movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslation)),
                };
            }
            else if (fixedShape is CircleInfo fixedCircle)
            {
                return movingShape switch
                {
                    PolygonInfo movingPolygon => ComputeMinimalTranslationCirclePolygon(fixedCircle, movingPolygon, direction),
                    CircleInfo movingCircle => ComputeMinimalTranslationCircleCircle(fixedCircle, movingCircle, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationCirclePolygon(fixedCircle, movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslation)),
                };
            }
            else if (fixedShape is IPolygonSimulatable fixedSimulatable)
            {
                return movingShape switch
                {
                    PolygonInfo movingPolygon => ComputeMinimalTranslationPolygonPolygon(fixedSimulatable.GetOptimalPolygonSimulation(), movingPolygon, direction),
                    CircleInfo movingCircle => ComputeMinimalTranslationPolygonCircle(fixedSimulatable.GetOptimalPolygonSimulation(), movingCircle, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationPolygonPolygon(fixedSimulatable.GetOptimalPolygonSimulation(), movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslation)),
                };
            }
            else throw new NotSupportedShapeInCalculationException(fixedShape, nameof(ComputeMinimalTranslation));
        }

        private static double ComputeMinimalTranslationPolygonPolygon(PolygonInfo fixedPolygon, PolygonInfo movingPolygon, Vector2D direction)
        {
            // Normalize direction
            Vector2D D = direction.Normalize();

            // Use SAT to find the minimal translation along D
            double minimalTranslation = double.PositiveInfinity;

            List<Vector2D> axes = [];
            axes.AddRange(movingPolygon.EdgesNormalAxesNormalized);
            axes.AddRange(fixedPolygon.EdgesNormalAxesNormalized);

            foreach (var axis in axes)
            {
                //Project both polygons onto the axis
                var proj1 = movingPolygon.GetProjectionOntoAxis(axis);
                var proj2 = fixedPolygon.GetProjectionOntoAxis(axis);

                //Compute Overlap
                var overlap = proj1.GetOverlap(proj2);
                if (overlap < DoubleSafeEqualityComparer.DefaultEpsilon) return 0; // No Collision , at least one axis with zero overlap
                else
                {
                    //project the Desired Direction onto the Axis (to find how much)
                    double directionProjection = D.Dot(axis);
                    if (Math.Abs(directionProjection) > DoubleSafeEqualityComparer.DefaultEpsilon)
                    {
                        //If the projection is non-Zero , calculate the minimal translation for THIS axis
                        double minimalTranslationCurrentAxis = overlap / Math.Abs(directionProjection);
                        if (minimalTranslationCurrentAxis < minimalTranslation) minimalTranslation = minimalTranslationCurrentAxis;
                    }
                }
            }
            //Return the minimal Translation found from all the axis (SAT , separating axis theorem)
            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationCirclePolygon(CircleInfo fixedCircle, PolygonInfo movingPolygon, Vector2D direction)
        {
            // Normalize direction
            Vector2D D = direction.Normalize();
            // Use SAT to find the minimal translation along D
            double minimalTranslation = double.PositiveInfinity;

            List<Vector2D> axes = [];
            //Add all the perpendicular axis to the edges
            axes.AddRange(movingPolygon.EdgesNormalAxesNormalized);
            //Add the closes axis to the circle
            axes.Add(movingPolygon.GetClosestNormalizedAxisFromPointToPolygon(fixedCircle.GetCentroid()));

            foreach (var axis in axes)
            {
                //Project Polygon and circle onto the Axis
                var proj1 = movingPolygon.GetProjectionOntoAxis(axis);
                var proj2 = fixedCircle.GetProjectionOntoAxis(axis);

                //Compute Overlap
                var overlap = proj1.GetOverlap(proj2);
                if (overlap < DoubleSafeEqualityComparer.DefaultEpsilon) return 0; // No Collision , at least one axis with zero overlap
                else
                {
                    //project the Desired Direction onto the Axis 
                    double directionProjection = D.Dot(axis);
                    if (Math.Abs(directionProjection) > DoubleSafeEqualityComparer.DefaultEpsilon)
                    {
                        //If the projection is non-Zero , calculate the minimal translation for THIS axis
                        double minimalTranslationCurrentAxis = overlap / Math.Abs(directionProjection);
                        if (minimalTranslationCurrentAxis < minimalTranslation) minimalTranslation = minimalTranslationCurrentAxis;
                    }
                }
            }
            //Return the minimal Translation found from all the axis (SAT , separating axis theorem)
            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationPolygonCircle(PolygonInfo fixedPolygon, CircleInfo movingCircle, Vector2D direction)
        {
            //Invert the problem from the CirclePolygon Method
            Vector2D invertedDirection = -direction;
            // and pass the circle as the fixed shape and the polygon as the moving shape ( we only get back the translation nothing else so no problem)
            return ComputeMinimalTranslationCirclePolygon(movingCircle, fixedPolygon, invertedDirection);
        }
        private static double ComputeMinimalTranslationCircleCircle(CircleInfo fixedCircle, CircleInfo movingCircle, Vector2D direction)
        {
            // Normalize the direction vector
            Vector2D D = direction.Normalize();

            // Vector from the center of the moving circle to the center of the other circle
            Vector2D C = new(movingCircle.GetCentroid(), fixedCircle.GetCentroid());

            // Project C onto D
            double cDotD = C.Dot(D);

            // The distance between the circles' centers along D
            double distanceAlongD = cDotD;

            // The overlap along D is the sum of radii minus the distance along D
            double overlap = movingCircle.Radius + fixedCircle.Radius - distanceAlongD;

            if (overlap < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                // No collision along this direction
                return 0;
            }

            // The minimal translation required is overlap
            // But we need to ensure that we move in the correct direction
            // Since D is normalized, the required movement s is overlap
            return overlap;
        }

        private static double ComputeMinimalTranslationShapeIntoShape(ShapeInfo fixedShape, ShapeInfo movingShape, Vector2D direction)
        {
            if (fixedShape is RectangleInfo rect)
            {
                return movingShape switch
                {
                    CircleInfo movingCircle => ComputeMinimalTranslationCircleIntoRectangle(rect, movingCircle, direction),
                    RectangleInfo movingRect => ComputeMinimalTranslationRectangleIntoRectangle(rect, movingRect, direction),
                    PolygonInfo movingPolygon => ComputeMinimalTranslationPolygonIntoPolygon(rect.GetPolygonSimulation(rect.OptimalSimulationSides), movingPolygon, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationPolygonIntoPolygon(rect.GetPolygonSimulation(rect.OptimalSimulationSides), movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslationShapeIntoShape)),
                };
            }
            else if (fixedShape is CircleInfo circle)
            {
                return movingShape switch
                {
                    CircleInfo movingCircle => ComputeMinimalTranslationCircleIntoCircle(circle, movingCircle, direction),
                    RectangleInfo movingRect => ComputeMinimalTranslationRectangleIntoCircle(circle, movingRect, direction),
                    PolygonInfo movingPolygon => ComputeMinimalTranslationPolygonIntoCircle(circle, movingPolygon, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationPolygonIntoCircle(circle, movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslationShapeIntoShape)),
                };
            }
            else if (fixedShape is PolygonInfo polygon)
            {
                return movingShape switch
                {
                    CircleInfo movingCircle => ComputeMinimalTranslationCircleIntoPolygon(polygon, movingCircle, direction),
                    PolygonInfo movingPolygon => ComputeMinimalTranslationPolygonIntoPolygon(polygon, movingPolygon, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationPolygonIntoPolygon(polygon, movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslationShapeIntoShape)),
                };
            }
            else if (fixedShape is IPolygonSimulatable simulatable)
            {
                return movingShape switch
                {
                    CircleInfo movingCircle => ComputeMinimalTranslationCircleIntoPolygon(simulatable.GetOptimalPolygonSimulation(), movingCircle, direction),
                    PolygonInfo movingPolygon => ComputeMinimalTranslationPolygonIntoPolygon(simulatable.GetOptimalPolygonSimulation(), movingPolygon, direction),
                    IPolygonSimulatable movingSimulatable => ComputeMinimalTranslationPolygonIntoPolygon(simulatable.GetOptimalPolygonSimulation(), movingSimulatable.GetOptimalPolygonSimulation(), direction),
                    _ => throw new NotSupportedShapeInCalculationException(movingShape, nameof(ComputeMinimalTranslationShapeIntoShape)),
                };
            }
            else throw new NotSupportedShapeInCalculationException(fixedShape, nameof(ComputeMinimalTranslationShapeIntoShape));
        }
        private static double ComputeMinimalTranslationRectangleIntoRectangle(RectangleInfo innerRectangle, RectangleInfo movingRectangle, Vector2D direction)
        {
            // Assume direction is normalized
            Vector2D D = direction.Normalize();

            // Calculate the minimal translation along X and Y
            double deltaX = 0;
            double deltaY = 0;

            // If moving right
            if (D.X > DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                deltaX = innerRectangle.RightX - movingRectangle.LeftX;
            }
            // If moving left
            else if (D.X < -DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                deltaX = innerRectangle.LeftX - movingRectangle.RightX;
            }

            // If moving down
            if (D.Y > DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                deltaY = innerRectangle.BottomY - movingRectangle.TopY;
            }
            // If moving up
            else if (D.Y < -DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                deltaY = innerRectangle.TopY - movingRectangle.BottomY;
            }

            // Compute translation needed along the direction
            double translationX = D.X != 0 ? deltaX / D.X : double.PositiveInfinity;
            double translationY = D.Y != 0 ? deltaY / D.Y : double.PositiveInfinity;

            // The minimal positive translation required
            double minimalTranslation = Math.Min(
                translationX > 0 ? translationX : double.PositiveInfinity,
                translationY > 0 ? translationY : double.PositiveInfinity
            );

            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationCircleIntoRectangle(RectangleInfo innerRectangle, CircleInfo movingCircle, Vector2D direction)
        {
            // Assume direction is normalized
            Vector2D D = direction.Normalize();

            // Find the closest point on the rectangle to the circle center
            PointXY closestPoint = MathCalculations.Rectangle.GetClosestPointOnPerimeterFromPoint(innerRectangle, movingCircle.GetCentroid());

            // Vector from closest point to circle center
            Vector2D toCenter = new(closestPoint, movingCircle.GetCentroid());

            // Current distance
            double currentDistance = toCenter.Magnitude();

            // Required distance after translation
            double requiredDistance = movingCircle.Radius;

            // Minimal translation needed along the direction
            double minimalTranslation = requiredDistance - (currentDistance * D.Dot(toCenter.Normalize()));

            if (minimalTranslation < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                // The circle already fits within the inner rectangle in this direction
                return 0;
            }
            
            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationPolygonIntoPolygon(PolygonInfo innerPolygon, PolygonInfo movingPolygon, Vector2D direction)
        {
            // Implement SAT to find minimal translation along the direction
            // This involves finding the axis with the smallest overlap and calculating the required translation

            // Placeholder for actual implementation
            double minimalTranslation = double.PositiveInfinity;

            // Get all normals from both polygons
            var axes = innerPolygon.EdgesNormalAxesNormalized.Concat(movingPolygon.EdgesNormalAxesNormalized);

            foreach (var axis in axes)
            {
                // Project both polygons onto the axis
                var projection1 = innerPolygon.GetProjectionOntoAxis(axis);
                var projection2 = movingPolygon.GetProjectionOntoAxis(axis);

                // Calculate overlap
                double overlap = projection1.GetOverlap(projection2);

                if (overlap < DoubleSafeEqualityComparer.DefaultEpsilon)
                {
                    // No collision on this axis
                    return double.PositiveInfinity;
                }
                else
                {
                    // Project the direction onto the axis
                    double directionProjection = direction.Dot(axis.Normalize());

                    if (Math.Abs(directionProjection) > DoubleSafeEqualityComparer.DefaultEpsilon)
                    {
                        continue; // No contribution along this axis
                    }

                    // Calculate translation needed along this axis
                    double translation = overlap / Math.Abs(directionProjection);

                    if (translation < minimalTranslation)
                    {
                        minimalTranslation = translation;
                    }
                }
            }

            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationPolygonIntoCircle(CircleInfo innerCircle, PolygonInfo movingPolygon, Vector2D direction)
        {
            // Ensure the direction is normalized
            Vector2D D = direction.Normalize();

            // Find the vertex of the polygon that is farthest in the opposite direction of D
            // This vertex will determine the minimal translation needed
            PointXY farthestVertex = movingPolygon.Vertices
                .OrderByDescending(vertex =>
                {
                    // Vector from polygon center to vertex
                    Vector2D vectorToVertex = new(movingPolygon.GetCentroid(), vertex);
                    // Project this vector onto the opposite direction
                    return vectorToVertex.Dot(-D);
                })
                .First();

            // Vector from circle center to farthest vertex
            Vector2D toVertex = new(innerCircle.GetCentroid(), farthestVertex);

            // Current distance from the vertex to the circle center
            double currentDistance = toVertex.Magnitude();

            // Required distance for the vertex to be within the circle
            // Assuming the entire polygon needs to fit, consider the polygon's maximum vertex distance from its centroid
            double requiredDistance = innerCircle.Radius - movingPolygon.GetMaxVertexDistance();

            // Calculate the minimal translation along the direction
            double projection = toVertex.Dot(D);

            double minimalTranslation = requiredDistance - projection;

            // If the minimal translation is negative or zero, the polygon already fits
            if (minimalTranslation < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return 0;
            }

            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationCircleIntoCircle(CircleInfo innerCircle, CircleInfo movingCircle, Vector2D direction)
        {
            if (innerCircle.Radius <= movingCircle.Radius)
            {
                //moving cannot fit in fixed !
                return double.PositiveInfinity;
            }

            // Assume direction is normalized
            Vector2D D = direction.Normalize();

            // Vector from moving circle center to inner circle center
            Vector2D toCenter = new(movingCircle.GetCentroid(), innerCircle.GetCentroid());

            // Project this vector onto the direction
            double projection = toCenter.Dot(D);

            // Current distance between centers
            //double currentDistance = toCenter.Magnitude();

            // Required distance after translation
            double requiredDistance = innerCircle.Radius - movingCircle.Radius;

            // Minimal translation needed along the direction
            double minimalTranslation = requiredDistance - projection;

            if (minimalTranslation < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                // The circle already fits within the inner circle in this direction
                return 0;
            }

            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationRectangleIntoCircle(CircleInfo innerCircle, RectangleInfo movingRectangle, Vector2D direction)
        {
            // Ensure the direction is normalized
            Vector2D D = direction.Normalize();

            // Get the corners of the moving rectangle
            var corners = movingRectangle.GetVectices(); // Returns List<PointXY>

            // Find the corner farthest in the opposite direction of D
            PointXY farthestCorner = corners.OrderByDescending(corner =>
            {
                // Vector from rectangle center to corner
                Vector2D vectorToCorner = new(movingRectangle.GetCentroid(), corner);
                // Project this vector onto the opposite direction (-D)
                return vectorToCorner.Dot(-D);
            }).First();

            // Vector from circle center to farthest corner
            Vector2D toCenter = new(innerCircle.GetCentroid(), farthestCorner);

            // Current distance from the corner to the circle center
            double currentDistance = toCenter.Magnitude();

            // Compute the required distance after translation
            // Ensure that after translation, the farthest corner is within the circle
            // Therefore, the distance from circle center to farthest corner should be <= innerCircle.Radius
            // Thus, we need to move the rectangle along D by (currentDistance - innerCircle.Radius)
            // However, to minimize translation, we consider the projection of toCenter onto D
            // and adjust accordingly.

            // Projection of toCenter onto D
            double projection = toCenter.Dot(D);

            // Calculate the minimal translation needed along the direction
            double minimalTranslation = (currentDistance - innerCircle.Radius) / (1 - D.Dot(toCenter.Normalize()));

            // Handle potential division by zero or invalid translations
            if (double.IsNaN(minimalTranslation) || double.IsInfinity(minimalTranslation))
            {
                // Fallback: Calculate the minimal translation directly
                minimalTranslation = currentDistance - innerCircle.Radius;
            }

            // If the minimal translation is negative or zero, the rectangle already fits
            if (minimalTranslation < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return 0;
            }

            return minimalTranslation;
        }
        private static double ComputeMinimalTranslationCircleIntoPolygon(PolygonInfo innerPolygon, CircleInfo movingCircle, Vector2D direction)
        {
            // Ensure the direction is normalized
            Vector2D D = direction.Normalize();

            // Find the closest point on the polygon to the circle's center
            PointXY closestPoint = innerPolygon.GetClosestPoint(movingCircle.GetCentroid());

            // Vector from the closest point to the circle's center
            Vector2D toCircleCenter = new(closestPoint, movingCircle.GetCentroid());

            // Current distance from the closest point to the circle's center
            double currentDistance = toCircleCenter.Magnitude();

            // Determine the required distance for the circle to fit within the polygon
            // The circle fits if the distance from its center to the polygon's boundary is >= circle's radius
            double requiredDistance = movingCircle.Radius;

            // Calculate the difference between the required distance and the current distance
            double distanceDifference = requiredDistance - currentDistance;

            // If distanceDifference <= 0, the circle already fits within the polygon in this direction
            if (distanceDifference < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return 0;
            }

            // To resolve the collision, translate the circle along direction D by distanceDifference
            // However, ensure that translating by distanceDifference along D actually moves the circle towards fitting within the polygon

            // Project the vector from closest point to circle center onto direction D
            double projection = toCircleCenter.Dot(D);

            // Calculate the minimal translation required along direction D
            double minimalTranslation = distanceDifference - projection;

            // If minimalTranslation <= 0, it means that moving along D by distanceDifference - projection resolves the collision
            if (minimalTranslation < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return distanceDifference;
            }

            return minimalTranslation;
        }


    }

    public class NotSupportedShapeInCalculationException(ShapeInfo shape, string calculationName) : Exception($"Shape {shape.GetType().Name} is not supported for Calculations in:{calculationName}")
    {
    }
}
