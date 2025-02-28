using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;

    private bool facingRight = true;

    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask wnatIsGround;

    private Animator anim;

    // Слой для стен
    public LayerMask wallLayer;

    private bool isOnWall = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Проверка на застревание на стене
        if (IsOnWall())
        {
            isOnWall = true;
            Debug.Log("На стене");
            // Скользить по стене вниз
            if (!isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -speed * Time.deltaTime));
                Debug.Log("Скользим вниз");
            }
        }
        else
        {
            isOnWall = false;
            Debug.Log("Не на стене");
        }

        if (!facingRight && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight && moveInput < 0)
        {
            Flip();
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, wnatIsGround);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            anim.SetTrigger("takeOF");
        }

        anim.SetBool("isJumping", !isGrounded);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    bool IsOnWall()
    {
        // Проверка наличия стены слева или справа
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.1f, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.1f, wallLayer);

        // Отладочные сообщения
        if (hitLeft.collider != null)
        {
            Debug.Log("Стена слева");
        }
        if (hitRight.collider != null)
        {
            Debug.Log("Стена справа");
        }

        return (hitLeft.collider != null && moveInput < 0) || (hitRight.collider != null && moveInput > 0);
    }
}
