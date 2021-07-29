using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum CurrentState { MOVE, CAST }

public enum CastState { DEFAULT, SHOW_RANGE, SHOW_AREA, CAST_SPELL }

public class TurnBasedSystem : MonoBehaviour
{
    // Grid
    public Tilemap tilemap;
    public Tilemap cellsGrid;

    public GameObject CameraView;

    private GameObject Player;
    private Transform PlayerTransform;

    // Scripts
    public MoveSystem MoveSystem;
    public CastSystem CastSystem;
    public SpellList SpellList;
    public DrawOnMap DrawOnMap;

    // UI
    public SpellScrollView SpellScrollView;
    public InfoScrollView EnemiesScrollView;
    public InfoScrollView PlayersScrollView;
    public GameObject EndResultUI;
    public Text dialogueText;

    public CurrentState CurrentState;
    public CastState CastState;

    public Dictionary<Unit, GameObject> enemyList;
    public Dictionary<Unit, GameObject> playerList;
    public Dictionary<Vector3Int, GameObject> obstacleList = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Unit, bool> initiativeList = new Dictionary<Unit, bool>();
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

    public void onClickSpell(GameObject spellGO, Unit unit)
    {
        unit.selectedSpell = spellGO;
        // Show on map

        Spell spell = spellGO.GetComponent<Spell>();
        DrawOnMap.showRange(spell.getRange(currentUnit, obstacleList, tilemap), true);

        CurrentState = CurrentState.CAST;
        CastState = CastState.SHOW_AREA;
    }

    public void addUnitInInitList(Unit unit)
    {
        initiativeList.Add(unit, unit.summon);
        initiativeList = initiativeList.OrderBy(x => x.Key.initiative).Reverse().ToDictionary(x => x.Key, x => x.Value);
        // Initiative list changed
        FightingSceneStore.initiativeList = initiativeList;
        if (currentUnit != null)
        {
            initiativeList[currentUnit] = false;
        }
        currentUnit = getUnitTurn();
        initiativeList[currentUnit] = true;

        DrawOnMap.resetMap();
        updateScrollViews();
    }

    private Unit getUnitTurn()
    {
        return initiativeList.FirstOrDefault(x => !x.Value).Key;
    }

