using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public static class FirebaseReady
{
    private static Task _initTask;
    private static bool _isReady;

    public static bool IsReady => _isReady;

    public static Task Ensure()
    {
        // Already finished successfully
        if (_isReady)
            return Task.CompletedTask;

        // Already in progress – return the same task
        if (_initTask != null)
            return _initTask;

        Debug.Log("[FirebaseReady] Starting dependency check...");

        var tcs = new TaskCompletionSource<bool>();

        _initTask = FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("[FirebaseReady] CheckAndFixDependenciesAsync faulted: " + task.Exception);
                    tcs.SetException(task.Exception ?? new Exception("Firebase dependency check failed"));
                    return;
                }

                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    Debug.Log("[FirebaseReady] Firebase dependencies are available.");
                    _isReady = true;
                    tcs.SetResult(true);
                }
                else
                {
                    var ex = new Exception($"Could not resolve all Firebase dependencies: {status}");
                    Debug.LogError("[FirebaseReady] " + ex);
                    tcs.SetException(ex);
                }
            });

        return tcs.Task;
    }
}
