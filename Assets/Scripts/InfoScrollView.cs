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
        // Put button in scrollviewer
        unitInfo.transform.SetParent(ContentContainer.transform, false);
        // Put enemy in list
        HUDList.Add(unit, enemyHUD);
    }

    public void setSliderHP(Unit unit)
    {
        BattleHUD unitHUD = HUDList[unit];
        unitHUD.setHP(unit.currentHP);
    }
}
