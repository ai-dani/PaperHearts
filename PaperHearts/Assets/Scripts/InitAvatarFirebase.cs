using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

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

    [Header("Authentication")]
    public string key;
    User user;
    

    // Start is called before the first frame update
    void Start()
    {
        
        custom=GetComponent<Customization>();
        finishRetrieval = false;  
        startSession = false;
        _database = FirebaseDatabase.DefaultInstance;
        RetrieveAvatarPartsData(); // retrieves general retrieval of avatar database
        startButton.onClick.AddListener(PopulateAvatarListsOnStart);
        RetrieveUserData();
        user = FindObjectOfType<User>();
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

    // Populates general customization of avatar
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

//=============================== USER DATA MANAGEMENT ==========================================

    private void RetrieveUserData(){
        string potentialKey = ReturnUserKeyIfExists();
        string newUserKey = ""; // this variable is only used for new users registered devices

        // if empty generate a key to put into database
        if(potentialKey.Equals("")){
            newUserKey = RandomUserKeyGenerator(16);
            Debug.Log("random key: " + newUserKey);
            PlayerPrefs.SetString("userkey", newUserKey);
            Debug.Log("get userkey from player pref:" + PlayerPrefs.GetString("userkey"));
        }

        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => //LEARNED: YOU CANNOT ACCESS PLAYERPREF INSIDE FIREBASE INSTANCE!!!!
        {

            if(task.IsFaulted){
            Debug.Log("Error, cannot retrieve user data");
            }
            else if(task.IsCompleted){ //successfully retrieve data
                DataSnapshot snapshot = task.Result;

                //if the key returned is blank, this device is a new device / new user
                if(potentialKey.Equals("")){
                    SaveUserDeviceData(newUserKey);
                } 
                
                //***IMPORTANT: USER MUST NOT DELETE PLAYER PREF KEY OR AVATAR DATA WILL BE DELETED***
                else{ //else there is already an existing user data for this device 

                    //populate user instance class
                    string name = snapshot.Child(potentialKey).Child("name").Value.ToString(); //path to key
                    Debug.Log("This is " + name + " from users firebase");
                }
            }
        });


    }
    
    public void SaveUserDeviceData(string generatedKey){
        string json = JsonUtility.ToJson(user);
        _database.RootReference.Child("users").Child(generatedKey).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if(task.IsCompleted){
                Debug.Log("successfully added user data to firebase");
            }
            else{
                Debug.Log("not successful");
            }
        }
        );
        
        
    }

    public string ReturnUserKeyIfExists(){
        string userKey = "";
        // if it does exist/stored in playerprefs
        // retrieve the player's "#####" key
        if(PlayerPrefs.HasKey("userkey")){
            userKey = PlayerPrefs.GetString("userkey"); 
        }
        return userKey;
    }

    public static string RandomUserKeyGenerator(int lengthNeeded){

        StringBuilder userKey = new StringBuilder("");
        char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        char singleChar;

        while(userKey.Length < lengthNeeded){
            singleChar = chars[UnityEngine.Random.Range(0, chars.Length)];
            userKey.Append(singleChar);
        }

        return userKey.ToString();
    }
}

