using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectionSpell : MonoBehaviour
{
    public GameObject contentContainer;
    public GameObject SpellButtonPrefab;

    public GameObject FirstSpellSlot, SecondSpellSlot, ThirdSpellSlot;

    private GameObject selectedSpellSlot;

    void setSpells()
    {
        SpellList.getSpellList().ForEach(spell =>
        {
            // Instantiate button
            GameObject newSpellButton = Instantiate(SpellButtonPrefab);
            // Get GameObject and set sprite
            GameObject spellPicture = newSpellButton.transform.GetChild(0).gameObject;
            Image ImageSP = spellPicture.GetComponent<Image>();
            ImageSP.sprite = spell.GetComponent<SpriteRenderer>().sprite;
            // Add onClick Effect
            Button buttonComponent = newSpellButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => { onClickSpell(spell); });
            // Put button in scrollviewer
            newSpellButton.transform.SetParent(contentContainer.transform, false);
            RectTransform rt = newSpellButton.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 100);
        });
    }

    void onClickSpell(GameObject spell)
    {
        selectedSpellSlot = spell;
    }

    void Start()
    {
        setSpells();
    }

    void Update()
    {

    }
}
