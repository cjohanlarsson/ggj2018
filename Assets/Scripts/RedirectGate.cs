public class RedirectGate : GridObject {
	public MoveDirection redirectDirection;

	public override void OnNoteEnter ( Note note ) {
		note.direction = redirectDirection;
	}
}