    public void onClickEndTurn()
    {
        currentUnit = getUnitTurn();
        // IA finished playing
        IAisPlaying = false;
        DrawOnMap.resetMap();
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
        // Update status for characters
        applyStatusEntities();

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

    void applyStatusEntities()
    {
        //  Remove dead characters
        List<Unit> deadPlayerList = new List<Unit>();
        List<Unit> deadEnemyList = new List<Unit>();
        List<Vector3Int> destroyedObstacleList = new List<Vector3Int>();

        // Players
        foreach (var p in playerList)
        {
            // Take damages
            p.Key.takeStatus();
            // Update status
            p.Key.updateStatus();

            if (p.Key.currentHP <= 0)
            {
                deadPlayerList.Add(p.Key);
            }
        }
        // Enemies
        foreach (var e in enemyList)
        {
            // Take damages
            e.Key.takeStatus();
            // Update status
            e.Key.updateStatus();

            if (e.Key.currentHP <= 0)
            {
                deadEnemyList.Add(e.Key);
            }
        }
        // Obstacle
        foreach (var o in obstacleList)
        {
            Obstacle obstacle = o.Value.GetComponent<Obstacle>();
            // Take damages
            obstacle.takeStatus();
            // Update status
            obstacle.updateStatus();

            if (obstacle.currentHP <= 0)
            {
                destroyedObstacleList.Add(o.Key);
            }
        }

        foreach (var s in deadPlayerList)
        {
            playerList.Remove(s);
            initiativeList.Remove(s);
            DestroyImmediate(s.gameObject);
        }
        foreach (var s in deadEnemyList)
        {
            enemyList.Remove(s);
            initiativeList.Remove(s);
            DestroyImmediate(s.gameObject);
        }
        foreach (var s in destroyedObstacleList)
        {
            DestroyImmediate(obstacleList[s]);
            obstacleList.Remove(s);
        }
    }

    void tryEndGame()
    {
        string text = getWinningSideText();
        if (text != null)
        {
            Text endText = EndResultUI.GetComponentInChildren<Text>();
            Animator animator = EndResultUI.GetComponent<Animator>();
            endText.text = text;
            EndResultUI.SetActive(true);
            StartCoroutine(loadNewScene(animator));
        }
    }

    IEnumerator loadNewScene(Animator animator)
    {
        // Wait for anim to finish
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SelectionMap");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    string getWinningSideText()
    {
        string res = null;
        if (!playerList.Any() && !enemyList.Any())
        {
            res = "Draw";
        }
        else if (!playerList.Any())
        {
            res = "Lost";
        }
        else if (!enemyList.Any())
        {
            res = "Won";
        }
        return res;
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
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }

    void Start()
    {
        FightingSceneStore.TurnBasedSystem = this;

        // Instantiate state
        CurrentState = CurrentState.MOVE;
        // Casting state
        CastState = CastState.DEFAULT;

        // Get Player prefab from Assets/Resources
        GameObject PlayerPrefab = Resources.Load<GameObject>("Characters/PC/Player");
        Player = InstantiatePlayer(PlayerPrefab, new Vector3Int(15, 15, 0));

        // Get transform
        PlayerTransform = Player.GetComponent<Transform>();

        // Init Player
        Unit PlayerStats = Player.GetComponent<Unit>();

        // Init Ennemies
        GameObject EnemyPrefab = Resources.Load<GameObject>("Characters/NPC/Phantom/Phantom");
        // First
        GameObject phantom = InstantiatePlayer(EnemyPrefab, new Vector3Int(10, 15, 0));
        Transform green1Transform = phantom.GetComponent<Transform>();
        Unit green1Stats = phantom.GetComponent<Unit>();

        // Add characters in lists
        enemyList = new Dictionary<Unit, GameObject>() {
            { green1Stats, phantom },
        };
        playerList = new Dictionary<Unit, GameObject>()
        {
            { PlayerStats, Player },
        };

        // Init obstacle List
        Obstacle[] obstacleScripts = (Obstacle[])GameObject.FindObjectsOfType(typeof(Obstacle));
        foreach (var o in obstacleScripts)
        {
            Vector3Int pos = tilemap.WorldToCell(o.transform.position);
            obstacleList.Add(pos, o.gameObject);
        }

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

        // Store infos
        FightingSceneStore.CastSystem = CastSystem;
        FightingSceneStore.tilemap = tilemap;
        FightingSceneStore.cellsGrid = cellsGrid;
        FightingSceneStore.playerList = playerList;
        FightingSceneStore.enemyList = enemyList;
        FightingSceneStore.obstacleList = obstacleList;
        FightingSceneStore.initiativeList = initiativeList;
        FightingSceneStore.PlayersScrollView = PlayersScrollView;
        FightingSceneStore.EnemiesScrollView = EnemiesScrollView;
    }

    void Update()
    {
        /*
        dialogueText.text = "Current State : " + CurrentState + "\n" +
            "Cast State : " + CastState + "\n" +
            "Turn : " + currentTurn + "\n" +
            "Current unit : \n" + currentUnit;
        */
        // Move Camera
        Vector3 posPlayer = PlayerTransform ? PlayerTransform.position : CameraView.transform.position;
        CameraView.transform.position = new Vector3(posPlayer.x, posPlayer.y, -10);

        // Everytime a spell has been casted
        if (CastSystem.casted)
        {
            tryEndGame();
            CastSystem.casted = false;
            tilemap.RefreshAllTiles();
        }

        // Play Enemies
        if (currentUnit != null && !currentUnit.playable && !IAisPlaying)
        {
            GameObject currentEnemy = currentUnit.gameObject;
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

            GameObject currentPlayer = currentUnit.gameObject;
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
                        CastState
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
