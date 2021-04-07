using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoManager.Model
{
    public class DrugPostForm
    {
        public string DrugName { get; set; }

        public decimal DrugPrice { get; set; }

        public string DrugUsage { get; set; }

        public string DrugSave { get; set; }
    }
    public class DrugAddForm
    {
        public string DrugName { get; set; }

        public decimal DrugPrice { get; set; }

        public string DrugUsage { get; set; }

        public string DrugSave { get; set; }
    }
    public class ProcessForm
    {
        public string Name { get; set; }
        public string Video { get; set; }
        public string Image { get; set; }
        public string Process { get; set; }
        public string Route { get; set; }
    }
    public class CaseForm
    {

        public string CaseName { get; set; }
        public CaseStages CaseStage { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public string CaseType { get; set; }
    }
    public class ChargeProjectForm
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectCharge { get; set; }
    }
    public class AuthCheckResponse
    {
        public string IsAdmin { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Message { get; set; }
    }
}
