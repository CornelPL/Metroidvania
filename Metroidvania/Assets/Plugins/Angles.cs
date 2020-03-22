using UnityEngine;

namespace MyMath
{
    public static class Angles
    {
        /// <returns>
		/// Normalized vector where Vector2.right is 0 degrees and goes counterclockwise.
		/// </returns>
        public static Vector2 AngleToVector2( float angle )
        {
            float a = -(angle - 90f) * Mathf.Deg2Rad;
            return new Vector2( Mathf.Sin( a ), Mathf.Cos( a ) ).normalized;
        }


        /// <returns>
		/// Angle in degrees (0,360) where Vector2.right is 0 and goes counterclockwise
		/// </returns>
        public static float Vector2ToAngle( Vector2 vector )
        {
            float angle = Mathf.Atan2( -vector.y, vector.x ) * Mathf.Rad2Deg;

            if ( angle < 0f )
            {
                angle *= -1f;
            }
            else if ( angle > 0f )
            {
                angle = -(angle - 360f);
            }

            return angle;
        }
    }
}