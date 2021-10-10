using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpellEffect : MonoBehaviour
{
    public status status;
    private Unit unit;
    private Status selectedStatus;
    void Start()
    {
        unit = gameObject.GetComponentInParent<Unit>();
        selectedStatus = StatusList.getStatuses().Find(s => s.name == status.ToString());
    }

    void Update()
    {
        if (!unit.statusList.Contains(selectedStatus))
        {
            Destroy(gameObject);
        }
    }
}
