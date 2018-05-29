using System.Numerics;

namespace OpenWheels.MonoGame
{
    public static class OpenWheelsConversions
    {
        public static Vector2 ToNum(this Microsoft.Xna.Framework.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToNum(this Microsoft.Xna.Framework.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Rectangle ToOw(this Microsoft.Xna.Framework.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Microsoft.Xna.Framework.Rectangle ToMg(this Rectangle rect)
        {
            return new Microsoft.Xna.Framework.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Color ToOw(this Microsoft.Xna.Framework.Color col)
        {
            return new Color(col.PackedValue);
        }
    }
}
