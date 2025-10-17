using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;

// Checking User stuff
public class UserProfileService : MonoBehaviour
{
    public static UserProfileService Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    static async Task WaitForAuthReady(FirebaseUser expectedUser, int timeoutMs = 5000)
    {
        // Ensure CurrentUser is set and token available
        var start = Time.realtimeSinceStartup;
        while (FirebaseAuth.DefaultInstance.CurrentUser?.UserId != expectedUser.UserId)
        {
            await System.Threading.Tasks.Task.Yield();
            if ((Time.realtimeSinceStartup - start) * 1000 > timeoutMs)
                throw new System.TimeoutException("Auth user not ready.");
        }
        // Force-get an ID token (ensures Firestore has credentials)
        await expectedUser.TokenAsync(false);
    }
    private static async Task<FirebaseFirestore> GetDbAsync()
    {
        await FirebaseReady.Ensure();
        return FirebaseFirestore.DefaultInstance;
    }

    private static async Task<DocumentReference> DocAsync(string uid)
    {
        var db = await GetDbAsync();
        return db.Collection("users").Document(uid);
    }

    public async Task EnsureProfile(FirebaseUser user, string displayName = null, string phone = null)
    {
        if (user == null) throw new InvalidOperationException("EnsureProfile: user is null");

        // get ready Firestore each time (avoids Start() race)
        await FirebaseReady.Ensure();
        var db  = FirebaseFirestore.DefaultInstance;
        var doc = db.Collection("users").Document(user.UserId);

        var snap = await doc.GetSnapshotAsync();
        if (snap.Exists) return;

        var data = new Dictionary<string, object>
        {
            { "uid", user.UserId },                                // required by your rules
            { "displayName", string.IsNullOrWhiteSpace(displayName) ? "Player" : displayName },
            { "email", user.Email ?? "" },
            { "phone", phone ?? "" },
            { "role", "user" },
            { "createdAt", FieldValue.ServerTimestamp },
            { "updatedAt", FieldValue.ServerTimestamp }
        };
        await doc.SetAsync(data, SetOptions.MergeAll);
        Debug.Log("[Profile] Created /users/" + user.UserId);
    }

    public async Task SaveProfileFields(string uid, UserProfile p)
    {
        var db = await GetDbAsync();
        await db.Collection("users").Document(uid).SetAsync(new Dictionary<string, object> {
            { "displayName", p.displayName ?? "" },
            { "phone", p.phone ?? "" },
            { "career", p.career ?? "" },
            { "updatedAt", FieldValue.ServerTimestamp },
            // add optional fields only if set:
            // ...
        }, SetOptions.MergeAll);
    }

    public async Task<UserProfile> GetProfile(string uid)
    {
        var doc = await DocAsync(uid);
        var s = await doc.GetSnapshotAsync();
        if (!s.Exists) return null;

        var p = new UserProfile();
        p.uid = s.ContainsField("uid") ? s.GetValue<string>("uid") : uid;
        p.displayName = s.ContainsField("displayName") ? s.GetValue<string>("displayName") : "";
        p.email = s.ContainsField("email") ? s.GetValue<string>("email") : "";
        p.phone = s.ContainsField("phone") ? s.GetValue<string>("phone") : "";
        p.career = s.ContainsField("career") ? s.GetValue<string>("career") : "";
        p.role = s.ContainsField("role") ? s.GetValue<string>("role") : "user";
        if (s.ContainsField("age")) p.age = (int?)(long)s.GetValue<long>("age");
        if (s.ContainsField("weightKg")) p.weightKg = s.GetValue<double>("weightKg");
        if (s.ContainsField("heightCm")) p.heightCm = s.GetValue<double>("heightCm");
        if (s.ContainsField("bloodGlucoseMgDl")) p.bloodGlucoseMgDl = s.GetValue<double>("bloodGlucoseMgDl");
        if (s.ContainsField("createdAt")) p.createdAt = s.GetValue<Timestamp>("createdAt");
        if (s.ContainsField("updatedAt")) p.updatedAt = s.GetValue<Timestamp>("updatedAt");
        return p;
    }

    

    public static async Task<bool> TryClaimDisplayName(string displayName)
    {
        await FirebaseReady.Ensure();
        var db = FirebaseFirestore.DefaultInstance;
        var uid = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrWhiteSpace(uid)) throw new InvalidOperationException("No signed-in user.");

        var handle = displayName.Trim().ToLowerInvariant();
        var handleRef = db.Collection("displayNames").Document(handle);
        var userRef = db.Collection("users").Document(uid);

        try
        {
            await db.RunTransactionAsync(async tx =>
            {
                var hSnap = await tx.GetSnapshotAsync(handleRef);
                if (hSnap.Exists)
                    throw new InvalidOperationException("HANDLE_TAKEN");

                // Reserve the handle
                tx.Set(handleRef, new Dictionary<string, object> {
                    { "uid", uid },
                    { "createdAt", FieldValue.ServerTimestamp }
                });

                // Update user doc (merge); uid already present from EnsureProfile
                tx.Set(userRef, new Dictionary<string, object> {
                    { "displayName", displayName },
                    { "updatedAt", FieldValue.ServerTimestamp }
                }, SetOptions.MergeAll);
            });

            return true;
        }
        catch (InvalidOperationException ex) when (ex.Message == "HANDLE_TAKEN")
        {
            return false;
        }
    }

    public static async Task<bool> IsDisplayNameTaken(string displayName)
    {
        await FirebaseReady.Ensure();
        var db = FirebaseFirestore.DefaultInstance;
        var handle = displayName.Trim().ToLowerInvariant();
        var snap = await db.Collection("displayNames").Document(handle).GetSnapshotAsync();
        return snap.Exists; // true => taken
    }
}

public static class FirebaseReady
{
    private static System.Threading.Tasks.Task _t;
    public static System.Threading.Tasks.Task Ensure() => _t ??= Initialize();
    private static async System.Threading.Tasks.Task Initialize()
    {
        var status = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != Firebase.DependencyStatus.Available)
            throw new System.Exception("Firebase deps not available: " + status);
    }
}
