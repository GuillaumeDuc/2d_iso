using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;

[CustomEditor(typeof(SpellEffectScript))]
[CanEditMultipleObjects]
public class CustomListEditor : Editor
{
    SpellEffectScript t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    int _choiceIndex = 0;
    string[] _choices = SpellEffectList.spellEffects.Select(a => a.name).ToArray();

    void OnEnable()
    {
        t = (SpellEffectScript)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("selectedSpellEffects"); // Find the List in our script and create a reference of it
    }

    public override void OnInspectorGUI()
    {
        //Update our list

        GetTarget.Update();

        DrawDefaultInspector();

        //Resize our list
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select the spell effect");
        ListSize = ThisList.arraySize;

        if (ListSize != ThisList.arraySize)
        {
            while (ListSize > ThisList.arraySize)
            {
                ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
            }
            while (ListSize < ThisList.arraySize)
            {
                ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
            }
        }

        _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
        if (_choiceIndex < 0)
            _choiceIndex = 0;

        EditorGUILayout.Space();

        //Add a new item to the List<> with a button
        EditorGUILayout.LabelField("Add a new spell effect");

        if (GUILayout.Button("Add New"))
        {
            if (!t.selectedSpellEffects.Contains(SpellEffectList.spellEffects[_choiceIndex]))
            {
                t.selectedSpellEffects.Add(SpellEffectList.spellEffects[_choiceIndex]);
            }
        }

        EditorGUILayout.Space();

        //Display our list to the inspector window

        for (int i = 0; i < ThisList.arraySize; i++)
        {
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty MyString = MyListRef.FindPropertyRelative("name");

            EditorGUILayout.LabelField("Spell effect");
            EditorGUILayout.LabelField("Name", MyString.stringValue);
            // MyString.stringValue = EditorGUILayout.TextField("Name", MyString.stringValue);
            EditorGUILayout.Space();
            if (GUILayout.Button("Remove Effect"))
            {
                ThisList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.Space();
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
