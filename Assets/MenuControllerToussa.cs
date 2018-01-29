using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerToussa : MonoBehaviour
{
    private float timer;
    public int step = 0;
    public bool clicked = false;

    void Start()
    {
        AkSoundEngine.SetState("menuState", "menuIn");
        AkSoundEngine.PostEvent("Play_menuMusicSwitch", gameObject);
        timer = 2.625f;
        step = 0;
    }

    public void OnClick() {
        Debug.Log("Start game !");
        clicked = true;
        AkSoundEngine.SetState("menuState", "menuOut");
    }

    public void OnClickSpecial()
    {
        AkSoundEngine.SetState("menuState", "menuCredit");
    }

    public void OnClickBack()
    {
        AkSoundEngine.SetState("menuState", "menuIn");
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (clicked && step == 1)
            {
                AkSoundEngine.PostEvent("Play_ocean", gameObject);
                Debug.Log("GO !");
                step = 4;
            }
            else
            {
                switch (step)
                {
                    case 0:
                        timer += 5.345f;
                        step = 1;
                        break;
                    case 1:
                        AkSoundEngine.SetState("menuState", "menuIn");
                        timer += 7.53f - 5.345f;
                        step = 2;
                        break;
                    case 2:
                        timer += 5.345f;
                        step = 1;
                        break;
                }
            }
        }
    }
}
