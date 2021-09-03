using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpellSlot
{
    public GameObject spellSlotUI, spell;
    public SpellSlot(GameObject spellSlotUI)
    {
        this.spellSlotUI = spellSlotUI;
    }
}

public class SelectionSpell : MonoBehaviour
{
    public GameObject contentContainer;
    public GameObject SpellButtonPrefab;

    public GameObject FirstSpellSlotUI, SecondSpellSlotUI, ThirdSpellSlotUI;

    private SpellSlot FirstSpellSlot, SecondSpellSlot, ThirdSpellSlot;
    private SpellSlot selectedSpellSlot;

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
        selectedSpellSlot.spellSlotUI.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
        selectedSpellSlot.spellSlotUI.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = spell.GetComponent<SpriteRenderer>().sprite;
        // Set selected spell
        selectedSpellSlot.spell = spell;
        // Check for third spell
        if (FirstSpellSlot.spell != null && SecondSpellSlot.spell != null)
        {
            ThirdSpellSlot.spell = MixSpell.mix(FirstSpellSlot.spell.GetComponent<Spell>(), SecondSpellSlot.spell.GetComponent<Spell>());
            if (ThirdSpellSlot.spell != null)
            {
                ThirdSpellSlot.spellSlotUI.transform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
                ThirdSpellSlot.spellSlotUI.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = ThirdSpellSlot.spell.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    void initializeSpellSlot()
    {
        // Initialize spell slot
        FirstSpellSlot = new SpellSlot(FirstSpellSlotUI);
        SecondSpellSlot = new SpellSlot(SecondSpellSlotUI);
        ThirdSpellSlot = new SpellSlot(ThirdSpellSlotUI);
        // Add on click listener
        addListenerSpellSlot(FirstSpellSlot);
        addListenerSpellSlot(SecondSpellSlot);
    }

    void addListenerSpellSlot(SpellSlot spellSlot)
    {
        Button buttonFirst = spellSlot.spellSlotUI.GetComponent<Button>();
        buttonFirst.onClick.AddListener(() => { setSelectedSpellSlot(spellSlot); });
    }

    void setSelectedSpellSlot(SpellSlot spellSlot)
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
}
