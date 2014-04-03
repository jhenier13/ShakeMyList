using System;
using System.Collections.Generic;

namespace ShakeMyList.Mobile.Views
{
    public interface IMenuView
    {
        List<ShakeList> RecentLists{ get; set; }
    }
}

