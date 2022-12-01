using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface MoveStrategy
{
    Vector2 tick(Collectible collectible);
    public class Grab : MoveStrategy {
        public static float TargetDist = 2.5f;
        public static float TargetSpring = 0.6f;
        public static float PappyPower = 5f;

        readonly Transform parent;
        readonly Transform grandparent;
        Vector3 velo;
        public Grab(Transform parent, Transform grandparent, Vector3 velo) {
            this.parent = parent; this.grandparent = grandparent; this.velo = velo;
        }

        public Vector2 tick(Collectible collectible) {
            var currentPos = collectible.transform.localPosition;
            var perturb = currentPos - parent.transform.localPosition;
            var force = -perturb * (perturb.magnitude - TargetDist) * TargetSpring;
            if (grandparent != null) {
                var grappyPerturb = currentPos - grandparent.transform.localPosition;
                force += PappyPower * grappyPerturb / grappyPerturb.sqrMagnitude;
            }
            //velo += force * Time.deltaTime;
            //velo *= Mathf.Exp(Damping, -Time.deltaTime);
            //currentPos += velo * Time.deltaTime;
            currentPos += force * Time.deltaTime;
            return currentPos;
        }
    }
    public record Release(float time, Vector3 startPos, Vector3 endPos) : MoveStrategy {
        public static float ReturnTime = 1f;
        public static float ReturnExp = 8f;
        public Vector2 tick(Collectible collectible) {
            var t = (Time.time - time) / ReturnTime;
            if (t >= 1) {
                End(collectible);
                return endPos;
            }
            var expTime = 1 - (Mathf.Pow(ReturnExp, 1 - t) - 1) / (ReturnExp - 1);
            Debug.Log(expTime);
            return Vector3.Lerp(startPos, endPos, expTime);
        }
        public virtual void End(Collectible collectible) {
            collectible.GetComponent<Collider2D>().enabled = true;
            collectible.move = null;
        }
    }
    public record TimedDelay(float time, MoveStrategy prev, Func<MoveStrategy> next) : MoveStrategy {
        public Vector2 tick(Collectible collectible) {
            if (Time.time > time) {
                var nextSt = next();
                collectible.move = nextSt;
                return nextSt.tick(collectible);
            }
            return prev.tick(collectible);
        }
    }
    public record ReleaseOpen(float time, Vector3 startPos, Vector3 endPos, RuneDoor door) : Release(time, startPos, endPos) {
        public override void End(Collectible collectible) {
            base.End(collectible);
            door.GetComponent<Collider2D>().enabled = false;
            door.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
