using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator animator;
    public int nextMove;
    SpriteRenderer sprite;
    CapsuleCollider2D capsulecollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        //

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            // Debug.Log("warning");
            nextMove *= -1;
            sprite.flipX = nextMove == 1;
            CancelInvoke();
            Invoke("Think", 5);
        }

    }
    void Think()
    {
        nextMove = Random.Range(-1, 2);

        //Animation
        if (nextMove != 0)
        {
            animator.SetInteger("RunningSpeed", nextMove);
        }
        //Flip Sprite
        if (nextMove != 0)
        {
            sprite.flipX = nextMove == 1;
        }

        float nextThinkTime = Random.Range(2, 5);
        Invoke("Think", nextThinkTime);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        sprite.flipY = true;
        //Collider Disable
        capsulecollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //Destory
        Invoke("DeActive", 5);
    }
    void DeActive()
    {
        gameObject.SetActive(false);
    }
}