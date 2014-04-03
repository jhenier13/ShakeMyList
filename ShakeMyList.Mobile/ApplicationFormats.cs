using System;

namespace ShakeMyList.Mobile
{
    public static class ApplicationFormats
    {
        //Date represented in numbers >> year:month:day (without dots) e.g. 20140325 = 25 March of 2014
        public static readonly string DATE_NORMAL_FORMAT = "yyyyMMdd";
        //Date represented like full format in numbers >> year:month:day:hour:minutes:seconds (without dots) e.g. 20140325154530 = 25 March of 2014 15:45:30 
        public static readonly string DATE_FULL_FORMAT = "yyyyMMddHHmmss";

    }
}

