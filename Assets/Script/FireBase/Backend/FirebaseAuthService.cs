using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public sealed class FirebaseAuthService : IAuthService
{
    private FirebaseAuth _auth;
    public FirebaseAuthService(FirebaseAuth auth) { _auth = auth; }

    public string CurrentUserId => _auth?.CurrentUser?.UserId;
    public string CurrentEmail  => _auth?.CurrentUser?.Email;

    public async Task<(bool ok, string err)> RegisterAsync(string email, string password)
    {
        try
        {
            await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            return (true, null);
        }
        catch (FirebaseException fex)
        {
            switch (fex.ErrorCode)
            {
                case (int) AuthError.EmailAlreadyInUse:
                    return (false, "This email is already registered.");
                case (int) AuthError.InvalidEmail:
                    return (false, "Please enter a valid email.");
                case (int) AuthError.WeakPassword:
                    return (false, "Password is too weak (min 6 chars).");
                default:
                    return (false, $"[{fex.ErrorCode}] {fex.Message}");
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool ok, string err)> LoginAsync(string email, string password)
    {
        try { await _auth.SignInWithEmailAndPasswordAsync(email, password); return (true, null); }
        catch (Exception ex) { return (false, ex.Message); }
    }

    public Task<(bool ok, string err)> SendPasswordResetAsync(string email)
        => Try(async () => await _auth.SendPasswordResetEmailAsync(email));

    public void SignOut() => _auth?.SignOut();

    private static async Task<(bool ok, string err)> Try(Func<Task> TaskFactory)
    {
        try { await TaskFactory(); return (true, null); }
        catch (Exception ex) { return (false, ex.Message); }
    }
}

public sealed class FirestoreUserProfileRepository : IUserProfileRepository
{
    private readonly FirebaseFirestore _db;
    public FirestoreUserProfileRepository(FirebaseFirestore db) { _db = db; }

    DocumentReference Doc(string uid) => _db.Collection("users").Document(uid);

    public async Task EnsureProfileAsync(string uid, string email, string displayName, string phone)
    {
        var snap = await Doc(uid).GetSnapshotAsync();
        if (!snap.Exists)
        {
            await Doc(uid).SetAsync(new {
                uid, displayName, email, phone,
                role="user", createdAt=FieldValue.ServerTimestamp, updatedAt=FieldValue.ServerTimestamp
            }, SetOptions.MergeAll);
        }
    }

    public Task SaveAsync(string uid, UserProfile p)
    {
        var up = new Dictionary<string, object>{
            {"displayName", p.displayName ?? ""}, {"phone", p.phone ?? ""}, {"career", p.career ?? ""},
            {"updatedAt", FieldValue.ServerTimestamp}
        };
        if (p.age.HasValue) up["age"]=p.age.Value;
        if (p.weightKg.HasValue) up["weightKg"]=p.weightKg.Value;
        if (p.heightCm.HasValue) up["heightCm"]=p.heightCm.Value;
        if (p.bloodGlucoseMgDl.HasValue) up["bloodGlucoseMgDl"]=p.bloodGlucoseMgDl.Value;
        return Doc(uid).SetAsync(up, SetOptions.MergeAll);
    }

    public async Task<UserProfile> GetAsync(string uid)
    {
        var s = await Doc(uid).GetSnapshotAsync();
        if (!s.Exists) return null;
        var p = new UserProfile();
        p.uid = uid;
        if (s.ContainsField("displayName")) p.displayName = s.GetValue<string>("displayName");
        if (s.ContainsField("phone"))       p.phone       = s.GetValue<string>("phone");
        if (s.ContainsField("career"))      p.career      = s.GetValue<string>("career");
        if (s.ContainsField("age"))         p.age         = (int)(long)s.GetValue<long>("age");
        if (s.ContainsField("weightKg"))    p.weightKg    = s.GetValue<double>("weightKg");
        if (s.ContainsField("heightCm"))    p.heightCm    = s.GetValue<double>("heightCm");
        if (s.ContainsField("bloodGlucoseMgDl")) p.bloodGlucoseMgDl = s.GetValue<double>("bloodGlucoseMgDl");
        return p;
    }
}
