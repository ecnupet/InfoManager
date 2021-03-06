using static InfoManager.Response.ResponseResult;
using InfoManager.Model;
using InfoManager.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Processing.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;

namespace InfoManager.Controllers
{
    [Route("api/im")]
    [ApiController]
    public class InfoManagerController : ControllerBase
    {
        public PersonServiceGrpc personServe { get; set; }
        public InfoContext _context { get; set; }
        public IHttpContextAccessor _accessor { get; set; }
        public HttpClient Http { get; set; }
        public InfoManagerController(InfoContext context, PersonServiceGrpc personServiceGrpc, IHttpContextAccessor http, HttpClient httpClient)
        {
            personServe = personServiceGrpc;
            _context = context;
            _accessor = http;
            Http = httpClient;
        }

        #region disease

        [HttpPost("diseaseadd")] 
        public async Task<ActionResult<ResponseResultModel<Object>>> DiseaseAddAsync(DiseasePostForm diseasePostForm)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Diseases.Where(d => d.DiseaseName == diseasePostForm.DiseaseName).FirstOrDefault();
            if(res!=null)
            {
                return Fail("疾病已存在");
            }
            _context.Diseases.Add(new Disease { DiseaseName = diseasePostForm.DiseaseName, DiseaseType = diseasePostForm.DiseaseType });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }


        [HttpGet("diseaseget")]
        public async Task<ActionResult<ResponseResultModel<List<Disease>>>> DiseaseGetAsync()
        {
            var result = await AuthCheck<List<Disease>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Diseases.Select(d => d);


            return Success<List<Disease>>(res.ToList(), "添加成功");
        }

        [HttpPost("diseaseupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DiseaseUpdateAsync(Disease disease)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Diseases.Where(d => d.ID == disease.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("疾病不存在");
            }
            res.DiseaseName = disease.DiseaseName;
            res.DiseaseType = disease.DiseaseType;
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }


        [HttpPost("diseasedelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DiseaseDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Diseases.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("疾病不存在");
            }
            _context.Diseases.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }
        #endregion

        #region drug

        [HttpGet("drug")]
        public async Task<ActionResult<ResponseResultModel<SearchResult<Drug>>>> DrugSearchAsync(int page, int pageSize, string keyword)
        {
            var result = await AuthCheck<SearchResult<Drug>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var key = keyword ?? "";
            var number = _context.Drugs.Where(x => x.DrugName.Contains(key)).Count();
            var drugs = _context.Drugs.Where(x => x.DrugName.Contains(key)).Select(x => x).Take(page * pageSize).Skip((page - 1) * pageSize).ToList();
            var res = new SearchResult<Drug> { Records = drugs, Count = number };
            return Success(res, "查询成功");
        }

