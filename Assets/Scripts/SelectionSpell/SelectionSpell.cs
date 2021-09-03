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
            // RectTransform rt = newSpellButton.GetComponent<RectTransform>();
            // rt.sizeDelta = new Vector2(300, 100);
        });
    }

    void onClickSpell(GameObject spell)
    {
        // Change picture
        selectedSpellSlot.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
        selectedSpellSlot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = spell.GetComponent<SpriteRenderer>().sprite;
    }

    void initializeSpellSlot()
    {
        addListenerSpellSlot(FirstSpellSlot);
        addListenerSpellSlot(SecondSpellSlot);
    }

    void addListenerSpellSlot(GameObject spellSlot)
    {
        Button buttonFirst = spellSlot.GetComponent<Button>();
        buttonFirst.onClick.AddListener(() => { setSelectedSpellSlot(spellSlot); });
    }

    void setSelectedSpellSlot(GameObject spellSlot)
    {
        selectedSpellSlot = spellSlot;
    }

    void Start()
    {
        // Set spell list
        setSpells();
        // Initialize spell slots
        initializeSpellSlot();
        // Selected spell slot is First spell slot by default
        setSelectedSpellSlot(FirstSpellSlot);
    }

    void Update()
    {

    }
}
