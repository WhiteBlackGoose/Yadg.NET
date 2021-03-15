﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YadgNet
{
    public sealed class DescriptionFromXmlBuilder
    {
        private string xml;
        public DescriptionFromXmlBuilder(string xml)
            => this.xml = xml;

        private static string FixLinks(string xml)
        {
            if (!xml.Contains("<a"))
                return xml;
            var id = xml.IndexOf("<a");
            var nextId = xml.IndexOf("/>", id);
            if (nextId == -1)
                return xml;
            var linkInside = xml.Substring(id + 2, nextId - id - 2);
            if (linkInside.Contains('<') || linkInside.Contains('>'))
                return xml;
            return xml.Substring(0, nextId) + ">Link</a>" + FixLinks(xml.Substring(nextId + 2));
        }
        public string Build()
            => FixLinks(xml);
    }
}