// A stub of the few UnityEngine types this package uses, their signatures as in Unity 6000.3's
// UnityEngine.CoreModule, so that the package compiles on a machine without Unity (the CI). Debug
// writes to the console, so that Program.cs can run the sample's first call. Not shipped: Unity
// ignores Tests~.
using System;
using System.Collections;

namespace UnityEngine
{
    public class Object { }

    public class Component : Object
    {
        public T GetComponent<T>() => default;
    }

    public class Behaviour : Component
    {
        public bool enabled { get; set; } = true;
    }

    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(IEnumerator routine) => new Coroutine();
    }

    public sealed class Coroutine { }

    public class YieldInstruction { }

    public struct Vector3 { }

    public struct Quaternion { }

    public sealed class WaitForSeconds : YieldInstruction
    {
        public WaitForSeconds(float seconds) { }
    }

    public static class Application
    {
        public static bool isBatchMode => false;
    }

    public static class Debug
    {
        public static void Log(object message) => Console.WriteLine(message);

        public static void LogWarning(object message) => Console.WriteLine("warning: " + message);

        public static void LogError(object message) => Console.WriteLine("error: " + message);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddComponentMenu : Attribute
    {
        public AddComponentMenu(string menuName) { }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyAttribute : Attribute { }

    public sealed class HeaderAttribute : PropertyAttribute
    {
        public HeaderAttribute(string header) { }
    }

    public sealed class TooltipAttribute : PropertyAttribute
    {
        public TooltipAttribute(string tooltip) { }
    }
}
