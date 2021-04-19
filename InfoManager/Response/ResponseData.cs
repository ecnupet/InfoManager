using InfoManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Response
{
    public class DrugData
    {

        public string DrugName { get; set; }
        public decimal DrugPrice { get; set; }
        public string DrugUsage { get; set; }
        public string DrugSave { get; set; }
    }
    public class CaseName
    {
        public string Name { get; set; }
        public string CaseType { get; set; }
    }
    public class SearchResult<T>
    {
        public int Count { get; set; }
        public List<T> Records { get; set; }
    }

}
