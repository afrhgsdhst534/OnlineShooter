#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class AutoRefresh
{
    static AutoRefresh()
    {
        EditorApplication.update += AutoRefreshLoop;
    }
    private static float nextRefreshTime = 0f;
    private static float refreshInterval = 2.5f; // �������� ���������� (���)
    private static void AutoRefreshLoop()
    {
        // ���������, ���� �� ����������
        if (EditorApplication.isCompiling)
        {
            return;
        }

        if (EditorApplication.timeSinceStartup > nextRefreshTime)
        {
            AssetDatabase.Refresh(); // ������������� �����
            nextRefreshTime = (float)EditorApplication.timeSinceStartup + refreshInterval;
        }
    }
}
#endif