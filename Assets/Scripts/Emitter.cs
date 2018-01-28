using UnityEngine;

[SelectionBase]
public class Emitter : GridObject {

	[SerializeField] private int emitPerBeats;
	public float duration = 0.5f;

	public MoveDirection emitDirection;

	private int beatsLeft;

	public int maxEmitted;

	public int remainingEmitted;

	[SerializeField]
	GameObject emitDirectionImg;

	public override void Init( Grid grid ) {
		base.Init( grid );
		OnStop();
	}

	public override void OnStop () {
		base.OnStop();
		beatsLeft = 0;
		remainingEmitted = maxEmitted;
	}

    public override void OnNoteEnter( Note note ) {
		grid.DestroyNote( note );
		var particles = Grid.MakeNoteCollideParticles( note );
		particles.transform.position = transform.position + Grid.GetParticleOffset( note.direction, 0.15f );
		particles.transform.rotation = Grid.GetDirectionRotation( Grid.GetOppositeDirection( note.direction ) );
		//Waver( note.duration * (float)grid.frequency - 0.04f );
	}

	public override void Rotate ( bool clockwise = true ) {
		emitDirection = Grid.RotateDirection( emitDirection, clockwise );
		UpdateRotationGraphics();
	}

    private void UpdateRotationGraphics() {
        Vector3 d1 = Grid.GetDirectionVector(emitDirection).ToVector3();
        emitDirectionImg.transform.localPosition = d1 * 0.2800119f;

        var angle = Vector3.Angle(Vector3.up, d1);

        if (emitDirection == MoveDirection.Left)
        {
            angle = -angle;
        }

        emitDirectionImg.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

	public bool CheckReady() {
		if( remainingEmitted <= 0 ) return false;
		beatsLeft--;
		if(beatsLeft <= 0)
		{
			beatsLeft = emitPerBeats;
			remainingEmitted -= 1;
			return true;
		}
		else
		{
			return false;
		}
	}

    void OnValidate () {
        if( !Application.isPlaying ) UpdateRotationGraphics();
    }
}
