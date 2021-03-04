using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dook.tools.animatey
{
    [Serializable]
    public class Animatey
    {
        public AnimationCurve Curve;
        public float duration;
        public Vector2 FromTo = new Vector2(0, 1);
        private float chainTime = -1;

        public delegate void OnAnimate(float val, object args = null);

        public OnAnimate Action;

        private bool chainFired;
        public EventHandler Chain;
        public EventHandler Done;

        public bool playedInReverse { get; private set; }

        public IEnumerator Routine(bool reverse = false, float speed = 1, object args = null)
        {
            playedInReverse = reverse;

            float timeElapsed = 0;
            while (timeElapsed <= duration)
            {
                Evaluate(timeElapsed, reverse, args);

                timeElapsed += Time.deltaTime * speed;

                yield return null;
            }

            Evaluate(duration, reverse, args); //make sure the final tick actually happens
            Done?.Invoke(this, null);
            chainFired = false;
        }
        
        private void Evaluate(float timeElapsed, bool reverse, object args)
        {
            var i = reverse ? Curve.Evaluate(1 - (timeElapsed / duration)) : Curve.Evaluate(timeElapsed / duration);

            i = Mathf.Clamp01(i);
            var val = Mathf.Lerp(FromTo.x, FromTo.y, i);

            Action?.Invoke(val, args);
            
            if (chainTime > 0 && i > chainTime && !chainFired)
            {
                Chain?.Invoke(this, null);
                chainFired = true;
            }
        }

        public Animatey ChainAt(float duration)
        {
            chainTime = duration;
            return this;
        }

        public Animatey SetGoal(float goal)
        {
            FromTo.y = goal;
            return this;
        }

        public void Play(MonoBehaviour behaviour, bool reverse = false, float speed = 1)
        {
            behaviour.StopCoroutine(Routine());
            behaviour.StartCoroutine(Routine(reverse, speed));
        }
    }
}
