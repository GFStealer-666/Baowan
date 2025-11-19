using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("[FirebaseInitializer] SKIP Services.InitAsync TEST");
        // await Services.InitAsync();   // TEMPORARILY COMMENT THIS OUT
    }
}

