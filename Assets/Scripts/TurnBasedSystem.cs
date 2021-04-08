using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public enum CurrentState { MOVE, CAST }

public enum CastState { DEFAULT, SELECT_AREA, CONFIRM_AREA, CAST_SPELL }

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

    // Freeze spell selection
    private Spell selectedSpell;
    private Vector3Int playerPos;
    private Vector3Int clickedPos;
    private Vector3Int secondClickPos;

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
        CastSystem.showSpellRange(spell, unit.position, cellsGrid, tilemap);

        CurrentState = CurrentState.CAST;
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

        if (CurrentState == CurrentState.MOVE)
        {
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

                // Move player
                if (tilemap.HasTile(cellPosition))
                {
                    MoveSystem.moveCharacter(Player, cellPosition, tilemap);
                }
            }

        }
        else if (CurrentState == CurrentState.CAST)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Get Mouse Input
                Vector2 screenPosition = new Vector2(
                    Input.mousePosition.x,
                    Input.mousePosition.y
                );

                // Position in 2d on screen
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                // Position in world
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

                if (CastState == CastState.DEFAULT)
                {
                    // Remove any cells on cellsGrid
                    RangeUtils.removeCells(cellsGrid);

                    // Save spell selected from SpellScrollView
                    selectedSpell = PlayerStats.selectedSpell;
                    // Player Position in world
                    playerPos = tilemap.WorldToCell(PlayerTransform.position);
                    // Cell clicked
                    clickedPos = cellPosition;

                    if (CastSystem.canCast(selectedSpell, clickedPos, playerPos, RangeUtils.getAreaCircleFull(playerPos, selectedSpell.range, tilemap), tilemap))
                    {
                        // Show Area (in white if selectable area
                        if (selectedSpell.selectableArea)
                        {
                            CastSystem.showSpellArea(selectedSpell, clickedPos, playerPos, cellsGrid, tilemap, new Color(1, 1, 1, 0.5f));
                        }
                        else
                        {
                            CastSystem.showSpellArea(selectedSpell, clickedPos, playerPos, cellsGrid, tilemap);
                        }

                        // Move to casting the spell or selecting the area
                        if (selectedSpell.selectableArea)
                        {
                            CastState = CastState.SELECT_AREA;
                        }
                        else
                        {
                            CastState = CastState.CAST_SPELL;
                        }
                    }
                    else
                    {
                        CurrentState = CurrentState.MOVE;
                    }
                }
                else if (CastState == CastState.SELECT_AREA)
                {
                    // Remove any cells on cellsGrid
                    RangeUtils.removeCells(cellsGrid);

                    secondClickPos = cellPosition;
                    // Show Area
                    if (CastSystem.canCast(selectedSpell, clickedPos, secondClickPos, selectedSpell.getArea(clickedPos, playerPos, tilemap), tilemap))
                    {
                        CastSystem.showAreaSelected(clickedPos, secondClickPos, cellsGrid);
                        CastState = CastState.CAST_SPELL;
                    }
                    else
                    {
                        CastState = CastState.DEFAULT;
                        CurrentState = CurrentState.MOVE;
                    }
                }
                else if (CastState == CastState.CAST_SPELL)
                {
                    // Remove any cells on cellsGrid
                    RangeUtils.removeCells(cellsGrid);
                    if (selectedSpell.selectableArea)
                    {
                        CastSystem.castSpell(selectedSpell, secondClickPos, clickedPos, tilemap);
                    }
                    else
                    {
                        if (CastSystem.canCast(selectedSpell, cellPosition, playerPos, selectedSpell.getArea(clickedPos, playerPos, tilemap), tilemap))
                        {
                            CastSystem.castSpell(selectedSpell, clickedPos, playerPos, tilemap);
                        }
                    }
                    // Cast state over, set cast state to default and switch to move state
                    CastState = CastState.DEFAULT;
                    CurrentState = CurrentState.MOVE;
                }
            }
        }
    }
}
