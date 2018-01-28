using UnityEngine;

[SelectionBase]
public class SplitGate : GridObject {
    public MoveDirection direction1;
    public MoveDirection direction2;

	[SerializeField]
    GameObject direction1Img;
	[SerializeField]
    GameObject direction2Img;

    public override void Init(Grid grid) {
        base.Init( grid );
		UpdateRotationGraphics();
    }

    public override void OnNoteEnter( Note note ) {
        var clone = grid.CloneNote( note );
        note.direction = direction1;
        clone.direction = direction2;
        clone.Init( grid, false );
		Pulse();
	}

	public override void Rotate ( bool clockwise = true ) {
        direction1 = Grid.RotateDirection( direction1, clockwise );
        direction2 = Grid.RotateDirection( direction2, clockwise );
        UpdateRotationGraphics();
    }

    void OnValidate () {
        if( !Application.isPlaying ) UpdateRotationGraphics();
    }

    private void UpdateRotationGraphics() {
        Vector3 d1 = Grid.GetDirectionVector(direction1).ToVector3();
        Vector3 d2 = Grid.GetDirectionVector(direction2).ToVector3();
        direction1Img.transform.localPosition = d1 * 0.2800119f;
        direction2Img.transform.localPosition = d2 * 0.2800119f;

        var angle = Vector3.Angle(Vector3.up, d1);

        if (direction1 == MoveDirection.Left)
        {
            angle = -angle;
        }

        direction1Img.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        angle = Vector3.Angle(Vector3.up, d2);

        if (direction2 == MoveDirection.Left)
        {
            angle = -angle;
        }

        direction2Img.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
