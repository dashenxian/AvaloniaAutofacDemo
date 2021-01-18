using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Abp
{


    public class SerilogOption
    {
        public string[] Using { get; set; }
        public Minimumlevel MinimumLevel { get; set; }
        public WritetoAsync WriteToAsync { get; set; }
        public string[] Enrich { get; set; }
    }

    public class Minimumlevel
    {
        public string Default { get; set; }
        public Override Override { get; set; }
    }

    public class Override
    {
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class WritetoAsync
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }

    public class Args
    {
        public Configure[] configure { get; set; }
    }

    public class Configure
    {
        public string Name { get; set; }
        public Args1 Args { get; set; }
    }

    public class Args1
    {
        public string path { get; set; }
    }
}