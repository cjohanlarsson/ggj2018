using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visuals : MonoBehaviour {

	public static Visuals Singleton;

	[System.Serializable]
	public class NoteColorToColor
	{
		public NoteColor noteColor;
		public Color color;
	}
	[SerializeField] NoteColorToColor[] noteColors;
	[SerializeField] Material noteColorMaterial;
	Dictionary<NoteColor, Material> noteColorMaterials = new Dictionary<NoteColor, Material>();

	void Awake()
	{
		Singleton = this;
	}

	void OnDestroy()
	{
		Singleton = null;
		foreach(var kvp in noteColorMaterials)
		{
			GameObject.Destroy(kvp.Value);
		}
	}

	public Color ConvertNoteColorToColor(NoteColor c)
	{
		foreach(var n in noteColors)
		{
			if(n.noteColor == c)
				return n.color;
		}

		return new Color(0,0,0);
	}

	public Material ConvertNoteColorToMaterial(NoteColor c)
	{
		if(!noteColorMaterials.ContainsKey(c)) {
			var m = new Material(noteColorMaterial);
			m.color = ConvertNoteColorToColor(c);
			noteColorMaterials.Add(c, m);
		}

		return noteColorMaterials[c];
	}
}
