using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CastResult
{
    public enum Type { Safe = 0, Danger, Treasure }
    public class Miss : CastResult {} 
    public record Info(int colliderID, Type type);
    public record Hit(Info info, Vector2 point) : CastResult;
    public record Collection(Info info, List<Vector2> points);
    public bool IsNear(CastResult other, float thresh) {
        return this is Hit th && other is Hit oh && th.info == oh.info && Vector2.Distance(th.point, oh.point) < thresh;
    }
    public Info GetInfo() {
        return this is Hit th ? th.info : null;
    }
}

namespace System.Runtime.CompilerServices
{
      internal static class IsExternalInit {}
}
