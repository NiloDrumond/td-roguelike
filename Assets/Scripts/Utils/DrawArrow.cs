
using UnityEngine;
using System.Collections;

public static class DrawArrow
{
    public static void ForGizmo(Vector3 from, Vector3 to, float arrowHeadLength = 0.1f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawLine(from, to);
        Vector3 direction = to - from;
        DrawArrowEnd(true, from, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForGizmo(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.1f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawLine(from, to);
        Vector3 direction = to - from;
        DrawArrowEnd(true, from, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForDebug(Vector3 from, Vector3 to, float arrowHeadLength = 0.1f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawLine(from, to);
        Vector3 direction = to - from;
        DrawArrowEnd(false, from, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }

    public static void ForDebug(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.1f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawLine(from, to);
        Vector3 direction = to - from;
        DrawArrowEnd(false, from, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.1f, float arrowHeadAngle = 20.0f)
    {
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
        if (gizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
        }
        else
        {
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, up * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, down * arrowHeadLength, color);
        }
    }
}
