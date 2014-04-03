using System;
using System.Collections.Generic;

namespace ShakeMyList.Mobile.Views
{
    public interface IShakeListsManagerView
    {
        List<ShakeList> AllLists{ get; set; }
    }
}

