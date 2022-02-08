using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customization : MonoBehaviour
{
    public List<Texture> hairFrontList;
    public List<Texture> hairBackList;
    public List<Texture> eyeList;
    public List<Texture> torsoList;
    public List<Texture> pantsList;

    public bool finishCreatingList = false;

    // Start is called before the first frame update
    void Start()
    {
        hairFrontList = new List<Texture>();
        hairBackList = new List<Texture>();
        eyeList = new List<Texture>();
        torsoList = new List<Texture>();
        pantsList = new List<Texture>();
        
        finishCreatingList = true;
    }

}
