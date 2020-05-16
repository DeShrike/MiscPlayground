using UnityEngine;

public class PlayerController : MonoBehaviour 
{
	public float MoveSpeed = 5f;

    private bool IsMoving;
    private float LastX;
    private float LastY;

    private Rigidbody2D Rigid;
    private Animator Anim;

	// Use this for initialization
	private void Start () 
	{
        this.Anim = this.GetComponent<Animator>();
        this.Rigid = this.GetComponent<Rigidbody2D>();
        Debug.Log("Start");
    }

    // Update is called once per frame
    private void Update () 
	{
        this.IsMoving = false;
		float hor = Input.GetAxisRaw ("Horizontal");
		float ver = Input.GetAxisRaw ("Vertical");

        float dx = 0f;
        float dy = 0f;

		if (hor > 0.5f || hor < -0.5f) 
		{
            dx = hor * this.MoveSpeed ;
            this.IsMoving = true;
            this.LastX = hor;
            this.LastY = 0f;
        }

        if (ver > 0.5f || ver < -0.5f)
        {
            dy = ver * this.MoveSpeed ;
            this.IsMoving = true;
            this.LastX = 0f;
            this.LastY = ver;
        }

        if (dx != 0f || dy != 0f)
        {
            this.Rigid.velocity = new Vector2(dx,dy);
            //transform.Translate(new Vector3(dx * Time.deltaTime, dy * Time.deltaTime, 0f));
        }
        else
        {
            this.Rigid.velocity = Vector2.zero;
        }

        this.Anim.SetFloat("MoveX", hor);
        this.Anim.SetFloat("MoveY", ver);
        this.Anim.SetBool("IsMoving", this.IsMoving);
        this.Anim.SetFloat("LastMoveX", this.LastX);
        this.Anim.SetFloat("LastMoveY", this.LastY);
    }
}
