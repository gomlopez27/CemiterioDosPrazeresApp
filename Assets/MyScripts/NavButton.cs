using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class NavButton : MonoBehaviour, /*IPointerEnterHandler,*/ IPointerClickHandler, IPointerExitHandler
{
    public Navigation navigation;

    public Image background;

    //public Sprite activeIcon;
    //public Sprite inactiveIcon;


    //public UnityEvent onButtonSeleted;
    //public UnityEvent onButtonDeseleted;


    public void OnPointerClick(PointerEventData eventData)
    {
        navigation.OnNavSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        navigation.OnNavExit(this);
    }

    //public void Select()
    //{
    //    if (onButtonSeleted != null)
    //        onButtonSeleted.Invoke();

    //}

    //public void Deselect()
    //{
    //    if (onButtonDeseleted != null)
    //        onButtonDeseleted.Invoke();
    //}

    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        navigation.Subscribe(this);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
