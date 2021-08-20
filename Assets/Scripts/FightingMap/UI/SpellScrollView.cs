using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine;

public class SpellScrollView : MonoBehaviour
{
    public GameObject contentContainer;

    public GameObject SpellButtonPrefab;

    public TurnBasedSystem turnBasedSystem;

    public void addSpell(GameObject spell, Unit unit)
    {
        // Instantiate button
        GameObject newSpellButton = Instantiate(SpellButtonPrefab);
        // Get GameObject and set sprite
        GameObject spellPicture = newSpellButton.transform.GetChild(0).gameObject;
        Image ImageSP = spellPicture.GetComponent<Image>();
        ImageSP.sprite = spell.GetComponent<SpriteRenderer>().sprite;
        // Add onClick Effect
        Button buttonComponent = newSpellButton.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => { onClickSpell(spell, unit); });
        // Put button in scrollviewer
        newSpellButton.transform.SetParent(contentContainer.transform, false);
    }

    void onClickSpell(GameObject spell, Unit unit)
    {
        turnBasedSystem.onClickSpell(spell, unit);
    }
}
