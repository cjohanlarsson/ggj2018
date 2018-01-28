using UnityEngine;

[SelectionBase]
public class SplitGate : GridObject {
    public MoveDirection direction1;
    public MoveDirection direction2;

    public override void OnNoteEnter( Note note ) {
        var clone = grid.CloneNote( note );
        note.direction = direction1;
        clone.direction = direction2;
        clone.Init( grid, false );
    }

    public override void Rotate ( bool clockwise = true ) {
        direction1 = Grid.RotateDirection( direction1, clockwise );
        direction2 = Grid.RotateDirection( direction2, clockwise );
    }
}
