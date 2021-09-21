using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;

[CustomEditor(typeof(Spell))]
[CanEditMultipleObjects]
public class CustomSpellEditor : Editor
{
    Spell t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    int _choiceIndex = 0;
    List<string> _choices = new List<string> { "None", "Wet", "Steam" };

    void OnEnable()
    {
        t = (Spell)target;
        GetTarget = new SerializedObject(t);
        _choiceIndex = _choices.FindIndex(c => c == t.includesOnly.type);
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        GetTarget.Update();
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Status Includes only");
        _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices.ToArray());
        EditorGUILayout.EndHorizontal();
        if (_choiceIndex < 0)
            _choiceIndex = 0;

        t.includesOnly = StatusList.getStatuses().Find(s => _choices[_choiceIndex] == s.type);

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
