using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public enum CurrentState { MOVE, CAST }

public enum CastState { DEFAULT, SHOW_RANGE, SHOW_AREA, CAST_SPELL }

public class TurnBasedSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap cellsGrid;
    public Tile replacingTile;

    private GameObject Player;
    private Animator PlayerAnimator;
    private Rigidbody2D PlayerRigidBody;
    private Transform PlayerTransform;
    private Unit PlayerStats;

    private RangeUtils RangeUtils;

    public MoveSystem MoveSystem;

    public CastSystem CastSystem;

    public SpellList SpellList;

    public SpellScrollView SpellScrollView;

    public Text dialogueText;

    // Player Speed
    private float moveSpeed = 5f;

    // Spawn Player
    private float posPlayerX = -2.5f, posPlayerY = -2.5f;

    public CurrentState CurrentState;
    public CastState CastState;

    void InstantiatePlayer(GameObject PlayerPrefab)
    {
        Vector2 pos = new Vector2(posPlayerX, posPlayerY);
        Player = Instantiate(PlayerPrefab, pos, Quaternion.identity);
    }

    void MovePlayerWithPos(Animator animator, Rigidbody2D rb, Vector2 pos)
    {
        animator.SetFloat("Horizontal", pos.x);
        animator.SetFloat("Vertical", pos.y);
        animator.SetFloat("Speed", Mathf.Abs(pos.sqrMagnitude));

        rb.MovePosition(pos * moveSpeed * Time.fixedDeltaTime);
    }

    public void onClickSpell(Spell spell, Unit unit)
    {
        // Remove any previously added range
        RangeUtils.removeCells(cellsGrid);

        unit.selectedSpell = spell;
        spell.casterPos = unit.position;
        CastSystem.showArea(spell.getRange(tilemap), cellsGrid);

        CurrentState = CurrentState.CAST;
        CastState = CastState.SHOW_AREA;
    }

    void Start()
    {
        // Get Player prefab from Assets/Resources
        GameObject PlayerPrefab = Resources.Load<GameObject>("Characters/Player");
        InstantiatePlayer(PlayerPrefab);

        // Instantiate state
        CurrentState = CurrentState.MOVE;
        // Casting state
        CastState = CastState.DEFAULT;

        // Get Animation and Rigidbody from player to move it
        PlayerAnimator = Player.GetComponent<Animator>();
        PlayerRigidBody = Player.GetComponent<Rigidbody2D>();
        PlayerTransform = Player.GetComponent<Transform>();

        // Init Player
        PlayerStats = Player.GetComponent<Unit>();
        PlayerStats.setSpellList(SpellList.Explosion);
        PlayerStats.setSpellList(SpellList.Icycle);
        PlayerStats.setStats("Player", 100, tilemap.WorldToCell(PlayerTransform.position));

        // Init UI
        foreach (var s in PlayerStats.spellList)
        {
            SpellScrollView.addSpell(s, PlayerStats);
        }

        // Init RangeUtils
        RangeUtils = new RangeUtils();
    }

    void Update()
    {

        dialogueText.text = "Current State : " + CurrentState + "\n" + "Cast State : " + CastState;

        // Get keys input
        /*
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 worldPositio = new Vector2(x,y);
        Debug.Log("MOVE TO " + worldPositio);
        MovePlayerWithPos(PlayerAnimator, PlayerRigidBody, worldPositio);
        */

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

            if (CurrentState == CurrentState.MOVE)
            {
                // Move player
                if (tilemap.HasTile(cellPosition))
                {
                    MoveSystem.moveCharacter(Player, cellPosition, tilemap);
                }

            }
            else if (CurrentState == CurrentState.CAST)
            {
                CastState = CastSystem.cast(PlayerStats.selectedSpell, PlayerStats, cellPosition, CastState, tilemap, cellsGrid);
                if (CastState == CastState.DEFAULT)
                {
                    CurrentState = CurrentState.MOVE;
                }
            }
        }
    }
}
