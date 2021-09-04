using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoScrollView : MonoBehaviour
{
    public GameObject ContentContainer;

    public GameObject InfoPrefab;

    private Dictionary<Unit, BattleHUD> HUDList = new Dictionary<Unit, BattleHUD>();

    public void addInfo(Unit unit)
    {
        // Instantiate Info
        GameObject unitInfo = Instantiate(InfoPrefab);
        BattleHUD enemyHUD = unitInfo.GetComponent<BattleHUD>();
        enemyHUD.setHUD(unit);
        // Put InfoScrollView in content
        unitInfo.transform.SetParent(ContentContainer.transform, false);
        // Put enemy in list
        HUDList.Add(unit, enemyHUD);
    }

    public void updateScrollView()
    {
        foreach (var h in HUDList)
        {
            h.Value.setHP(h.Key.currentHP);
            h.Value.setMana(h.Key.currentMana);
            h.Value.setMovementPoint(h.Key.currentMovementPoint);
        }
    }
}
