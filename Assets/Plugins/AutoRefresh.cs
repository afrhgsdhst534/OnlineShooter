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
    private static float refreshInterval = 2.5f; // Интервал обновления (сек)
    private static void AutoRefreshLoop()
    {
        // Проверяем, идет ли компиляция
        if (EditorApplication.isCompiling)
        {
            return;
        }

        if (EditorApplication.timeSinceStartup > nextRefreshTime)
        {
            AssetDatabase.Refresh(); // Перезагружаем файлы
            nextRefreshTime = (float)EditorApplication.timeSinceStartup + refreshInterval;
        }
    }
}
#endif