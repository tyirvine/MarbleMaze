using UnityEditor;

[InitializeOnLoadAttribute]
public static class PlayRefreshEditor
{
    static PlayRefreshEditor()
    {
        if (EditorPrefs.GetBool("kAutoRefresh") == false)
        {
            EditorApplication.playModeStateChanged += PlayRefresh;
        }
    }

    private static void PlayRefresh(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            AssetDatabase.Refresh();
        }
    }
}