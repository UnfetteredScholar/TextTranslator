﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslator
{
    internal class RequestContent
    {
        public string sentence { get; set; }

        public RequestContent(string request)
        {
            sentence = request;
        }
    }
}
