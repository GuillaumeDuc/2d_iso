using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpellInstantiateList : MonoBehaviour
{
    static SpellInstantiateList instance;
    void Start()
    {
        instance = this;
    }

    public static void instantiateAreaWithDelay(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(target, caster, obstacleList, tilemap);

        instance.StartCoroutine(multipleInstantiateOnCell(listCells, caster, spell, obstacleList, tilemap));
    }

    static IEnumerator multipleInstantiateOnCell(
        List<Vector3Int> listCells,
        Unit caster,
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        foreach (var c in listCells)
        {
            yield return new WaitForSeconds(0.1f);
            instantiateOnCellClicked(spell, caster, c, obstacleList, tilemap);
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
        string paramName = "Attack";
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                animator.SetTrigger(paramName);
            }
        }
    }

    private static void setSpellPosition(GameObject go, Vector3Int target)
    {
        Spell spell = go.GetComponent<Spell>();
        spell.position = target;
    }
}
