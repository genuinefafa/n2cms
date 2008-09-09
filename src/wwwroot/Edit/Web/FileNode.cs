﻿using System;
using System.Collections.Generic;
using System.Web;

namespace N2.Edit.Web
{
    public class FileNode : FileSiteMapNode
    {
        public FileNode(SiteMapProvider provider, string url) : base(provider, url)
        {
        }

        public override string IconUrl
        {
            get { return "../img/ico/page_white.gif"; }
        }

        public override string Target
        {
            get { return "file"; }
        }
    }
}
