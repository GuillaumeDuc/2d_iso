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
    public Tilemap obstacles;

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

    public InfoScrollView EnemiesScrollView;

    public InfoScrollView PlayersScrollView;

    public Text dialogueText;

    // Player Speed
    private float moveSpeed = 5f;

    // Spawn Player
    private float posPlayerX = -2.5f, posPlayerY = -2.5f;

    public CurrentState CurrentState;
    public CastState CastState;

    private Dictionary<Unit, GameObject> enemyList;
    private Dictionary<Unit, GameObject> playerList;

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
        CastSystem.showArea(spell.getRange(tilemap, obstacles), cellsGrid);

        CurrentState = CurrentState.CAST;
        CastState = CastState.SHOW_AREA;
    }

    void Start()
    {
        // Get Player prefab from Assets/Resources
        GameObject PlayerPrefab = Resources.Load<GameObject>("Characters/PC/Player");
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
        PlayerStats.setSpellList(SpellList.Sandwall);
        PlayerStats.setStats("Player", 100, tilemap.WorldToCell(PlayerTransform.position));

        // Init Ennemies
        GameObject EnnemyPrefab = Resources.Load<GameObject>("Characters/NPC/Green");
        // First
        GameObject green1 = Instantiate(EnnemyPrefab, tilemap.CellToWorld(new Vector3Int(0, 0, 0)), Quaternion.identity);
        Transform green1Transform = green1.GetComponent<Transform>();
        Unit green1Stats = green1.GetComponent<Unit>();
        green1Stats.setSpellList(SpellList.Explosion);
        green1Stats.setStats("Green ennemy 1", 100, tilemap.WorldToCell(green1Transform.position));
        // Second
        GameObject green2 = Instantiate(EnnemyPrefab, tilemap.CellToWorld(new Vector3Int(-1, -2, 0)), Quaternion.identity);
        Transform green2Transform = green2.GetComponent<Transform>();
        Unit green2Stats = green2.GetComponent<Unit>();
        green2Stats.setSpellList(SpellList.Explosion);
        green2Stats.setStats("Green ennemy 2", 100, tilemap.WorldToCell(green2Transform.position));

        // Init RangeUtils
        RangeUtils = new RangeUtils();

        // Add characters in lists
        enemyList = new Dictionary<Unit, GameObject>() {
            { green1Stats, green1 },
            { green2Stats, green2 }
        };
        playerList = new Dictionary<Unit, GameObject>()
        {
            { PlayerStats, Player },
        };

        // Init UI
        // Spell scrollview
        foreach (var s in PlayerStats.spellList)
        {
            SpellScrollView.addSpell(s, PlayerStats);
        }
        // Ennemies UI
        foreach (var e in enemyList)
        {
            EnemiesScrollView.addInfo(e.Key);
        }
        // Player UI
        foreach (var e in playerList)
        {
            PlayersScrollView.addInfo(e.Key);
        }

        Tile transparent = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile");
        List<Vector3Int> displayPos = new List<Vector3Int>() { green1Stats.position, green2Stats.position };
        RangeUtils.setTileOnTilemap(displayPos, transparent, cellsGrid);
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
                if (tilemap.HasTile(cellPosition) && !obstacles.HasTile(cellPosition))
                {
                    MoveSystem.moveCharacter(Player, cellPosition, tilemap, obstacles);
                }

            }
            else if (CurrentState == CurrentState.CAST)
            {
                CastState = CastSystem.cast(
                    PlayerStats.selectedSpell,
                    PlayerStats,
                    cellPosition,
                    playerList,
                    enemyList,
                    CastState,
                    tilemap,
                    cellsGrid,
                    obstacles
                );
                if (CastState == CastState.DEFAULT)
                {
                    CurrentState = CurrentState.MOVE;
                }
            }
        }
    }
}
