using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MarionetteXNA
{
    static class Functions
    {
        public static void getAxisAngle(Quaternion quaternion, ref Vector3 outAxis, ref float outAngle)
        {
            if (quaternion.W > 1)
            {
                quaternion.Normalize();
            }
            outAngle = 2 * (float)Math.Acos(quaternion.W);
            float s = (float)Math.Sqrt(1 - quaternion.W * quaternion.W);
            if (s < 0.001)
            {
                outAxis.X = quaternion.X;
                outAxis.Y = quaternion.Y;
                outAxis.Z = quaternion.Z;
            }
            else
            {
                outAxis.X = quaternion.X / s;
                outAxis.Y = quaternion.Y / s;
                outAxis.Z = quaternion.Z / s;
            }
        }

        public static Quaternion getQuaternionFromKuka(float A, float B, float C)
        {
            return Quaternion.CreateFromRotationMatrix((Matrix.CreateRotationZ(A) * Matrix.CreateRotationY(B)) * Matrix.CreateRotationX(C));
        }

        public static float[] getKukaAnglesFromQuaternion(Quaternion rotation)
        {
            Matrix rotationMat = Matrix.CreateFromQuaternion(rotation);
            float[] angles = new float[3];
            angles[2] = (float)Math.Atan2(rotationMat.M32, rotationMat.M33);
            angles[1] = (float)Math.Atan2(-rotationMat.M31, Math.Sqrt(rotationMat.M32 * rotationMat.M32 + rotationMat.M33 * rotationMat.M33));
            angles[0] = (float)Math.Atan2(rotationMat.M21, rotationMat.M11);
            return angles;
        }

        public static float[] getAnglesFromQuaternion(Quaternion rotation)
        {
            float sqw = rotation.W * rotation.W;
            float sqx = rotation.X * rotation.X;
            float sqy = rotation.Y * rotation.Y;
            float sqz = rotation.Z * rotation.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = rotation.X * rotation.W - rotation.Y * rotation.Z;
            float[] v = new float[3];

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v[1] = 2f * (float)Math.Atan2(rotation.Y, rotation.X);
                v[0] = (float)Math.PI / 2;
                v[3] = 0;
                return NormalizeAngles(v);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v[1] = -2f * (float)Math.Atan2(rotation.Y, rotation.X);
                v[0] = -(float)Math.PI / 2;
                v[2] = 0;
                return NormalizeAngles(v);
            }
            Quaternion q = new Quaternion(rotation.W, rotation.Z, rotation.X, rotation.Y);
            v[1] = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
            v[0] = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                             // Pitch
            v[2] = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));      // Roll
            return NormalizeAngles(v);
        }

        static float[] NormalizeAngles(float[] angles)
        {
            for (int i = 0; i < angles.Length; i++)
            {
                angles[i]=NormalizeAngle(angles[i]);
            }
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 2 * (float)Math.PI)
                angle -= 2 * (float)Math.PI;
            while (angle < 0)
                angle += 2 * (float)Math.PI;
            return angle;
        }
    }
}
