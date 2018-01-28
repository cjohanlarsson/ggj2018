using UnityEngine;

[SelectionBase]
public class RedirectGate : GridObject {
	public MoveDirection redirectDirection;

	public override void Init ( Grid grid ) {
		base.Init( grid );
		UpdateRotationGraphics();
	}

	public override void OnNoteEnter ( Note note ) {
		if( note.direction != redirectDirection ) Pulse( 1, note );
		note.direction = redirectDirection;

	}

	public override void Rotate ( bool clockwise = true ) {
		redirectDirection = Grid.RotateDirection( redirectDirection, clockwise );
		UpdateRotationGraphics();
	}

    void OnValidate () {
        if( !Application.isPlaying ) UpdateRotationGraphics();
    }

    private void UpdateRotationGraphics() {
        Vector3 d1 = Grid.GetDirectionVector(redirectDirection).ToVector3();

        var angle = Vector3.Angle(Vector3.up, d1);

        if (redirectDirection == MoveDirection.Left)
        {
            angle = -angle;
        }

		graphics.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
    }
}
