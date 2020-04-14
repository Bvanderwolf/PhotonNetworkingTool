#if (UNITY_EDITOR)

using ConnectCards;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConnectCardHandler))]
public class ConnectCardHandlerEditor : Editor
{
    private SerializedProperty m_BuildIndexOfGameScene;
    private SerializedProperty m_BackgroundSprite;

    private void OnEnable()
    {
        m_BuildIndexOfGameScene = this.serializedObject.FindProperty("BuildIndexOfGameScene");
        m_BackgroundSprite = this.serializedObject.FindProperty("BackGroundSprite");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        this.serializedObject.Update();
        var handler = (ConnectCardHandler)target;
        if (handler.LoadSceneOnCountdownEnd)
        {
            handler.BuildIndexOfGameScene = EditorGUILayout.IntField("BuildIndexOfGameScene", handler.BuildIndexOfGameScene);
            m_BuildIndexOfGameScene.intValue = handler.BuildIndexOfGameScene;
        }

        if (handler.UseBackGroundImage)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Source Image");
            var newBackground = (Sprite)EditorGUILayout.ObjectField(handler.BackGroundSprite, typeof(Sprite), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();
            m_BackgroundSprite.objectReferenceValue = newBackground;
        }

        this.serializedObject.ApplyModifiedProperties();
    }
}

#endif