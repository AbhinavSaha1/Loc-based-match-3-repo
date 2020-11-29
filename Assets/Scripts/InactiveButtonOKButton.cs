using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactiveButtonOKButton : MonoBehaviour
{
    public void OK()
    {
        this.gameObject.SetActive(false);
    }
}
