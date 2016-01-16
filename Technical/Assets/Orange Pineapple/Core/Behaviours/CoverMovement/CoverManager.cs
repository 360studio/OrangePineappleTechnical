using UnityEngine;
using System.Collections;
using Lockstep; 
using System.Collections.Generic;
public static class CoverManager {
    private static FastBucket<Cover> Covers = new FastBucket<Cover> ();
    internal static uint _Version {get; private set;}

    public static void Initialize () {
        _Version++;
        Covers.Clear();
    }
    public static void Simulate () {

    }
    public static void Deactivate () {

    }

    public static void Create () {

    }
    public static Cover FindCover (IEnumerable<LSBody> checkBodies, Cover ignoreCov) {
        foreach (Cover cover in FindCovers (checkBodies,ignoreCov)) {
            return cover;
        }
        return null;
    }
    public static IEnumerable<Cover> FindCovers (IEnumerable<LSBody> checkBodies, Cover ignoreCov) {
        foreach (LSBody body in checkBodies) {
            Cover cover = body.GetComponent<Cover> ();
            if (body.GetComponent<Cover>() == null) {
                break;
            }
            if (cover == ignoreCov) {
                continue;
            }
            if (!IsActive (cover)) {
                continue;
            }
            yield return cover;
        }

    }

    public static bool IsActive (Cover cover) {
        return cover.Version == _Version;
    }

    public static Cover GetCover (ushort id) {
        return Covers[id];
    }

    public static void Assimilate (Cover cover) {
        cover.ID = (ushort)Covers.Add(cover);
        cover.Version = _Version;
    }
    public static void Dessimilate (Cover cover) {
        Covers.RemoveAt(cover.ID);
        cover.Version--;
    }
}
