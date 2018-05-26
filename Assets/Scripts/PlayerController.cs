using UnityEngine;

public class PlayerController : PhysicsObject
{
    public float m_horizontalSpeed = 7f;
    public float m_jumpSpeed = 7f;

    private SpriteRenderer m_spriteRenderer;
    private Animator m_animator;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
    }

    protected override void CalculateMovement()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal") * m_horizontalSpeed;

        if (Input.GetButtonDown("Jump") && m_isGrounded)
        {
            m_velocity.y = m_jumpSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (m_velocity.y > 0f)
            {
                m_velocity *= 0.5f;
            }
        }

        bool flipX = m_spriteRenderer.flipX;        
        if ((move.x > 0.001f && flipX) ||
            (move.x < -0.001f && !flipX))
        {
            m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
        }

        m_animator.SetBool("grounded", m_isGrounded);
        m_animator.SetFloat("velocityX", Mathf.Abs(move.x) / m_horizontalSpeed);

        m_targetVelocity = move;
    }
}
