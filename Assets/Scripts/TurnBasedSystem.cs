using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentState { PLAYERTURN, ENNEMYTURN, WON, LOST }

public class TurnBasedSystem : MonoBehaviour
{

    private GameObject Player;
    private Animator PlayerAnimator;
    private Rigidbody2D PlayerRigidBody;

    // Player Speed
    private float moveSpeed = 5f;

    // Spawn Player
    private float posPlayerX = -2.5f, posPlayerY = -2.5f;

    public CurrentState State;

    void InstantiatePlayer(GameObject PlayerPrefab)
    {
        Vector2 pos = new Vector2(posPlayerX, posPlayerY);
        Player = Instantiate(PlayerPrefab, pos, Quaternion.identity);
    }
    
    public void MovePlayerWithPos(Animator animator, Rigidbody2D rb, Vector2 pos)
    {
        animator.SetFloat("Horizontal", pos.x);
        animator.SetFloat("Vertical", pos.y);
        animator.SetFloat("Speed", Mathf.Abs(pos.sqrMagnitude));

        Debug.Log(pos + "\n" + rb.position );



        rb.MovePosition(pos * moveSpeed * Time.fixedDeltaTime);
    }

    void Start()
    {
        // Get Player prefab from Assets/Resources
        GameObject PlayerPrefab = Resources.Load<GameObject>("Player");
        InstantiatePlayer(PlayerPrefab);
        State = CurrentState.PLAYERTURN;
        // Get Animation and Rigidbody from player to move it
        PlayerAnimator = Player.GetComponent<Animator>();
        PlayerRigidBody = Player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (State == CurrentState.PLAYERTURN)
        {
            // Get keys input
            // float x = Input.GetAxisRaw("Horizontal");
            // float y = Input.GetAxisRaw("Vertical");
            // Vector2 worldPostion = new Vector2(x,y);
            
            // Left mouse click
            if (Input.GetMouseButtonDown(0))
            {
                // Get Mouse Input
                Vector2 screenPosition = new Vector2(
                    Input.mousePosition.x, 
                    Input.mousePosition.y
                    );

                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                
                // Move & Animate Player
                MovePlayerWithPos(PlayerAnimator, PlayerRigidBody, worldPosition);
            }

        }
    }
}
