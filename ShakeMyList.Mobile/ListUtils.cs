using System;
using System.Collections.Generic;

namespace ShakeMyList.Mobile
{
    public static class ListUtils
    {
        public static void Move<T>(this IList<T> list, int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex)
                return;

            T item = list[fromIndex];
            int insertAt = toIndex;
            int deleteAt = fromIndex;

            if (toIndex < fromIndex)
                deleteAt += 1;
            else
                insertAt += 1;

            list.Insert(insertAt, item);
            list.RemoveAt(deleteAt);
        }
    }
}

