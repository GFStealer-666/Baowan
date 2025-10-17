using System;
using Firebase.Firestore;


[Serializable]
public class UserProfile
{
    public string uid;
    public string displayName;
    public string email;
    public string phone;
    public int? age;
    public double? weightKg;
    public double? heightCm;
    public string career;
    public double? bloodGlucoseMgDl;
    public string role;
    public Timestamp createdAt;
    public Timestamp updatedAt;
}
