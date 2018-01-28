using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeGate : GridObject {
	public NoteColor[] colorsToChangeTo;

	public Renderer[] renderersToSwapForColorChange;

	public override void OnNoteEnter ( Note note ) {
		note.color = colorsToChangeTo[this.grid.BeatsTicked % colorsToChangeTo.Length];
	}

	void Update() {
		if(renderersToSwapForColorChange != null) {
			foreach(var r in renderersToSwapForColorChange) {
				var nc = colorsToChangeTo[this.grid.BeatsTicked % colorsToChangeTo.Length];
				r.sharedMaterial = Visuals.Singleton.ConvertNoteColorToMaterial( nc );
			}
		}
	}
}
