using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// User data
public class User : MonoBehaviour
{
    [SerializeField]
    string name; // player preference key

    [SerializeField]
    UserAvatar avatar;
    // [SerializeField]
    // string UserAvatar avatar;

}

// create a serializable subclass of user
[System.Serializable]
public class UserAvatar
{
    [SerializeField]
    string torsoURL;
}