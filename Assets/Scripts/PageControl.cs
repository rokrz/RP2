using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PageControl : MonoBehaviour
{
    PageSwiper pg;
    public void setPG(PageSwiper pg)
    {
        this.pg = pg;
    }
    // Start is called before the first frame update
    public void ControlaPage()
    {
        Debug.Log("caralineos voadores");
        if(pg == null)
        {
            pg = GameObject.FindObjectOfType<LevelSelector>().swiper;
        }
        if (this.name=="true")
        {
            pg.PageSwipe(true);
        }
        else{
            pg.PageSwipe(false);
        }
    }


}
