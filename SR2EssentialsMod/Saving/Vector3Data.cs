using System;

namespace SR2E.Saving
{
    [Serializable]
    public struct Vector3Data
    {
        public float X = 0;
        public float Y = 0;
        public float Z = 0;

        public Vector3Data(float x, float y, float z) { X = x; Y = y; Z = z; }

        public Vector3Data(Vector3 vector) { X = vector.x; Y = vector.y; Z = vector.z; }

        public static Vector3 ConvertBack(Vector3Data vector) => new Vector3(vector.X, vector.Y, vector.Z);
    }
}
