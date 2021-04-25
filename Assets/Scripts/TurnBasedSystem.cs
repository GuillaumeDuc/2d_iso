using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Linq;

public enum CurrentState { MOVE, CAST }

public enum CastState { DEFAULT, SHOW_RANGE, SHOW_AREA, CAST_SPELL }

public class TurnBasedSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap cellsGrid;

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
    private Dictionary<Vector3Int, GameObject> obstacleList;

    private Dictionary<Unit, bool> initiativeList;
    private Unit currentUnit;
    private int currentTurn = 1;

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
        CastSystem.showArea(spell.getRange(obstacleList, tilemap), cellsGrid);

        CurrentState = CurrentState.CAST;
        CastState = CastState.SHOW_AREA;
    }

    private Dictionary<Unit, bool> getInitList()
    {
        Dictionary<Unit, bool> fullList = new Dictionary<Unit, bool>(
            playerList
            .Concat(enemyList)
            .OrderBy(x => x.Key.initiative)
            .Reverse()
            .ToDictionary(x => x.Key, x => false)
        );
        return fullList;
    }

    private Unit getUnitTurn()
    {
        return initiativeList.FirstOrDefault(x => !x.Value).Key;
    }

    public void onClickEndTurn()
    {
        currentUnit = getUnitTurn();
        // Next character
        if (currentUnit != null)
        {
            initiativeList[currentUnit] = true;
        }
        // All characters have played, next turn
        if (currentUnit == null)
        {
            currentTurn += 1;
            // Playing for all character = false
            foreach (var key in initiativeList.Keys.ToList())
            {
                initiativeList[key] = false;
            }
            // New turn
            currentUnit = getUnitTurn();
            initiativeList[currentUnit] = true;
            // Apply all status on players then update
            applyStatus();
            updateScrollViews();
        }
    }

    public void updateScrollViews()
    {
        foreach(var enemy in enemyList)
        {
            EnemiesScrollView.setSliderHP(enemy.Key);
        }
        foreach (var player in playerList)
        {
            PlayersScrollView.setSliderHP(player.Key);
        }
    }

    public void applyStatus()
    {
        Dictionary<Unit, GameObject> allCharacters = playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value);
        foreach (var c in allCharacters)
        {
            // Take damages
            c.Key.takeStatus();
            // Update status
            c.Key.updateStatus();
        }
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
        PlayerStats.setStats("Player", tilemap.WorldToCell(PlayerTransform.position), 100, 3 ,110);
        PlayerStats.playable = true;

        // Init Ennemies
        GameObject EnnemyPrefab = Resources.Load<GameObject>("Characters/NPC/Green");
        // First
        GameObject green1 = Instantiate(EnnemyPrefab, tilemap.CellToWorld(new Vector3Int(0, 0, 0)), Quaternion.identity);
        Transform green1Transform = green1.GetComponent<Transform>();
        Unit green1Stats = green1.GetComponent<Unit>();
        green1Stats.setSpellList(SpellList.Explosion);
        green1Stats.setStats("Green ennemy 1", tilemap.WorldToCell(green1Transform.position), 100);
        // Second
        GameObject green2 = Instantiate(EnnemyPrefab, tilemap.CellToWorld(new Vector3Int(-1, -2, 0)), Quaternion.identity);
        Transform green2Transform = green2.GetComponent<Transform>();
        Unit green2Stats = green2.GetComponent<Unit>();
        green2Stats.setSpellList(SpellList.Explosion);
        green2Stats.setStats("Green ennemy 2", tilemap.WorldToCell(green2Transform.position), 100);

        // Init RangeUtils
        RangeUtils = new RangeUtils();

        // Add characters in lists
        enemyList = new Dictionary<Unit, GameObject>() {
            { green1Stats, green1 },
            { green2Stats, green2 }
        };
        List<GameObject> a = new List<GameObject>();
        playerList = new Dictionary<Unit, GameObject>()
        {
            { PlayerStats, Player },
        };

        // Init obstacle List
        obstacleList = new Dictionary<Vector3Int, GameObject>();

        // Init initiative list
        initiativeList = new Dictionary<Unit, bool>(getInitList());

        // Init current turn
        currentUnit = getUnitTurn();
        initiativeList[currentUnit] = true;

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
        dialogueText.text = "Current State : " + CurrentState + "\n" +
            "Cast State : " + CastState + "\n" +
            "Turn : " + currentTurn + "\n" +
            "Current unit : \n" + currentUnit;

        // Get keys input
        /*
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 worldPositio = new Vector2(x,y);
        Debug.Log("MOVE TO " + worldPositio);
        MovePlayerWithPos(PlayerAnimator, PlayerRigidBody, worldPositio);
        */
        if (Input.GetMouseButtonDown(0))
        {
            // Get Mouse Input
            Vector2 screenPosition = new Vector2(
                Input.mousePosition.x,
                Input.mousePosition.y
            );

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
            
            Sprite brickSp = Resources.Load<Sprite>("Tilemaps/Brick/brick_short_tile_iso");
            Sprite brickSp2 = Resources.Load<Sprite>("Tilemaps/Brick/brick_tile_iso");
            Sprite[] spl = new Sprite[2];
            spl[0] = brickSp;
            spl[1] = brickSp2;

            GroundTile gd = Resources.Load<GroundTile>("ground");
            gd.animatedSprites = spl;
            tilemap.SetTile(cellPosition, gd);
        }
        /*
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
                if (tilemap.HasTile(cellPosition) && !obstacleList.ContainsKey(cellPosition))
                {
                    MoveSystem.moveCharacter(Player, cellPosition, obstacleList, tilemap);
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
                    obstacleList,
                    CastState,
                    tilemap,
                    cellsGrid
                );
                if (CastState == CastState.DEFAULT)
                {
                    CurrentState = CurrentState.MOVE;
                }
            }
        }
        */
    }
}
