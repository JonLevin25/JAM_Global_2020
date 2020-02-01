using UnityEngine;

public static class Extensions
{
    public static int GetFloor<T>(this T t) where T : Component => Mathf.FloorToInt(FloorHelper.Instance.GetFloor(t));
}