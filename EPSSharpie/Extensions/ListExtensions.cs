using System;
using System.Collections.Generic;

namespace EPSSharpie
{
    public static class ListExtensions
    {
        public static T Pop<T>(this List<T> list)
        {
             var index = list.Count - 1;
             var result = list[index];
             list.RemoveAt(index);
             return result;
        }

        public static T Top<T>(this List<T> list)
        {
            var index = list.Count - 1;
            return list[index];
        }

        public static T[] Pop<T>(this List<T> list, int count)
        {
            var temp = new List<T>();
            for (var i = 0; i < count; i++)
            {
                temp.Add(list.Pop());
            }
            return temp.ToArray();
        }

        public static void Push<T>(this List<T> list, T value)
        {
            list.Add(value);
        }

        public static void Push<T>(this List<T> list, T[] values)
        {
            foreach (var value in values)
            {
                list.Add(value);
            }
        }
    }
}
