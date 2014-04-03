using System;
using ShakeMyList.Mobile.Views;
using System.Collections.Generic;

namespace ShakeMyList.Mobile.Presenters
{
    public class ShakeListsManagerPresenter
    {
        private IShakeListsManagerView __view;
        private List<ShakeList> __allLists;

        public ShakeListsManagerPresenter(IShakeListsManagerView view)
        {
            if (view == null)
                throw new ArgumentNullException("The view can't be null");

            __view = view;
        }

        public void LoadData()
        {
            __allLists = ShakeList.GetAllLists();
            __view.AllLists = __allLists;
        }

        public void DeleteList(int index)
        {
            ShakeList list = __allLists[index];
            list.Delete();
            __allLists.RemoveAt(index);
        }

        public ShakeList ListFrom(int index)
        {
            return __allLists[index];
        }
    }
}

