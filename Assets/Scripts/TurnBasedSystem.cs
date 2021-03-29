using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public enum CurrentState { PLAYERTURN, ENNEMYTURN, WON, LOST }

public class TurnBasedSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile replacingTile;

    private GameObject Player;
    private Animator PlayerAnimator;
    private Rigidbody2D PlayerRigidBody;

    public MoveSystem MoveSystem;

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

        rb.MovePosition(pos * moveSpeed * Time.fixedDeltaTime);
    }

    void Start()
    {
        // Get Player prefab from Assets/Resources
        GameObject PlayerPrefab = Resources.Load<GameObject>("Characters/Player");
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
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector2 worldPositio = new Vector2(x,y);
            Debug.Log("MOVE TO " + worldPositio);
            MovePlayerWithPos(PlayerAnimator, PlayerRigidBody, worldPositio);

            // Left mouse click
            if (Input.GetMouseButtonDown(0))
            {
                // Get Mouse Input
                Vector2 screenPosition = new Vector2(
                    Input.mousePosition.x, 
                    Input.mousePosition.y
                    );

                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
                
                /*
                Vector3Int clickPos = tilemap.WorldToCell(worldPosition);
                TileBase tile = tilemap.GetTile(clickPos);
                if (tile != null)
                {
                    Debug.Log("x:" + worldPosition.x + " y:" + worldPosition.y + " tile:" + tile.name);
                }
                else
                {
                    Debug.Log("x:" + worldPosition.x + " y:" + worldPosition.y + " tile: (null)");
                }
                */

                // Move player
                if (tilemap.HasTile(cellPosition))
                {
                    MoveSystem.moveCharacter(Player, cellPosition, tilemap);
                }
                
            }

        }
    }
}
