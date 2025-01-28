using System;
using TMPro;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;


[CustomEditor(typeof(CustomButton), true)]
public class CustomButtonEditor : ButtonEditor
{
    private SerializedProperty childImageColors;
    private SerializedProperty childTextColors;
    private SerializedProperty childTextMeshProColors;


    protected override void OnEnable()
    {
        base.OnEnable();
        childImageColors = serializedObject.FindProperty("childImage_colors");
        childTextColors = serializedObject.FindProperty("childText_colors");
        childTextMeshProColors = serializedObject.FindProperty("childTextMeshPro_colors");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (CustomEditorTools.DrawHeader("[Child Input Change]", "Child Input Change", false, true))
        {
            DrawChildImageState();
            DrawChildTextState();
            DrawChildTextMeshProState();
        }
    }

    void DrawChildImageState()
    {
        EditorGUI.BeginChangeCheck();
        
        CustomButton btn = (CustomButton)target;

        CustomEditorTools.BeginContents(true);
        {
            if (CustomEditorTools.DrawHeader("[Image State]", "Image State", false, true))
            {
                CustomEditorTools.BeginContents(true);
                {
                    GUILayout.BeginHorizontal();
                    btn.childImage = EditorGUILayout.ObjectField("[Target]", btn.childImage, typeof(Image), true) as Image;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3f);

                    if (CustomEditorTools.DrawHeader("[Event State]", "Event State_1", false, true))
                    {
                        CustomEditorTools.BeginContents(true);
                        {
                            EditorGUI.BeginDisabledGroup(btn.childImage == null);
                            {
                                if (btn.childImage != null)
                                {
                                    CustomEditorTools.BeginContents(true);
                                    {
                                        EditorGUILayout.PropertyField(childImageColors);
                                    }
                                    CustomEditorTools.EndContents();
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        CustomEditorTools.EndContents();
                    }
                }
                CustomEditorTools.EndContents();
            }
        }
        CustomEditorTools.EndContents();
        
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }

    void DrawChildTextState()
    {
        EditorGUI.BeginChangeCheck();
        
        CustomButton btn = (CustomButton)target;
        
        CustomEditorTools.BeginContents(true);
        {
            if (CustomEditorTools.DrawHeader("[Text State]", "Text State", false, true))
            {
                CustomEditorTools.BeginContents(true);
                {
                    GUILayout.BeginHorizontal();
                    btn.childText = EditorGUILayout.ObjectField("[Target]", btn.childText, typeof(Text), true) as Text;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3f);

                    if (CustomEditorTools.DrawHeader("[Event State]", "Event State_2", false, true))
                    {
                        CustomEditorTools.BeginContents(true);
                        {
                            EditorGUI.BeginDisabledGroup(btn.childText == null);
                            {
                                if (btn.childText != null)
                                {
                                    CustomEditorTools.BeginContents(true);
                                    {
                                        EditorGUILayout.PropertyField(childTextColors);
                                    }
                                    CustomEditorTools.EndContents();
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        CustomEditorTools.EndContents();
                    }
                }
                CustomEditorTools.EndContents();
            }
        }
        CustomEditorTools.EndContents();
        
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }

    void DrawChildTextMeshProState()
    {
        EditorGUI.BeginChangeCheck();

        CustomButton btn = (CustomButton)target;

        CustomEditorTools.BeginContents(true);
        {
            if (CustomEditorTools.DrawHeader("[TextMeshPro State]", "TextMeshPro State", false, true))
            {
                CustomEditorTools.BeginContents(true);
                {
                    GUILayout.BeginHorizontal();
                    btn.childTextMeshPro = EditorGUILayout.ObjectField("[Target]", btn.childTextMeshPro, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3f);

                    if (CustomEditorTools.DrawHeader("[Event State]", "Event State_2", false, true))
                    {
                        CustomEditorTools.BeginContents(true);
                        {
                            EditorGUI.BeginDisabledGroup(btn.childTextMeshPro == null);
                            {
                                if (btn.childTextMeshPro != null)
                                {
                                    CustomEditorTools.BeginContents(true);
                                    {
                                        EditorGUILayout.PropertyField(childTextMeshProColors);
                                    }
                                    CustomEditorTools.EndContents();
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        CustomEditorTools.EndContents();
                    }
                }
                CustomEditorTools.EndContents();
            }
        }
        CustomEditorTools.EndContents();

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
