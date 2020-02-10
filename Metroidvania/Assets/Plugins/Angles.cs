using UnityEngine;

namespace MyMath
{
    public static class Angles
    {
        /// <summary>
		/// Vector2.right is 0 degrees and goes counterclockwise.
		/// </summary>
        public static Vector2 AngleToVector2( float angle )
        {
            float a = -(angle - 90f) * Mathf.Deg2Rad;
            return new Vector2( Mathf.Sin( a ), Mathf.Cos( a ) ).normalized;
        }
    }
}