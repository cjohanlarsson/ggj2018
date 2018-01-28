public class TrashGate : GridObject {
    public int useCount;

    int remainingUses;

    public override void Init( Grid grid ) {
        base.Init( grid );
        OnStop();
    }

    public override void OnStop () {
        remainingUses = useCount;
    }

    public override void OnNoteEnter( Note note ) {
        if( useCount > 0 ) {
            useCount--;
        } else {
            grid.DestroyNote( note );
        }
    }
}