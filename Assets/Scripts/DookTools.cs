using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace dook.tools
{
    public static class DookTools
    {
        /// <summary>
        /// Clamp a vector's length
        /// </summary>
        public static Vector3 ClampMagnitude(Vector3 v, float min, float max)
        {
            double sm = v.sqrMagnitude;
            if (sm > (double) max * (double) max) return v.normalized * max;
            else if (sm < (double) min * (double) min) return v.normalized * min;
            return v;
        }

        /// <summary>
        /// Find length of an animator clip given a name
        /// </summary>
        /// <returns>Clip length, null if not found</returns>
        public static float? GetACLength(RuntimeAnimatorController ac, string clipName)
        {
            return GetACLength(ac.animationClips, clipName);
        }

        /// <summary>
        /// Find length of an animator clip given a name
        /// </summary>
        /// <returns>Clip length, null if not found</returns>
        public static float? GetACLength(AnimationClip[] clips, string clipName)
        {
            foreach (var clip in clips)
            {
                if (clip.name.Contains(clipName)) //If it has the same name as your clip
                {
                    return clip.length;
                }
            }

            return null;
        }

        /// <summary>
        /// Find length of an animator clip given a name
        /// </summary>
        /// <returns>Clip length, null if not found</returns>
        public static float? GetACLength(Animator anim, string clipName)
        {
            return GetACLength(anim.runtimeAnimatorController, clipName);
        }


        /// <summary>
        /// Fade an image from a color to another
        /// </summary>
        public static IEnumerator Fade(Image img, float duration, Color from, Color to)
        {
            float elapsed = 0;
            float lerp = 0;
            while (lerp < 1)
            {
                img.color = Color.Lerp(from, to, lerp);
                lerp = elapsed / duration;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Hijack a coroutine so that it can be nested synchrounously
        /// https://jacksondunstan.com/articles/3241
        /// </summary>
        public static IEnumerator NestCoroutine(IEnumerator routine)
        {
            while (routine.MoveNext())
            {
                yield return routine.Current;
            }
        }

        /// <summary>
        /// Draw a ray using debug.draw
        /// </summary>
        public static void DrawRay(Ray r, Color c, float time)
        {
            Debug.DrawRay(r.origin, r.direction, c, time);
        }

        /// <summary>
        /// Round a vector
        /// </summary>
        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            float multiplier = 1;
            for (int i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }

            return new Vector3(
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier);
        }

        public static Vector3 ShittyPerlinVector(float x, float y)
        {
            return new Vector3(Mathf.PerlinNoise(x, y), Mathf.PerlinNoise(x * 2, y * 2),
                Mathf.PerlinNoise(x * 3, y * 3));
        }


        /// <summary>
        /// Sway a vector on X and Y (ie aiming a gun) 
        /// </summary>
        /// <param name="Accuracy"></param>
        /// <returns>Swayed vector</returns>
        public static Vector3 Sway(float Accuracy)
        {
            return new Vector3(
                Accuracy * (Mathf.PerlinNoise(0, Time.time) * 2) - 1,
                Accuracy * (Mathf.PerlinNoise(Time.time, 0) * 2) - 1,
                0);
        }

        //https://answers.unity.com/questions/1007585/reading-and-setting-asn-objects-global-scale-with.html
        /// <summary>
        /// Sets a transform's scale globally
        /// </summary>
        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x,
                globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }

        /// <summary>
        /// Wait for real seconds elapsed instead of using game time
        /// </summary>
        /// <param name="time">time to wait</param>
        public static IEnumerator WaitForRealSeconds(float time)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }
        }

        public static float Map(float value, float OldMin, float OldMax, float NewMin, float NewMax)
        {
            float oldRange = (OldMax - OldMin);
            float newRange = (NewMax - NewMin);
            float newValue = (((value - OldMin) * newRange) / oldRange) + NewMin;

            return (newValue);
        }

        /// <summary>
        /// Finds the transform closest to a point in an array 
        /// </summary>
        /// <returns>Closest transform</returns>
        public static Transform FindClosest(this Component[] transforms, Vector3 pointToCompare)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = pointToCompare;
            foreach (Component potentialTarget in transforms)
            {
                if (potentialTarget == null) continue;

                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }

            return bestTarget;
        }

        /// <summary>
        /// Returns the closest point on a line given a position 
        /// </summary>
        /// <param name="p">Current position</param>
        /// <param name="a">Line Start</param>
        /// <param name="b">Line End</param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + Vector3.Project(p - a, b - a);
        }
        

        /// <summary>
        /// Get point on bezier curve 
        /// </summary>
        /// <param name="p0">bezier start</param>
        /// <param name="p1">bezier modifier</param>
        /// <param name="p2">bezier end</param>
        /// <param name="time">0-1 on how far along the curve to get</param>
        /// <returns></returns>
        public static Vector3 GetBezier(Vector3 p0, Vector3 p1, Vector3 p2, float time)
        {
            var t = InOutEase(time);
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }
        public static float InOutEase(float k)
        {
            if ((k *= 2f) < 1f) return 0.5f * k * k;
            return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
        }

        public static float DTLerp(float a, float b, float factor)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Pow(factor, Time.deltaTime));
        }

        /// <summary>
        /// Quick method to set color's alpha
        /// </summary>
        /// <param name="c"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color SetColorAlpha(Color c, float alpha)
        {
            Color newColor = c;
            newColor.a = alpha;
            return newColor;
        }

        /// <summary>
        /// Draw a rect using debug.drawline
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="time">time rect is drawn for</param>
        public static void DrawRect(Vector3 position, float size, Color color, float time)
        {
            Debug.DrawLine(position, new Vector3(position.x + size, position.y, position.z), color, time);
            Debug.DrawLine(position, new Vector3(position.x, position.y - size, position.z), color, time);
            Debug.DrawLine(new Vector3(position.x, position.y - size, position.z),
                new Vector3(position.x + size, position.y - size, position.z), color, time);
            Debug.DrawLine(new Vector3(position.x + size, position.y - size, position.z),
                new Vector3(position.x + size, position.y, position.z), color, time);
        }

        /// <summary>
        /// Round to decimal places
        /// </summary>
        /// <param name="places">How many decimal places to round to</param>
        /// <returns></returns>
        public static float RoundTo(this float a, float places)
        {
            return Mathf.Round(a / places) * places;
        }

        /// <summary>
        /// Get the middle point of a bunch of points
        /// </summary>
        /// <param name="positions">Array of points to get average of</param>
        /// <returns>Middle point of array</returns>
        public static Vector3 GetMeanVector(Vector3[] positions)
        {
            if (positions.Length == 0)
                return Vector3.zero;
            float x = 0f;
            float y = 0f;
            float z = 0f;
            foreach (Vector3 pos in positions)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
            }

            return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
        }

        //http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        /// <summary>
        /// Smooth lerp but takes delta time into account, use in lerp for interpolation
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float DTFactor(float f) => 1 - Mathf.Pow(f, Time.deltaTime);

        
        //Easily convert an int to number row keycodes
        public static KeyCode[] Numkeycodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Alpha0,
        };
        
        //http://csharphelper.com/blog/2018/04/make-extension-methods-that-pick-random-items-from-arrays-or-lists-in-c/
        // Return a random item from an array.
        public static T GetRandom<T>(this T[] items)
        {
            // Return a random item.
            return items[Random.Range(0, items.Length)];
        }

        // Return a random item from a list.
        public static T GetRandom<T>(this List<T> items)
        {
            // Return a random item.
            return items[Random.Range(0, items.Count)];
        }
    
        //https://answers.unity.com/questions/608674/does-mathfpingpong-always-have-to-start-at-000.html
        //Pingpong range
        public static float PingPong(float aValue, float aMin, float aMax)
        {
            return Mathf.PingPong(aValue, aMax-aMin) + aMin;
        }
    }
    
    public struct TransformSnapShot
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformSnapShot(Transform t)
        {
            Position = t.position;
            Rotation = t.rotation;
        }
    }
}

