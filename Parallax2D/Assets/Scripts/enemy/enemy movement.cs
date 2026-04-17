using UnityEngine;
using System.Collections.Generic;

public class NewMonoBehaviourScript : MonoBehaviour
{
   [SerializeField] private Rigidbody2D rb;
   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private float speed = 3f;
   [SerializeField] private int startDirection = 1;

   private int currentDirection;
   private float halfWidth;
   private Vector2 movement;
   
    private void Start()
    {
        halfWidth = spriteRenderer.bounds.extents.x;
        currentDirection = startDirection;
        spriteRenderer.flipX = startDirection == 1 ? false : true;
    }

    
    private void FixedUpdate()
    {
        movement.x = currentDirection * speed;
        movement.y = rb.linearVelocity.y;
        rb.linearVelocity = movement;
        SetDirection();
    }


    private void SetDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground")) &&
        rb.linearVelocity.x > 0){
            currentDirection *= -1;
            spriteRenderer.flipX = true;
        }else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground")) &&
        rb.linearVelocity.x < 0){
            currentDirection *= -1;
            spriteRenderer.flipX = false;
        }


        Debug.DrawRay(transform.position, Vector2.right * (halfWidth + 0.1f), Color.red);
     } 
   
   
   
   
   
 }

