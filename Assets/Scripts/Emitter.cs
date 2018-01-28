using UnityEngine;

[SelectionBase]
public class Emitter : GridObject {
    public override void OnNoteEnter( Note note ) {
		  grid.DestroyNote( note );
    }
}
