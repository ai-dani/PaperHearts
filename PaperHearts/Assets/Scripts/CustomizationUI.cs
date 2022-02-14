using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// holds all the buttons and events, and connects it to the customization list class
public class CustomizationUI : MonoBehaviour
{
    Customization custom;
    public TMP_Text customizeText;
    public GameObject avatarPanel;

    [Header("Torso")]
    public Button torsoButton;
    public RawImage torsoImage;
    private int torsoIterator;

    [Header("Easter Egg Only")]
    public Button badgeButton;
    public Image badgeImage;
    public int badgeClickCount; //easter egg only
    public GameObject easterEgg; //easter egg only
    
    // Start is called before the first frame update
    void Start()
    {
        custom = GetComponent<Customization>();
        torsoIterator = 0;
        torsoButton.onClick.AddListener(ChangeTorso);

        //easter egg only
        badgeButton.onClick.AddListener(ToggleBadge);
    }

    public void ChangeTorso(){
        //if texture matches with something in the list, and is not at the end of it -> change to next texture
        for(int i = 0; i < custom.torsoList.Count; i++){
            if(torsoIterator == i && i != custom.torsoList.Count-1){
                torsoImage.texture = custom.torsoList[i+1]; //switch to next
                torsoIterator++;
                break;
            }
            else if(torsoIterator == i && i == custom.torsoList.Count-1){
                torsoImage.texture = custom.torsoList[0];
                torsoIterator = 0;
                break;
            }
            // if(torsoImage.texture == custom.torsoList[i] && i != custom.torsoList.Count-1){
            //     torsoImage.texture = custom.torsoList[i+1];
            // }
            // else if(torsoImage.texture == custom.torsoList[i] && i==custom.torsoList.Count-1){
            //     torsoImage.texture = custom.torsoList[0]; //back to start of list
            // }
        }
    }

    public void ToggleBadge(){
        if(badgeImage.gameObject.activeInHierarchy){
            badgeImage.gameObject.SetActive(false);
            customizeText.text = "He loves me not";
        }
        else{
            badgeImage.gameObject.SetActive(true);
            customizeText.text = "He loves me";
        }

        badgeClickCount++;

        if(badgeClickCount == 10){
            easterEgg.SetActive(true);
            avatarPanel.SetActive(false);
            badgeClickCount = 0;
        }
    }
}
