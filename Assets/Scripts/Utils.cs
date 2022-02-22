using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Utils
{
    public static Vector3 Floor(this Vector3 vector3) {
        return new Vector3(Mathf.Floor(vector3.x), Mathf.Floor(vector3.y), Mathf.Floor(vector3.z));
    }

    public static Vector3Int FloorToInt(this Vector3 vector3) {
        return new Vector3Int(Mathf.FloorToInt(vector3.x), Mathf.FloorToInt(vector3.y), Mathf.FloorToInt(vector3.z));
    }
}
