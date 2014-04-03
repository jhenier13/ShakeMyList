using System;
using ShakeMyList.Mobile.Views;

namespace ShakeMyList.Mobile.Presenters
{
    public class MenuPresenter
    {
        private IMenuView __view;

        public MenuPresenter(IMenuView view)
        {
            if (view == null)
                throw new ArgumentNullException("The view can't be null");

            __view = view;
            __view.RecentLists = ShakeList.GetRecentLists();
        }
    }
}

