using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MixSpell
{
    public static GameObject mix(Spell firstSpell, Spell secondSpell)
    {
        // Mix Fire with Fire
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.SuperFire);
        }
        // Mix Fire with Earth
        if (firstSpell.type == SpellType.Fire && secondSpell.type == SpellType.Earth || firstSpell.type == SpellType.Earth && secondSpell.type == SpellType.Fire)
        {
            return SpellList.getMixedSpell(SpellType.Magma);
        }
        // None
        return null;
    }
}
