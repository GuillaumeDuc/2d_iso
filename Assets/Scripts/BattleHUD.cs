using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text movementPointText;
    public Slider hpSlider;
    public Slider manaSlider;

    public void setHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        manaSlider.maxValue = unit.mana;
        manaSlider.value = unit.currentMana;
        movementPointText.text = unit.currentMovementPoint + "";
    }

    public void setHP(int hp)
    {
        hpSlider.value = hp;
    }

    public void setMana(int mana)
    {
        manaSlider.value = mana;
    }

    public void setMovementPoint(int mp)
    {
        movementPointText.text = mp + "";
    }
}
