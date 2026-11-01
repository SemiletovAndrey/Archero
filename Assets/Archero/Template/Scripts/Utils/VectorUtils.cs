using UnityEngine;

namespace Template.Scripts.Utils
{
    public static class VectorUtils
    {
        public static bool Equals(Vector3 v1, Vector3 v2)
        {
            return (v1 - v2).sqrMagnitude < Mathf.Epsilon;
        }
        
        public static bool Equals(Quaternion q1, Quaternion q2)
        {
            return Mathf.Abs(Quaternion.Dot(q1, q2)) > 0.999999f;
        }
        
        public static Vector3 FromValue(float value)
        {
            return new Vector3(value, value, value);
        }

        public static Vector3 WithY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }
        
        public static Vector3 WithYOffset(this Vector3 vector, float value)
        {
            vector.y += value;
            return vector;
        }
    }
}