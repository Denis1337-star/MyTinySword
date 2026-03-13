using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableEntity
{
    void OnSelected(SelectionSystem selectionSystem);
    void OnDeselected();
}
