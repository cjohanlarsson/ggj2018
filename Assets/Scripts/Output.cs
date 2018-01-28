using UnityEngine;

[SelectionBase]
public class Output : GridObject {
    public override void OnNoteEnter( Note note ) {
        Debug.Log( "Played note at pitch " + note.pitch + " and duration " + note.duration );
        // grid.DestroyNote( note );
        MusicPlayer.Singleton.PlayNote(note.pitch, note.duration);
        grid.MarkGoal(note);
    }
}