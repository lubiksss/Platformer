using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public SoundManager soundManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator animator;
    CapsuleCollider2D capsulecollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
    }

    //단발적인 입력에 대해서는 여기에 쓴다.
    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Vertical") && !animator.GetBool("IsJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);

            //Jump Sound
            soundManager.source.clip = soundManager.audioJump;
            soundManager.source.Play();
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            // rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x, rigid.velocity.y);
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        //Flip Sprite
        // if (Input.GetButtonDown("Horizontal"))
        if (Input.GetButton("Horizontal"))
        {
            sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.5)
        {
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsRunning", true);
        }
    }

    //지속적인 입력의 경우에는 여기에 쓴다.
    void FixedUpdate()
    {
        //Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 2, ForceMode2D.Impulse);
        if (rigid.velocity.x > maxSpeed)//Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1))//Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }
        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                // if (rayHit.distance < 0.5f)
                // {
                // Debug.Log(rayHit.collider.name);
                // Debug.Log(rayHit.distance);
                animator.SetBool("IsJumping", false);
                // }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //몬스터 머리위에 있음 + 아래로 내려오는 중에 밟음
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                //Reaction Force
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                //Attack
                OnAttack(collision.transform);
                //Attack Sound
                soundManager.source.clip = soundManager.audioAttack;
                soundManager.source.Play();

            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
        else if (collision.gameObject.tag == "Spike")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {
                gameManager.stagePoint += 100;
            }
            //Deactive Item
            collision.gameObject.SetActive(false);
            //Item Sound
            soundManager.source.clip = soundManager.audioItem;
            soundManager.source.Play();
        }
        else if (collision.gameObject.tag == "Finish")
        {
            //Next Stage
            Debug.Log("finish");
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;
        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

    }

    void OnDamaged(Vector2 targetPos)
    {
        //Health Down
        gameManager.HealthDown();
        // Change Layer (Immortal Active)
        gameObject.layer = 11;
        // View Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);
        // Reactive Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc * 30, 5), ForceMode2D.Impulse);

        //Animation
        animator.SetTrigger("doDamaged");

        //OnDamaged Sound
        soundManager.source.clip = soundManager.audioDamaged;
        soundManager.source.Play();


        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        sprite.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Die Sound
        soundManager.source.clip = soundManager.audioDie;
        soundManager.source.Play();
        //Sprite Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        sprite.flipY = true;
        //Collider Disable
        capsulecollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        //Die Sound
    }
}
