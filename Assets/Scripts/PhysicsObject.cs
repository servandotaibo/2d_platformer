using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public const float MIN_COLLISION_NORMAL_Y = 0.65f;
    public const float MIN_MOVE_DISTANCE = 0.001f;
    public const float SHELL_RADIUS = 0.01f;

    public float m_gravityModifier = 1f;

    protected Rigidbody2D m_rigidBody;
    protected Vector2 m_velocity;
    public Vector2 m_targetVelocity;
    protected ContactFilter2D m_contactFilter;
    protected RaycastHit2D[] m_collisionHits = new RaycastHit2D[16];

    protected bool m_isGrounded = false;
    private Vector2 m_moveAlongGround;

    private void OnEnable()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();    
    }

    private void Start()
    {
        m_contactFilter.useTriggers = false;
        m_contactFilter.useLayerMask = true;
        m_contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }

    private void Update()
    {
        m_targetVelocity = Vector2.zero;
        CalculateMovement();    
    }

    protected virtual void CalculateMovement()
    {    
    }

    void FixedUpdate ()
    {
        m_isGrounded = false;
        m_velocity += m_gravityModifier * Physics2D.gravity * Time.deltaTime;
        m_velocity.x = m_targetVelocity.x;

        Vector2 delta = m_velocity * Time.deltaTime;

        Vector2 move = m_moveAlongGround * delta.x;
        Move(move, false);

        move = Vector2.up * delta.y;
        Move(move, true);        
	}

    private void Move(Vector2 move, bool isYMove)
    {
        float distance = move.magnitude;

        // To prevent this physics object from being applied physics when it's static
        if (distance > MIN_MOVE_DISTANCE)
        {
            int count = m_rigidBody.Cast(move, m_contactFilter, m_collisionHits, distance + SHELL_RADIUS);

            for (int i = 0; i < count; i++)
            {
                Vector2 collisionNormal = m_collisionHits[i].normal;

                // Makes sure the character can stand on this collision
                if (collisionNormal.y > MIN_COLLISION_NORMAL_Y)
                {
                    m_isGrounded = true;

                    if (isYMove)
                    {
                        m_moveAlongGround = new Vector2(collisionNormal.y, -collisionNormal.x);
                        collisionNormal.x = 0;
                    }
                }

                // Checks if we need to correct velocity to prevent the character from going inside the collider
                float projection = Vector2.Dot(m_velocity, collisionNormal);
                if (projection < 0)
                {
                    m_velocity = m_velocity - projection * collisionNormal;
                }

                float modifiedDistance = m_collisionHits[i].distance - SHELL_RADIUS;
                if (modifiedDistance < distance)
                {
                    distance = modifiedDistance;
                }
            }

            m_rigidBody.position += move.normalized * distance;
        }
    }
}
