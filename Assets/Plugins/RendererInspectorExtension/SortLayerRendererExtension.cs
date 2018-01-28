//  SortLayerRendererExtension.cs
//   Author:
//       Yves J. Albuquerque <yves.albuquerque-at-gmail.com>
//  Last Update:
//       27-12-2013
//Put this file into a folder named Editor.
//Based on Nick`s code at https://gist.github.com/nickgravelyn/7460288 and Ivan Murashko solution at http://forum.unity3d.com/threads/210683-List-of-sorting-layers?p=1432958&viewfull=1#post1432958 aput by Guavaman at http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

[CanEditMultipleObjects()]
[CustomEditor( typeof( MeshRenderer ), true )]
public class SortLayerRendererExtension : Editor {
	Renderer renderer;
	string[] sortingLayerNames;

	int selectedOption;

	void OnEnable () {
		sortingLayerNames = GetSortingLayerNames();
		renderer = ( target as Renderer );

		for( int i = 0; i < sortingLayerNames.Length; i++ ) {
			if( sortingLayerNames[ i ] == renderer.sortingLayerName ) {
				selectedOption = i;
			}
		}
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector();
		if( !renderer ) {
			return;
		}

		EditorGUILayout.Space();

		selectedOption = EditorGUILayout.Popup( "Sorting Layer", selectedOption, sortingLayerNames );
		if( sortingLayerNames[ selectedOption ] != renderer.sortingLayerName ) {
			Undo.RecordObject( renderer, "Sorting Layer" );
			renderer.sortingLayerName = sortingLayerNames[ selectedOption ];
			EditorUtility.SetDirty( renderer );
		}

		int newSortingLayerOrder = EditorGUILayout.IntField( "Order in Layer", renderer.sortingOrder );
		if( newSortingLayerOrder != renderer.sortingOrder ) {
			Undo.RecordObject( renderer, "Edit Sorting Order" );
			renderer.sortingOrder = newSortingLayerOrder;
			EditorUtility.SetDirty( renderer );
		}
	}

	// Get the sorting layer names
	public string[] GetSortingLayerNames () {
		Type internalEditorUtilityType = typeof( InternalEditorUtility );
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty( "sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic );
		return (string[])sortingLayersProperty.GetValue( null, new object[0] );
	}

	// Get the unique sorting layer IDs -- tossed this in for good measure
	public int[] GetSortingLayerUniqueIDs () {
		Type internalEditorUtilityType = typeof( InternalEditorUtility );
		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty( "sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic );
		return (int[])sortingLayerUniqueIDsProperty.GetValue( null, new object[0] );
	}
}
