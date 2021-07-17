using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpellInstantiateList : MonoBehaviour
{
    public void instantiateAreaWithDelay(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(caster, obstacleList, tilemap);

        StartCoroutine(multipleInstantiateOnCell(listCells, caster, spell, obstacleList, tilemap));
    }

    IEnumerator multipleInstantiateOnCell(
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

    public void instantiateOnCellClicked(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Vector2 worldPos = tilemap.CellToWorld(target);
        // Instantiate animation
        Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
    }

    public void instantiateObstacles(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(caster, obstacleList, tilemap);
        StartCoroutine(delayInstantiateObstacle(listCells, spell.gameObject, obstacleList, tilemap));
    }

    IEnumerator delayInstantiateObstacle(
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

    public void instantiateThrowedSpell(
        Spell spell,
        Unit caster,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Vector2 worldPos = tilemap.CellToWorld(target);
        GameObject go = Instantiate(spell.gameObject, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
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
}
