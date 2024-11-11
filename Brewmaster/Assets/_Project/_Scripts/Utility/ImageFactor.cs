using System.Security.Cryptography;
using UnityEngine;

class ImageFactor
{
    public static Vector2 _centerPoint = new Vector2(0.5f, 0.5f);

    public static Sprite CreateSprite(Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), _centerPoint, 100, 1, SpriteMeshType.Tight, Vector4.zero, false);
    }

    public static Sprite CreateSprite(Texture2D texture2D, Vector2 centerPoint)
    {
        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), centerPoint, 100, 1, SpriteMeshType.Tight, Vector4.zero, false);
    }
}
