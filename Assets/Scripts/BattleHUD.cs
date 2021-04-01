using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Slider hpSlider;

    public void setHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }

    public void setHP(int hp)
    {
        hpSlider.value = hp;
    }
 }
