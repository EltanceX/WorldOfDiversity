using System;
using System.Linq;
using System.Numerics;

namespace GlassMod;

public enum FaceDirection
{
    Forward,
    Backward,
    Right,
    Left,
    Up,
    Down,
    ForwardRight,
    ForwardLeft,
    ForwardDown
}
public enum FaceEnum
{
    pZ,
    pX,
    nZ,
    nX,
    pY,
    nY
}
public static class PosTransform
{
    public static Matrix4x4 GetTransform(Vector3 point, Vector3 center = new Vector3(), float degreesY = 0, float degreesX = 0, float degreesZ = 0)
    {
        float radiansX = MathF.PI / 180f * degreesX;
        float radiansY = MathF.PI / 180f * degreesY;
        float radiansZ = MathF.PI / 180f * degreesZ;

        Matrix4x4 translateToOrigin = Matrix4x4.CreateTranslation(-center);
        Matrix4x4 rotationX = Matrix4x4.CreateRotationX(radiansX);
        Matrix4x4 rotationY = Matrix4x4.CreateRotationY(radiansY);
        Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(radiansZ);
        Matrix4x4 translateBack = Matrix4x4.CreateTranslation(center);

        // 注意乘法顺序：先平移到原点 -> 旋转 -> 再平移回来
        Matrix4x4 transform = translateToOrigin * rotationX * rotationY * rotationZ * translateBack;
        return transform;

    }

    public static Vector3 Rotate(Vector3 point, Vector3 center = new Vector3(), float degreesY = 0, float degreesX = 0, float degreesZ = 0)
    {

        Matrix4x4 transform = GetTransform(point, center, degreesY, degreesX, degreesZ);
        Vector3 v = Vector3.Transform(point, transform);
        return v;
    }
    public static Vector3 RotateHalfPI(Vector3 point, Vector3 center = new Vector3(), float degreesY = 0, float degreesX = 0, float degreesZ = 0)
    {
        Vector3 v = Rotate(point, center, degreesY, degreesX, degreesZ);
        v.X = (int)v.X;
        v.Y = (int)v.Y;
        v.Z = (int)v.Z;
        return v;
    }
    public static int[] FaceToVectorI(FaceDirection direction, int face)
    {
        Vector3 v = FaceToVector(direction, face);
        return new int[3] { (int)v.X, (int)v.Y, (int)v.Z };
    }
    public static Vector3 FaceToVector(FaceDirection direction, int face)
    {
        Vector3 v = new Vector3();
        switch (direction)
        {//z+
            case FaceDirection.Forward:
                v = new Vector3(0, 0, 1);
                break;
            case FaceDirection.Backward:
                v = new Vector3(0, 0, -1);
                break;
            case FaceDirection.Right:
                v = new Vector3(1, 0, 0);
                break;
            case FaceDirection.Left:
                v = new Vector3(-1, 0, 0);
                break;
            case FaceDirection.Up:
                v = new Vector3(0, 1, 0);
                break;
            case FaceDirection.Down:
                v = new Vector3(0, -1, 0);
                break;
            case FaceDirection.ForwardRight:
                v = new Vector3(1, 0, 1);
                break;
            case FaceDirection.ForwardLeft:
                v = new Vector3(-1, 0, 1);
                break;
            case FaceDirection.ForwardDown:
                v = new Vector3(0, -1, 1);
                break;
        }

        var faceEnum = (FaceEnum)face;
        switch (faceEnum)
        {
            case FaceEnum.pZ:
                //v = Rotate(v, degreesY: 0);
                break;
            case FaceEnum.nZ:
                v = RotateHalfPI(v, degreesY: 180);
                break;
            case FaceEnum.pX:
                v = RotateHalfPI(v, degreesY: 90);
                break;
            case FaceEnum.nX:
                v = RotateHalfPI(v, degreesY: 270);
                break;
            case FaceEnum.pY:
                v = RotateHalfPI(v, degreesX: -90);
                break;
            case FaceEnum.nY:
                v = RotateHalfPI(v, degreesX: -270);
                break;
        }

        return v;
    }


    //public static void Main()
    //{
    //    var v = FaceToVector(FaceDirection.ForwardRight, (int)FaceEnum.nX);

    //    Console.WriteLine($"{v.X}, {v.Y}, {v.Z}");
    //}
}
