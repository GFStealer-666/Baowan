using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await Services.InitAsync();
        Debug.Log("Services ready");
    }
}
