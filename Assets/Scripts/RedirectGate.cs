using UnityEngine;

[SelectionBase]
public class RedirectGate : GridObject {
	public MoveDirection redirectDirection;

	public override void OnNoteEnter ( Note note ) {
		note.direction = redirectDirection;
	}

	public override void Rotate ( bool clockwise = true ) {
		redirectDirection = Grid.RotateDirection( redirectDirection, clockwise );
	}
}
