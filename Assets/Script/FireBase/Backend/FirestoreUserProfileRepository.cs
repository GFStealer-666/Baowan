using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;

public sealed class FirestoreUserProfileRepository : IUserProfileRepository
{
    private readonly FirebaseFirestore _db;

    public FirestoreUserProfileRepository(FirebaseFirestore db)
    {
        _db = db;
    }

    DocumentReference Doc(string uid) =>
        _db.Collection("users").Document(uid);

    public async Task EnsureProfileAsync(string uid, string email, string displayName, string phone)
    {
        var snap = await Doc(uid).GetSnapshotAsync();
        if (!snap.Exists)
        {
            await Doc(uid).SetAsync(new {
                uid,
                displayName,
                email,
                phone,
                role = "user",
                createdAt = FieldValue.ServerTimestamp,
                updatedAt = FieldValue.ServerTimestamp
            }, SetOptions.MergeAll);
        }
    }

    public Task SaveAsync(string uid, UserProfile p)
    {
        var data = new Dictionary<string, object>
        {
            { "displayName", p.displayName ?? "" },
            { "phone",       p.phone ?? "" },
            { "career",      p.career ?? "" },
            { "updatedAt",   FieldValue.ServerTimestamp }
        };

        if (p.age.HasValue)             data["age"] = p.age.Value;
        if (p.weightKg.HasValue)        data["weightKg"] = p.weightKg.Value;
        if (p.heightCm.HasValue)        data["heightCm"] = p.heightCm.Value;
        if (p.bloodGlucoseMgDl.HasValue)data["bloodGlucoseMgDl"] = p.bloodGlucoseMgDl.Value;

        return Doc(uid).SetAsync(data, SetOptions.MergeAll);
    }

    public async Task<UserProfile> GetAsync(string uid)
    {
        var s = await Doc(uid).GetSnapshotAsync();
        if (!s.Exists) return null;

        var p = new UserProfile();
        p.uid = uid;

        if (s.ContainsField("displayName"))       p.displayName = s.GetValue<string>("displayName");
        if (s.ContainsField("phone"))             p.phone       = s.GetValue<string>("phone");
        if (s.ContainsField("career"))            p.career      = s.GetValue<string>("career");
        if (s.ContainsField("age"))               p.age         = (int)(long)s.GetValue<long>("age");
        if (s.ContainsField("weightKg"))          p.weightKg    = s.GetValue<double>("weightKg");
        if (s.ContainsField("heightCm"))          p.heightCm    = s.GetValue<double>("heightCm");
        if (s.ContainsField("bloodGlucoseMgDl"))  p.bloodGlucoseMgDl = s.GetValue<double>("bloodGlucoseMgDl");

        return p;
    }
}
