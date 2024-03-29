using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static Vector3 GetCenter(Vector3[] arr) 
    {
        float x = 0;
        float y = 0;
        float z = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            x += arr[i].x;
            y += arr[i].y;
            z += arr[i].z;
        }

        x /= arr.Length;
        y /= arr.Length;
        z /= arr.Length;

        return new Vector3(x, y, z);

    }

    public static float GetScaledDelay(float initial, float timePassed)
    {
        return initial / Mathf.Pow(timePassed, timePassed * GlobalConstants.DELAY_SCALING);
    }

    public static int GetScaledSpawn(int initial, float timePassed)
    {
        return Mathf.FloorToInt(initial * Mathf.Pow(timePassed, timePassed * GlobalConstants.SPAWN_SCALING));
    }
}
