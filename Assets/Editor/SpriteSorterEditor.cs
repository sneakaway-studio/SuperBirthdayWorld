using UnityEngine;
using System.Collections;
using UnityEditor;

/**
 *  Button to make it easy to resort sprite renderers
 */

[CustomEditor(typeof(SpriteSorter))]
public class SpriteSorterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteSorter spriteSorter = (SpriteSorter)target;
        if (GUILayout.Button("Save & Sort Child Sprites"))
        {
            spriteSorter.SaveAndSort();
        }
    }
}