        [HttpPost("drugupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DrugUpdateAsync(Drug drug)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Drugs.Where(d => d.ID == drug.ID).FirstOrDefault();
            if(res == null)
            {
                return Fail(default(object), "药品不存在");
            }
            res.DrugName = drug.DrugName;
            res.DrugPrice = drug.DrugPrice;
            res.DrugSave = drug.DrugSave;
            res.DrugUsage = drug.DrugUsage;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }

        [HttpPost("drugadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DrugAddAsync(DrugPostForm drugPostForm)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Drugs.Where(d => d.DrugName == drugPostForm.DrugName).FirstOrDefault();
            if (res != null)
            {
                return Fail("药品已存在");
            }
            _context.Drugs.Add(new Drug {DrugName = drugPostForm.DrugName, DrugPrice = drugPostForm.DrugPrice, DrugSave = drugPostForm.DrugSave, DrugUsage = drugPostForm.DrugUsage });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpPost("drugdelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DrugDeleteAsync(DeleteForm deleteForm)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Drugs.Where(d => d.ID == deleteForm.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("药品不存在");
            }
            _context.Drugs.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }

        #endregion

        #region case


        [HttpGet("diseasecase")]
        public async Task<ActionResult<ResponseResultModel<DiseaseAllStage>>> DiseaseStageGetAsync(int diseaseID)
        {
            var result = await AuthCheck<DiseaseAllStage>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var cases = _context.Cases.Where(c => c.DiseaseID == diseaseID);
            var res = new DiseaseAllStage();
            if (cases.Count() < 1)
            {
                return Success(res, "查询成功");
            }

            foreach(var c in cases)
            {
                switch(c.CaseStage)
                {
                    case CaseStages.Introduce:
                        res.Introduce = c;
                        break;
                    case CaseStages.ClinicalReception:
                        res.ClinicalReception = c;
                        break;
                    case CaseStages.Check:
                        res.Check = c;
                        break;
                    case CaseStages.Diagnosis:
                        res.Diagnosis = c;
                        break;
                    case CaseStages.TherapeuticSchedule:
                        res.TherapeuticSchedule = c;
                        break;
                }
            }
            return Success(res, "查询成功");
        }


        [HttpPost("caseupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> CaseUpdateAsync(Case cases)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Cases.Where(d => d.ID == cases.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "病例不存在");
            }
            res.DiseaseID = cases.DiseaseID;
            res.CaseStage = cases.CaseStage;
            res.Description = cases.Description;
            res.Image = cases.Image;
            res.Video = cases.Video;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }

        [HttpPost("caseadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> CaseAddAsync(CaseForm cases)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Cases.Where(d => d.DiseaseID == cases.DiseaseID && d.CaseStage == cases.CaseStage).FirstOrDefault();
            if (res != null)
            {
                return Fail("病例已存在");
            }
            _context.Cases.Add(new Case { Image = cases.Image, Video = cases.Video, DiseaseID = cases.DiseaseID, CaseStage = cases.CaseStage, Description = cases.Description });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpPost("casedelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> CaseDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Cases.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("病例不存在");
            }
            _context.Cases.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }
        #endregion

        #region process
        [HttpGet("process")]
        public async Task<ActionResult<ResponseResultModel<RoomProcess>>> ProcessGet(string processRoute)
        {
            var result = await AuthCheck<RoomProcess>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var processes = processRoute.Split(";", StringSplitOptions.RemoveEmptyEntries);
            if (processes.Count() == 0)
            {
                return Fail(default(RoomProcess), "流程为空");
            }
            int fatherID = 0;
            RoomProcess roomProcess = default(RoomProcess);
            foreach (var i in processes)
            {
                if (fatherID == 0)
                {
                    fatherID = _context.RoomProcesses.Where(rp => rp.Name == i).Select(rp => rp.ID).FirstOrDefault();
                    roomProcess = _context.RoomProcesses.Where(rp => rp.Name == i).FirstOrDefault();

                }
                else
                {

                    roomProcess = _context.RoomProcesses.Where(rp => rp.Name == i && rp.FatherId == fatherID).FirstOrDefault();
                    fatherID = _context.RoomProcesses.Where(rp => rp.Name == i && rp.FatherId == fatherID).Select(rp => rp.ID).FirstOrDefault();
                }
            }
            if(roomProcess == null)
            {
                return Fail(default(RoomProcess), "流程出错");
            }
            return Success(roomProcess, "查询成功");
        }
        [HttpGet("processall")]
        public async Task<ActionResult<ResponseResultModel<List<RoomProcess>>>> ProcessAllGet()
        {
            var result = await AdminCheck<List<RoomProcess>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var roomProcess = _context.RoomProcesses.Select(x => x).ToList();
            return Success(roomProcess, "查询成功");
        }
        [HttpPost("processdelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ProcessDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.RoomProcesses.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("流程不存在");
            }
            _context.RoomProcesses.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }

        [HttpPost("processupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ProcessUpdateAsync(RoomProcess process)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.RoomProcesses.Where(d => d.ID == process.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "流程不存在");
            }
            res.Image = process.Image;
            res.Name = process.Name;
            res.Process = process.Process;
            res.Video = process.Video;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("processadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ProcessAddAsync(ProcessForm process)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var processes = process.Route.Split(";", StringSplitOptions.RemoveEmptyEntries);
            if (processes.Count() == 0)
            {
                _context.RoomProcesses.Add(new RoomProcess { FatherId = 0, Image = process.Image, Name = process.Name, Process = process.Process, Video = process.Video });
                return Success("添加成功");
            }
            int fatherID = 0;
            RoomProcess roomProcess = default(RoomProcess);
            foreach (var i in processes)
            {

                roomProcess = _context.RoomProcesses.Where(rp => rp.Name == i && rp.FatherId == fatherID).FirstOrDefault();
                if (roomProcess == null)
                {
                    return Fail("流程路径不合法");
                }
                fatherID = roomProcess.ID;
            }

            var checkProcess = _context.RoomProcesses.Where(rp => rp.FatherId == fatherID && rp.Name == process.Name);
            if (checkProcess.Count() != 0)
            {
                return Fail("流程已存在");
            }
            
            _context.RoomProcesses.Add(new RoomProcess{Name = process.Name, Process = process.Process, FatherId = fatherID, Video = process.Video, Image = process.Image });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        #endregion

        #region chargeproject
        [HttpPost("chargeprojectupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ChargeProjectUpdateAsync(ChargeProject chargeProject)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.ChargeProjects.Where(d => d.ID == chargeProject.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "收费项目不存在");
            }
            res.ProjectName = chargeProject.ProjectName;
            res.ProjectDescription = chargeProject.ProjectDescription;
            res.ProjectCharge = chargeProject.ProjectCharge;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("chargeprojectdelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ChargeProjectDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.ChargeProjects.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("收费项目不存在");
            }
            _context.ChargeProjects.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }
        [HttpPost("chargeProjectadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ChargeProjectAddAsync(ChargeProjectForm charge)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.ChargeProjects.Where(d => d.ProjectName == charge.ProjectName).FirstOrDefault();
            if (res != null)
            {
                return Fail("收费项目已存在");
            }
            _context.ChargeProjects.Add(new ChargeProject {ProjectCharge = charge.ProjectCharge, ProjectDescription = charge.ProjectDescription, ProjectName = charge.ProjectName });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpGet("chargeprojectsearch")]
        public async Task<ActionResult<ResponseResultModel<SearchResult<ChargeProject>>>> ChargeProjectSearchAsync(int page, int pageSize, string keyWord)
        {
            var result = await AuthCheck<SearchResult<ChargeProject>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var key = keyWord ?? "";
            var number = _context.ChargeProjects.Where(x => x.ProjectName.Contains(key)).Count();
            var chargeProjects = _context.ChargeProjects.Where(x => x.ProjectName.Contains(key)).Select(x => x).Take(page * pageSize).Skip((page - 1) * pageSize).ToList();
            var res = new SearchResult<ChargeProject> { Records = chargeProjects, Count = number };
            return Success(res, "查询成功");
        }
        #endregion

        #region document
        [HttpPost("docupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DocumentUpdateAsync(Document document)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Documents.Where(d => d.ID == document.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "档案不存在");
            }
            res.Description = document.Description;
            res.DocumentName = document.DocumentName;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("docdelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DocumentDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Documents.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("档案不存在");
            }
            _context.Documents.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }
        [HttpPost("docadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DocumentAddAsync(DocumentForm document)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Documents.Where(d => d.DocumentName == document.DocumentName).FirstOrDefault();
            if (res != null)
            {
                return Fail("档案已存在");
            }
            _context.Documents.Add(new Document { DocumentName = document.DocumentName, Description = document.Description});
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpGet("docsearch")]
        public async Task<ActionResult<ResponseResultModel<SearchResult<Document>>>> DocumentSearchAsync(int page, int pageSize, string keyWord)
        {
            var result = await AuthCheck<SearchResult<Document>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var key = keyWord ?? "";
            var number = _context.Documents.Where(x => x.DocumentName.Contains(key)).Count();
            var chargeProjects = _context.Documents.Where(x => x.DocumentName.Contains(key)).Select(x => x).Take(page * pageSize).Skip((page - 1) * pageSize).ToList();
            var res = new SearchResult<Document> { Records = chargeProjects, Count = number };
            return Success(res, "查询成功");
        }
        #endregion
        #region vaccine
        [HttpPost("vaccineupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> VaccineUpdateAsync(Vaccine vaccine)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Vaccines.Where(d => d.ID == vaccine.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "疫苗不存在");
            }
            res.Description = vaccine.Description;
            res.Image = vaccine.Image;
            res.Name = vaccine.Name;
            res.Video = vaccine.Video;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("vaccinedelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> VaccineDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Vaccines.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("疫苗不存在");
            }
            _context.Vaccines.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }
        [HttpPost("vaccineadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> VaccineAddAsync(Vaccine vaccine)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.Vaccines.Where(d => d.Name == vaccine.Name).FirstOrDefault();
            if (res != null)
            {
                return Fail("疫苗已存在");
            }
            _context.Vaccines.Add(new Vaccine { Name = vaccine.Name , Video = vaccine.Video, Image = vaccine.Image, Description = vaccine.Description}) ;
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpGet("vaccinesearch")]
        public async Task<ActionResult<ResponseResultModel<SearchResult<Vaccine>>>> VaccineSearchAsync(int page, int pageSize, string keyWord)
        {
            var result = await AuthCheck<SearchResult<Vaccine>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var key = keyWord ?? "";
            var number = _context.Vaccines.Where(x => x.Name.Contains(key)).Count();
            var chargeProjects = _context.Vaccines.Where(x => x.Name.Contains(key)).Select(x => x).Take(page * pageSize).Skip((page - 1) * pageSize).ToList();
            var res = new SearchResult<Vaccine> { Records = chargeProjects, Count = number };
            return Success(res, "查询成功");
        }
        #endregion

        #region inspectionproject
        [HttpPost("inspectionupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> InspectionUpdateAsync(InspectionProject inspection)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.InspectionProjects.Where(d => d.ID == inspection.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "化验项目不存在");
            }
            res.Description = inspection.Description;
            res.Image = inspection.Image;
            res.Name = inspection.Name;
            res.Video = inspection.Video;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("inspectiondelete")]
        public async Task<ActionResult<ResponseResultModel<Object>>> InspectionDeleteAsync(DeleteForm delete)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.InspectionProjects.Where(d => d.ID == delete.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail("化验项目不存在");
            }
            _context.InspectionProjects.Remove(res);
            await _context.SaveChangesAsync();
            return Success("删除成功");
        }
        [HttpPost("inspectionadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> InspectionAddAsync(InspectionForm inspection)
        {
            var result = await AdminCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var res = _context.InspectionProjects.Where(d => d.Name == inspection.Name).FirstOrDefault();
            if (res != null)
            {
                return Fail("化验项目已存在");
            }
            _context.InspectionProjects.Add(new InspectionProject { Name = inspection.Name, Video = inspection.Video, Image = inspection.Image, Description = inspection.Description });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpGet("inspectionsearch")]
        public async Task<ActionResult<ResponseResultModel<SearchResult<InspectionProject>>>> InspectionSearchAsync(int page, int pageSize, string keyWord)
        {
            var result = await AuthCheck<SearchResult<InspectionProject>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Unauthorized)
            {
                return result;
            }
            var key = keyWord ?? "";
            var number = _context.InspectionProjects.Where(x => x.Name.Contains(key)).Count();
            var inspections = _context.InspectionProjects.Where(x => x.Name.Contains(key)).Select(x => x).Take(page * pageSize).Skip((page - 1) * pageSize).ToList();
            var res = new SearchResult<InspectionProject> { Records = inspections, Count = number };
            return Success(res, "查询成功");
        }
        #endregion
        [NonAction]
        public async Task<ResponseResultModel<T>> AuthCheck<T>(IRequestCookieCollection cookies)
        {

            if (cookies.Count() == 0)
            {
                return ResponseResult.Unauthorized(default(T), "无权查询");
            }
            var cookie = cookies.First();
            var cookieString = cookie.Key + "=" + cookie.Value;
            var requestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri($"http://pm-app-svc.backend:5000/api/pm/auth/check")
            };
            requestMessage.Headers.Add("cookie", cookieString);
            var response = await Http.SendAsync(requestMessage);
            System.Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            var res =  await response.Content.ReadFromJsonAsync<ResponseResultModel<AuthCheckResponse>>();
            if (!res.Data.Message)
            {
                return ResponseResult.Unauthorized(default(T), "无权查询");
            }   
            return Success(default(T), "权限验证");
        }
        [NonAction]
        public async Task<ResponseResultModel<T>> AdminCheck<T>(IRequestCookieCollection cookies)
        {

            if (cookies.Count() == 0)
            {
                return ResponseResult.Unauthorized(default(T), "无权查询");
            }
            var cookie = cookies.First();
            var cookieString = cookie.Key + "=" + cookie.Value;
            var requestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri($"http://pm-app-svc.backend:5000/api/pm/auth/check")
            };
            requestMessage.Headers.Add("cookie", cookieString);
            var response = await Http.SendAsync(requestMessage);
            System.Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            var res = await response.Content.ReadFromJsonAsync<ResponseResultModel<AuthCheckResponse>>();
            if (!res.Data.Message || int.Parse(res.Data.Authorization) < 1)
            {
                return ResponseResult.Unauthorized(default(T), "无权查询");
            }
            return Success(default(T), "权限验证");
        }

    }
}
