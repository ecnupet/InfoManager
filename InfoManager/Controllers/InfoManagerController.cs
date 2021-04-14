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

        [HttpGet("drug")]
        public async Task<ActionResult<ResponseResultModel<DrugData>>> DrugInfoGetAsync(string drugID)
        {
            var result = await AuthCheck<DrugData>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var drugs = _context.Drugs.Where(d => d.ID == int.Parse(drugID));
            if (drugs.Count() < 1)
            {
                return Fail(default(DrugData), "无此药品");
            }
            var drug = drugs.First();
            var res = new DrugData { DrugName = drug.DrugName, DrugPrice = drug.DrugPrice, DrugSave = drug.DrugSave, DrugUsage = drug.DrugUsage };
            return Success<DrugData>(res, "查询成功");

        }

        [HttpGet("case")]
        public async Task<ActionResult<ResponseResultModel<Case>>> CaseInfoGetAsync(string caseName, string stage)
        {
            var result = await AuthCheck<Case>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var cases = _context.Cases.Where(c => c.CaseName == caseName && c.CaseStage.ToString() == stage);
            if (cases.Count() < 1)
            {
                return Fail(default(Case), "无此药品");
            }

            var res = cases.First();
            return Success(res, "查询成功");
        }

        [HttpGet("casename")]
        public async Task<ActionResult<ResponseResultModel<List<CaseName>>>> CaseNamesGetAsync()
        {
            var result = await AuthCheck<List<CaseName>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var cases = _context.Cases.Select(c => new CaseName { Name = c.CaseName, CaseType = c.CaseType }).Distinct().ToList();
            return Success(cases, "查询成功");
        }

        [HttpGet("process")]
        public async Task<ActionResult<ResponseResultModel<RoomProcess>>> ProcessGet(string processRoute)
        {
            var result = await AuthCheck<RoomProcess>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
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
            }
            if(roomProcess == null)
            {
                return Fail(default(RoomProcess), "流程出错");
            }
            return Success(roomProcess, "查询成功");
        }

        [HttpGet("chargeprojectnames")]
        public async Task<ActionResult<ResponseResultModel<List<string>>>> ChargeProjectNamesGetAsync()
        {
            var result = await AuthCheck<List<string>>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var res = _context.ChargeProjects.Select(cp => cp.ProjectName).ToList();
            return Success(res, "查询成功");
        }



        [HttpGet("chargeproject")]
        public async Task<ActionResult<ResponseResultModel<ChargeProject>>> ChargeProjectGetAsync(string projectName)
        {
            var result = await AuthCheck<ChargeProject>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var res = _context.ChargeProjects.Where(cp => cp.ProjectCharge == projectName).FirstOrDefault();
            if(res == null)
            {
                return Fail(default(ChargeProject), "项目不存在");
            }
            return Success(res, "查询成功");
        }

        [HttpPost("drugupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> DrugUpdateAsync(Drug drug)
        {
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
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
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
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

        [HttpPost("caseupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> CaseUpdateAsync(Case cases)
        {
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var res = _context.Cases.Where(d => d.ID == cases.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "病例不存在");
            }
            res.CaseName = cases.CaseName;
            res.CaseStage = cases.CaseStage;
            res.CaseType = cases.CaseType;
            res.Description = cases.Description;
            res.Image = cases.Image;
            res.Video = cases.Video;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("caseadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> CaseAddAsync(CaseForm cases)
        {
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var res = _context.Cases.Where(d => d.CaseName == cases.CaseName && d.CaseStage == cases.CaseStage).FirstOrDefault();
            if (res != null)
            {
                return Fail("病例已存在");
            }
            _context.Cases.Add(new Case {Image = cases.Image, Video = cases.Video, CaseName = cases.CaseName, CaseStage = cases.CaseStage, CaseType = cases.CaseType, Description =cases.Description });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpPost("processupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ProcessUpdateAsync(RoomProcess process)
        {
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
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
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var processes = process.Route.Split(";", StringSplitOptions.RemoveEmptyEntries);
            if (processes.Count() == 0)
            {
                _context.RoomProcesses.Add(new RoomProcess { FatherID = 0, Image = process.Image, Name = process.Name, Process = process.Process, Video = process.Video });
                return Success("添加成功");
            }
            int fatherID = 0;
            RoomProcess roomProcess = default(RoomProcess);
            foreach (var i in processes)
            {

                roomProcess = _context.RoomProcesses.Where(rp => rp.Name == i && rp.FatherID == fatherID).FirstOrDefault();
                if (roomProcess == null)
                {
                    return Fail("流程路径不合法");
                }
                fatherID = roomProcess.ID;
            }

            var checkProcess = _context.RoomProcesses.Where(rp => rp.FatherID == fatherID && rp.Name == process.Name);
            if (checkProcess.Count() != 0)
            {
                return Fail("流程已存在");
            }
            
            _context.RoomProcesses.Add(new RoomProcess{Name = process.Name, Process = process.Process, FatherID = fatherID, Video = process.Video, Image = process.Image });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }

        [HttpPost("chargeprojectupdate")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ChargeProjectUpdateAsync(ChargeProject chargeProject)
        {
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var res = _context.ChargeProjects.Where(d => d.ID == chargeProject.ID).FirstOrDefault();
            if (res == null)
            {
                return Fail(default(object), "项目不存在");
            }
            res.ProjectName = chargeProject.ProjectName;
            res.ProjectDescription = chargeProject.ProjectDescription;
            res.ProjectCharge = chargeProject.ProjectCharge;
            await _context.SaveChangesAsync();
            return Success("修改成功");
        }
        [HttpPost("chargeProjectadd")]
        public async Task<ActionResult<ResponseResultModel<Object>>> ChargeProjectAddAsync(ChargeProjectForm charge)
        {
            var result = await AuthCheck<Object>(_accessor.HttpContext.Request.Cookies);
            if (result.State == ResponseResultEnum.Fail)
            {
                return result;
            }
            var res = _context.ChargeProjects.Where(d => d.ProjectName == charge.ProjectName).FirstOrDefault();
            if (res != null)
            {
                return Fail("项目已存在");
            }
            _context.ChargeProjects.Add(new ChargeProject {ProjectCharge = charge.ProjectCharge, ProjectDescription = charge.ProjectDescription, ProjectName = charge.ProjectName });
            await _context.SaveChangesAsync();
            return Success("添加成功");
        }


        [NonAction]
        public async Task<ResponseResultModel<T>> AuthCheck<T>(IRequestCookieCollection cookies)
        {

            if (cookies.Count() == 0)
            {
                return Fail(default(T), "无权查询");
            }
            var cookie = cookies.First();
            var cookieString = cookie.Key + "=" + cookie.Value;
            var requestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri($"https://backend.ecnu.space/api/pm/auth/check")
            };
            requestMessage.Headers.Add("cookie", cookieString);
            var response = await Http.SendAsync(requestMessage);
            System.Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            var res =  await response.Content.ReadFromJsonAsync<ResponseResultModel<AuthCheckResponse>>();
            if (!res.Data.Message)
            {
                return Fail(default(T), "无权查询");
            }   
            return Success(default(T), "权限验证");
        }

    }
}
