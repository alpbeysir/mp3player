using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongDisplayManager : MonoBehaviour
{
    [SerializeField] private Image bg;
    public void OnPointerEnter()
    {
        bg.color = new Color32(((Color32)bg.color).r, ((Color32)bg.color).g, ((Color32)bg.color).b, 120);
    }
    public void OnPointerExit()
    {
        bg.color = new Color32(((Color32)bg.color).r, ((Color32)bg.color).g, ((Color32)bg.color).b, 40);
    }
    public void OnPointerDown()
    {

    }
}
