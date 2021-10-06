using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	private Rigidbody2D rd2d;
	public float moveTime =0.1f;
	public bool isMoveing=false;

	public LayerMask blockingLayer;
	private BoxCollider2D boxCollider;

    public int attckDamege=1;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
	    rd2d=GetComponent<Rigidbody2D>();
	    boxCollider=GetComponent<BoxCollider2D>();
        animator= GetComponent<Animator>();


    }
	
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playerTure)
        {
            return;
        }

        int horizontal =(int)Input.GetAxisRaw("Horizontal");
		int vertical=(int)Input.GetAxisRaw("Vertical");
		
		if(horizontal !=0)
		{
			vertical=0;
			if(horizontal==1)
			{
				transform.localScale= new Vector3(1,1,1);
			}
			else if(horizontal==-1)
			{
				transform.localScale=new Vector3(-1,1,1);
			}
			else if(vertical !=0)
			{
				horizontal=0;
			}
		}
		if(horizontal !=0|| vertical !=0)
		{
		ATMove(horizontal,vertical);
		}
	}
	public void ATMove(int horizontal,int vertical)
	{
		RaycastHit2D hit;

		bool canMove =Move(horizontal,vertical,out hit);

		if(hit.transform==null)
		{
            GameManager.instance.playerTure = false;

        return;
		}
        Damage hitComponent = hit.transform.GetComponent<Damage>();

        if (!canMove && hitComponent != null)
        {
            OncantMove(hitComponent);
        }
        GameManager.instance.playerTure = false;


    }
    public bool Move(int horizontal,int vertical,out RaycastHit2D hit)
	{
		Vector2 start =transform.position;
	
		Vector2 end =start + new Vector2(horizontal,vertical);

		boxCollider.enabled=false;
		
		hit= Physics2D.Linecast(start,end,blockingLayer);

		boxCollider.enabled=true;

		if(!isMoveing&&hit.transform==null)
		{
		StartCoroutine(Movement(end));

		return true;
		}
		return false;
	}

	IEnumerator Movement(Vector3 end)
	{
		isMoveing = true;

		float remainingDistance =(transform.position- end).sqrMagnitude;

		while(remainingDistance > float.Epsilon)
		{
		transform.position= Vector3.MoveTowards(transform.position,end,1f/moveTime*Time.deltaTime);

		remainingDistance=(transform.position- end).sqrMagnitude;

		yield return null;
		}

		transform.position= end;

		isMoveing=false;


	}
    public void OncantMove(Damage hit)
    {
        hit.AttackDamage(attckDamege);

        animator.SetTrigger("ATK");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Food")
        {
            collision.gameObject.SetActive(false);

        }
        else if (collision.tag == "Soda")
        {
            collision.gameObject.SetActive(false);

        }
    }


}
