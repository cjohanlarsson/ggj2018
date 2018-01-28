using UnityEngine;

[SelectionBase]
public class WaitGate : GridObject {
    public int waitTime; // in beats
    
    public override void OnNoteEnter( Note note ) {
        note.beatWait = waitTime;
    }
}