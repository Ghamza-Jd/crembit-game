using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This controller controls the UI Behaviour
/// </summary>
public class MainMenu : MonoBehaviour
{
    public Sprite playingSprite;
    public Sprite muteSprite;
    public Image musicImage;

    public TMP_Dropdown hairColor;
    public TMP_Dropdown tintColor;
    public TMP_Dropdown shirtColor;
    public TMP_Dropdown pantsColor;

    public GameObject chooseLevelPanel;
    public GameObject creditsPanel;
    public GameObject stylePanel;
    public GameObject htpPanel;

    public GameObject slides;

    public GameObject apiHandler;

    public void Play()
    {
        chooseLevelPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        apiHandler.GetComponent<APIHandler>().SendRequest();
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void HowToPlay()
    {
        htpPanel.SetActive(true);
    }

    public void NextHowToPlay()
    {
        slides.transform.GetChild(0).gameObject.SetActive(false);
        slides.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void PrevHowToPlay()
    {
        slides.transform.GetChild(0).gameObject.SetActive(true);
        slides.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void CloseHowToPlay()
    {
        PrevHowToPlay();
        htpPanel.SetActive(false);
    }

    public void Credits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleBackgroundMusic()
    {
        var music = GameObject.Find("Music");
        var isPlaying = music.GetComponent<AudioSource>().mute = !music.GetComponent<AudioSource>().mute;
        musicImage.sprite = isPlaying ? muteSprite : playingSprite;
    }

    public void OpenStyleSheet()
    {
        stylePanel.SetActive(true);
    }

    public void CloseStyleSheet()
    {
        stylePanel.SetActive(false);
    }

    public void OnChangeHairColor()
    {
        Style.StyleColors["hair"] = Style.HairColors[hairColor.options[hairColor.value].text];
        try
        {
            GameObject.Find("/humanoid/head/hair").GetComponent<SpriteRenderer>().color = Style.StyleColors["hair"];
        }
        finally
        {
            hairColor.Hide();
        }
    }

    public void OnChangeTintColor()
    {
        Style.StyleColors["tint"] = Style.TintColors[tintColor.options[tintColor.value].text];
        var objs = new[]
        {
         "/humanoid/head/head",
         "/humanoid/head/nose",
         "/humanoid/head/neck",
         "/humanoid/body/ubody/ubody",
         "/humanoid/body/ubody/arms/leftArm",
         "/humanoid/body/ubody/arms/rightArm",
         "/humanoid/body/ubody/hands/leftHand",
         "/humanoid/body/ubody/hands/rightHand",
         "/humanoid/body/lbody/lbody",
         "/humanoid/body/lbody/legs/leftLeg",
         "/humanoid/body/lbody/legs/rightLeg"
      };
        try
        {
            foreach (var obj in objs)
            {
                GameObject.Find(obj).GetComponent<SpriteRenderer>().color = Style.StyleColors["tint"];
            }
        }
        finally
        {
            tintColor.Hide();
        }
    }

    public void OnChangeShirtColor()
    {
        Style.StyleColors["shirt"] = Style.DefaultColors[shirtColor.options[shirtColor.value].text];
        var objs = new[]
        {
         "/humanoid/body/ubody/shirt",
         "/humanoid/body/ubody/shirt/rightArmT",
         "/humanoid/body/ubody/shirt/leftArmT"
      };
        try
        {
            foreach (var obj in objs)
            {
                GameObject.Find(obj).GetComponent<SpriteRenderer>().color =
                   Style.StyleColors["shirt"];
            }
        }
        finally
        {
            shirtColor.Hide();
        }
    }

    public void OnChangePantsColor()
    {
        Style.StyleColors["pants"] = Style.DefaultColors[pantsColor.options[pantsColor.value].text];
        var objs = new[]
        {
         "/humanoid/body/lbody/pants/middle",
         "/humanoid/body/lbody/pants/pantsRight",
         "/humanoid/body/lbody/pants/pantsLeft"
      };
        try
        {
            foreach (var obj in objs)
            {
                GameObject.Find(obj).GetComponent<SpriteRenderer>().color =
                   Style.StyleColors["pants"];
            }
        }
        finally
        {
            pantsColor.Hide();
        }
    }


    public void OpenLevelOne()
    {
        SceneManager.LoadScene("Level 1");
        PlayerData.level = 2;
    }

    public void OpenLevelTwo()
    {
        SceneManager.LoadScene("Level");
        PlayerData.level = 1;
    }

    public void CloseChooseLevelPanel()
    {
        chooseLevelPanel.SetActive(false);
    }

    public void TakeScreenShot()
    {
        StartCoroutine(CaptureCo());
    }

    private static IEnumerator CaptureCo()
    {
        var timestamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        var filename = "ScreenShot" + timestamp + ".png";
        ScreenCapture.CaptureScreenshot(filename);
        yield return new WaitForEndOfFrame();
    }
}
