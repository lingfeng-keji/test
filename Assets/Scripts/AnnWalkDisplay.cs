using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;

public class AnnWalkDisplay : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.down;
    private Animator anim = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rigidbody2D = GetComponent<Rigidbody2D>();
        //UnityEngine.Assertions.Assert.IsNotNull(rigidbody2D);

        anim = GetComponent<Animator>();
        UnityEngine.Assertions.Assert.IsNotNull(anim);

        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", moveDirection.y);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", moveDirection.y);
    }
}
