public class TrashGate : GridObject {
    public int useCount;

    public override void OnNoteEnter( Note note ) {
        if( useCount > 0 ) {
            useCount--;
        } else {
            grid.DestroyNote( note );
        }
    }
}