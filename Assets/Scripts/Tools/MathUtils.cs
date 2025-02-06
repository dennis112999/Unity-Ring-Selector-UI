using UnityEngine;

namespace Dennis.Tools
{
    public static class MathUtils
    {
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static float RadiansToDegrees(float radians)
        {
            return radians * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Calculates the 2D position on a circle given an angle in degrees
        /// </summary>
        public static Vector2 GetPositionFromDegrees(float degrees, float radius = 1.0f)
        {
            return GetPositionFromRadians(DegreesToRadians(degrees), radius);
        }

        /// <summary>
        /// Calculates the 2D position on a circle given an angle in radians
        /// </summary>
        public static Vector2 GetPositionFromRadians(float radians, float radius = 1.0f)
        {
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
        }
    }
}