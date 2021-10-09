using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum AnimationState { Idle, Attack, Cast, WalkLeft, WalkRight, Hurt }

public class SpellInstantiateList : MonoBehaviour
{
    static SpellInstantiateList instance;
    void Start()
    {
        instance = this;
    }

    public static void instantiateArea(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(target, caster, obstacleList, tilemap);
        // Instantiate spell only one time on first cell
        Vector2 worldPos = tilemap.CellToWorld(listCells.Count > 0 ? listCells[0] : target);
        // Instantiate animation
        GameObject go = Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        setSpellPosition(go, target);
        // Remove cell clicked from list cells
        if (listCells.Count > 0) listCells.RemoveAt(0);

        multipleInstantiateOnCell(listCells, go, tilemap);
    }

    static void multipleInstantiateOnCell(
        List<Vector3Int> listCells,
        GameObject gameObject,
        Tilemap tilemap
        )
    {
        foreach (var c in listCells)
        {
            GameObject newGO = Instantiate(gameObject, tilemap.CellToWorld(c), Quaternion.identity);
            Destroy(newGO.GetComponent<Spell>());
            Destroy(newGO.GetComponent<SpellEffectScript>());
            Destroy(newGO.GetComponent<SpellDamage>());
        }
    }

    public static void instantiateOnCellClicked(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Vector2 worldPos = tilemap.CellToWorld(target);
        // Instantiate animation
        GameObject go = Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        setSpellPosition(go, target);
    }

    public static void instantiateObstacles(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(target, caster, obstacleList, tilemap);
        instance.StartCoroutine(delayInstantiateObstacle(listCells, spell.gameObject, obstacleList, tilemap));
    }

    static IEnumerator delayInstantiateObstacle(
        List<Vector3Int> area,
        GameObject spellGO,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        foreach (var pos in area)
        {
            yield return new WaitForSeconds(0.1f);
            Vector2 worldPos = tilemap.CellToWorld(pos);
            GameObject obstacle = Instantiate(spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
            setSpellPosition(obstacle, pos);
            try
            {
                obstacleList.Add(pos, obstacle);
            }
            catch
            {
                Destroy(obstacle);
            }
        };
    }

    public static void instantiateThrowedSpell(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Vector2 worldPos = tilemap.CellToWorld(target);
        GameObject go = Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        setSpellPosition(go, target);
        GameObject goChild = go.transform.GetChild(0).gameObject;

        Vector2 casterPos = tilemap.CellToWorld(caster.position);
        casterPos.y += 0.2f;
        // Change position of actual spell
        goChild.transform.position = casterPos;

        // Rotate spell toward clicked cell
        Vector3 vectorToTarget = target - caster.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        goChild.transform.Rotate(0, 0, angle - 45, Space.Self);
    }

    public static void instantiateAttack(
       Spell spell,
       Unit caster,
       Vector3Int target,
       Dictionary<Vector3Int, GameObject> obstacleList,
       Tilemap tilemap
       )
    {
        Vector2 worldPos = tilemap.CellToWorld(target);
        // Instantiate player attack animation
        animateCasterAttack(caster);
        // Instantiate animation
        GameObject go = Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        setSpellPosition(go, target);
    }

    private static void animateCasterAttack(Unit caster)
    {
        Animator animator = caster.gameObject.GetComponent<Animator>();
        // If GO has an animator component & attack state exists
        if (animator != null && animator.HasState(0, Animator.StringToHash(AnimationState.Attack.ToString())))
        {
            animator.Play(AnimationState.Attack.ToString(), -1, 0);
        }
    }

    public static void instantiateMoveInLineFromPlayer(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {

        Vector2 worldPos = tilemap.CellToWorld(target);
        GameObject go = Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.19f), Quaternion.identity);
        setSpellPosition(go, target);
        GameObject goChild = go.transform.GetChild(0).gameObject;

        Vector2 casterPos = tilemap.CellToWorld(caster.position);
        casterPos.y += 0.2f;
        // Change position of actual spell
        goChild.transform.position = casterPos;
    }

    private static void setSpellPosition(GameObject go, Vector3Int target)
    {
        Spell spell = go.GetComponent<Spell>();
        spell.position = target;
    }
}
