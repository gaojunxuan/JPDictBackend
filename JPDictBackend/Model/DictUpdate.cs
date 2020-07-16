using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace JPDictBackend.Model
{
    public class DictUpdate : TableEntity
    {
        public int AutoId { get; set; }
        public string Definition { get; set; }
        public string Pos { get; set; }
        public string Keyword { get; set; }
        public string Reading { get; set; }
        public string Kanji { get; set; }
        public string LoanWord { get; set; }
        public string SeeAlso { get; set; }
    }
}
