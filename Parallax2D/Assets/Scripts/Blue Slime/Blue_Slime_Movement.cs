using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blue_Slime_Movement : MonoBehaviour
{
    public Transform slimeTransform;
    public float moveSpeed;

    public Transform playerTransform;
    public bool isChasing;
    public float chaseDistance;

    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (isChasing)
        {
            if(transform.position.x > playerTransform.position.x)
            {
                transform.localScale = new Vector3(-10,10,1);
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (transform.position.x < playerTransform.position.x)
            {
                transform.localScale = new Vector3(10, 10, 1);
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
            {
                isChasing = true;
            }
            else
            {
                anim.Play("Blue_Slime_Idle");
            }
            }
    }
}
