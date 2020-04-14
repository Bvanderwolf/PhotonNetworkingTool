using ConnectCards;
using UnityEditor;

[CustomEditor(typeof(ConnectCardHandler))]
public class ConnectCardHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var handler = (ConnectCardHandler)target;

        if (handler.LoadSceneOnCountdownEnd)
        {
            handler.SetBuildIndexOfGameScene(this, EditorGUILayout.IntField("BuildIndexOfGameScene", handler.BuildIndexOfGameScene));
        }
    }
}