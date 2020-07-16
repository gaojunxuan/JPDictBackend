using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SQLite;

namespace JPDictBackend.Model
{
    public class Dict
    {
        public Dict()
        {

        }
        [PrimaryKey, AutoIncrement]
        public int DefinitionId { get; set; }
        public int ItemId { get; set; }
        public string Definition { get; set; }
        public string Pos { get; set; }
        public string Keyword { get; set; }
        public string Reading { get; set; }
        public string Kanji { get; set; }
        public string LoanWord { get; set; }
        public string SeeAlso { get; set; }
        [JsonIgnore]
        public bool IsInNotebook { get; set; }
    }
}
