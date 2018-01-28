using UnityEngine;

[SelectionBase]
public class RedirectGate : GridObject {
	public MoveDirection redirectDirection;

	[SerializeField]
	GameObject redirectDirectionImg;

	public override void Init ( Grid grid ) {
		base.Init( grid );
		UpdateRotationGraphics();
	}

	public override void OnNoteEnter ( Note note ) {
		note.direction = redirectDirection;

		Pulse();
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
        redirectDirectionImg.transform.localPosition = d1 * 0.2800119f;

        var angle = Vector3.Angle(Vector3.up, d1);

        if (redirectDirection == MoveDirection.Left)
        {
            angle = -angle;
        }

        redirectDirectionImg.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
