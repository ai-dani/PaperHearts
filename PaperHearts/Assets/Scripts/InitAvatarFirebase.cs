using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InitAvatarFirebase : MonoBehaviour
{
    [Header("UI Stuff")]
    public Button startButton;
    public GameObject loadPanel;

    [Header("Temporary URLs of Avatar Parts")]
    public string[] tempTorsoUrl;

    private Customization custom;

    private bool finishRetrieval, startSession;
    private FirebaseDatabase _database;
    

    // Start is called before the first frame update
    void Start()
    {
        custom=GetComponent<Customization>();
        finishRetrieval = false;  startSession = false;
        _database = FirebaseDatabase.DefaultInstance;
        RetrieveAvatarPartsData();

        startButton.onClick.AddListener(PopulateAvatarListsOnStart);
    }

    //parts = hair, torso, bottom, etc.
    public void RetrieveAvatarPartsData(){
        FirebaseDatabase.DefaultInstance.GetReference("avatar").GetValueAsync().ContinueWith(task =>
        {

            if(task.IsFaulted){
                Debug.Log("Error, cannot retrieve avatar customizations");
            }
            else if(task.IsCompleted){
                DataSnapshot snapshot = task.Result;


                //=======STEPS TOWARDS RETRIEVAL==========
                //1) count total children of an avatar part 
                //2) retrieve json value of children via index
                //3) store image file url within temporary array (discarded later)
                //==========================================

                //Torso
                long torsoCount = snapshot.Child("torso").ChildrenCount;
                tempTorsoUrl = new string[torsoCount];
                for(int i = 0; i < torsoCount; i++){
                    string imageUrl = snapshot.Child("torso").Child(i.ToString()).GetRawJsonValue();
                    imageUrl = imageUrl.Trim("\"".ToCharArray());
                    tempTorsoUrl[i] = imageUrl;
                }
                
                //Pants

                finishRetrieval = true; 
            }
        });
    }

    void PopulateAvatarListsOnStart(){

        if(startSession){ //in case, we return to the homescreen again, don't populate again
            return;
        }
        
        while(!finishRetrieval && !custom.finishCreatingList){ //wait state

        }

        foreach(string imageUrl in tempTorsoUrl)
                StartCoroutine(DownloadImage(imageUrl, custom.torsoList));

         startSession = true;
         loadPanel.SetActive(false);
    }

    IEnumerator DownloadImage(string MediaUrl, List<Texture> articleList)
    {  
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError) 
            Debug.Log(request.error);
        else{
            Debug.Log("Downloading... " + MediaUrl);
            articleList.Add(((DownloadHandlerTexture) request.downloadHandler).texture);
        }    
    } 


}

