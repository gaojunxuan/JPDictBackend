﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JPDictBackend.Model
{
    public class DailySentenceResult
    {
        public string Sentence { get; set; }

        public string Trans { get; set; }

        public string Audio { get; set; }

        public string Creator { get; set; }

        public string SentencePoint { get; set; }
    }
}