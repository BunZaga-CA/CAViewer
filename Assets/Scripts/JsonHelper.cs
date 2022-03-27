using System;
using System.Collections.Generic;

public class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    public static List<T> ListFromJson<T>(string json)
    {
        WrapperList<T> wrapper = UnityEngine.JsonUtility.FromJson<WrapperList<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(List<T> list)
    {
        WrapperList<T> wrapper = new WrapperList<T>();
        wrapper.Items = list;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }
    
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
    
    [Serializable]
    private class WrapperList<T>
    {
        public List<T> Items;
    }
}
