using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public enum CurrentState { MOVE, CAST }

public enum CastState { DEFAULT, SHOW_RANGE, SHOW_AREA, CAST_SPELL }

public class TurnBasedSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap cellsGrid;

    public GameObject CameraView;

    private GameObject Player;
    private Transform PlayerTransform;

    public MoveSystem MoveSystem;

    public CastSystem CastSystem;

    public SpellList SpellList;

    public TileList TileList;

    public DrawOnMap DrawOnMap;

    public SpellScrollView SpellScrollView;

    public InfoScrollView EnemiesScrollView;

    public InfoScrollView PlayersScrollView;

    public Text dialogueText;

    public CurrentState CurrentState;
    public CastState CastState;

    public Dictionary<Unit, GameObject> enemyList;
    public Dictionary<Unit, GameObject> playerList;
    public Dictionary<Vector3Int, GameObject> obstacleList;

    private Dictionary<Unit, bool> initiativeList;
    private Unit currentUnit;
    private int currentTurn = 1;

    private bool IAisPlaying;

    GameObject InstantiatePlayer(GameObject PlayerPrefab, Vector3Int pos)
    {
        GroundTile tile = (GroundTile)tilemap.GetTile(pos);
        Vector3Int newPos = pos;
        while (!tile.walkable)
        {
            if (newPos.y < 20)
            {
                newPos.y += 1;
            }
            else
            {
                newPos = pos;
                newPos.x += 1;
            }
            tile = (GroundTile)tilemap.GetTile(newPos);
        }
        Vector3 cellToWorldVector = tilemap.CellToWorld(newPos);
        cellToWorldVector.y += 0.2f;
        return Instantiate(
            PlayerPrefab,
            cellToWorldVector,
            Quaternion.identity
        );
    }

    public void onClickSpell(Spell spell, Unit unit)
    {
        unit.selectedSpell = spell;
        // Show on map
        DrawOnMap.showRange(spell.getRange(unit, obstacleList, tilemap), true);

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
            foreach (var key in initiativeList.Keys.ToList())
            {
                // Playing for all character = false
                initiativeList[key] = false;
                // Reset stats (mana and movement)
                key.resetStats();
            }
            // New turn
            currentUnit = getUnitTurn();
            initiativeList[currentUnit] = true;
            // IA finished playing
            IAisPlaying = false;
            // Apply all status on players then update
            applyStatus();
            updateScrollViews();
        }
    }

    public void updateScrollViews()
    {
        EnemiesScrollView.updateScrollView();
        PlayersScrollView.updateScrollView();
    }

    public void applyStatus()
    {
        // Update status for all characters
        Dictionary<Unit, GameObject> allCharacters = playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value);
        foreach (var c in allCharacters)
        {
            // Take damages
            c.Key.takeStatus();
            // Update status
            c.Key.updateStatus();
        }

        // Update status for all tiles
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null && tile is GroundTile)
                {
                    GroundTile gt = (GroundTile)tile;
                    gt.updateStatus();
                }
            }
        }
        tilemap.RefreshAllTiles();
    }

    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void Start()
    {
        // Get Player prefab from Assets/Resources
        GameObject PlayerPrefab = Resources.Load<GameObject>("Characters/PC/Player");
        Player = InstantiatePlayer(PlayerPrefab, new Vector3Int(5, 0, 0));

        // Instantiate state
        CurrentState = CurrentState.MOVE;
        // Casting state
        CastState = CastState.DEFAULT;

        // Get transform
        PlayerTransform = Player.GetComponent<Transform>();

        // Init Player
        Unit PlayerStats = Player.GetComponent<Unit>();
        PlayerStats.setSpellList(SpellList.Explosion);
        PlayerStats.setSpellList(SpellList.Icycle);
        PlayerStats.setSpellList(SpellList.Sandwall);
        PlayerStats.setSpellList(SpellList.Blackhole);
        PlayerStats.setSpellList(SpellList.Teleportation);
        PlayerStats.setSpellList(SpellList.Slash);
        PlayerStats.setStats(Player, "Player", tilemap.WorldToCell(PlayerTransform.position), 100, 3, 110);
        PlayerStats.playable = true;

        // Init Ennemies
        GameObject EnemyPrefab = Resources.Load<GameObject>("Characters/NPC/Phantom/Phantom");
        // First
        GameObject phantom = InstantiatePlayer(EnemyPrefab, new Vector3Int(10, 15, 0));
        Transform green1Transform = phantom.GetComponent<Transform>();
        Unit green1Stats = phantom.GetComponent<Unit>();
        green1Stats.setSpellList(SpellList.Slash);
        green1Stats.setSpellList(SpellList.Teleportation);
        green1Stats.setStats(phantom, "Phantom", tilemap.WorldToCell(green1Transform.position), 100, 0, 100, 10, 3);

        // Add characters in lists
        enemyList = new Dictionary<Unit, GameObject>() {
            { green1Stats, phantom },
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
        // Draw position on map
        DrawOnMap.resetMap();
    }

    void Update()
    {
        dialogueText.text = "Current State : " + CurrentState + "\n" +
            "Cast State : " + CastState + "\n" +
            "Turn : " + currentTurn + "\n" +
            "Current unit : \n" + currentUnit;

        // Move Camera
        Vector3 posPlayer = PlayerTransform.position;
        CameraView.transform.position = new Vector3(posPlayer.x, posPlayer.y, -10);

        // Play Enemies
        if (!currentUnit.playable && !IAisPlaying)
        {
            GameObject currentEnemy = currentUnit.unitGO;
            EnemyAI enemyAI = currentUnit.GetComponent<EnemyAI>();
            IAisPlaying = true;
            enemyAI.play(
                MoveSystem,
                CastSystem,
                obstacleList,
                playerList,
                enemyList,
                tilemap,
                onClickEndTurn
            );
        }

        // Left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Reset map when clicking
            DrawOnMap.resetMap();

            // Get Mouse Input
            Vector2 screenPosition = new Vector2(
                Input.mousePosition.x,
                Input.mousePosition.y
            );

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

            GameObject currentPlayer = currentUnit.unitGO;
            if (currentUnit.playable)
            {
                if (CurrentState == CurrentState.MOVE && !IsPointerOverUIElement())
                {
                    // Move player
                    if (tilemap.HasTile(cellPosition) && !obstacleList.ContainsKey(cellPosition))
                    {
                        MoveSystem.moveCharacter(currentPlayer, cellPosition, obstacleList, tilemap);
                        updateScrollViews();
                    }
                }
                else if (CurrentState == CurrentState.CAST)
                {
                    CastState = CastSystem.cast(
                        currentUnit.selectedSpell,
                        currentUnit,
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

                        // Reset map when exiting click
                        DrawOnMap.resetMap();
                    }
                }
            }
        }
    }
}
