using UnityEngine;

[SelectionBase]
public class PitchRaiseGate : GridObject {
    public int raiseAmount;

    public override void OnNoteEnter( Note note ) {
        note.pitch += raiseAmount;
    }
}
