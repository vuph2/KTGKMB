using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using KTGKMD.Exams.Dto;
using KTGKMD.Exams.Services;
using Microsoft.AspNetCore.Mvc;

namespace KTGKMD.Web.Controllers
{
    [Route("api/exams")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class ExamApiController : AbpController
    {
        private readonly IExamAppService _examAppService;

        public ExamApiController(IExamAppService examAppService)
        {
            _examAppService = examAppService;
        }

        // c) Giao dien hien thi danh sach de thi cho Mobile (Ten de, thoi gian lam bai, so luong cau hoi)
        [HttpGet]
        public async Task<ActionResult<List<ExamDto>>> GetAllExams()
        {
            var exams = await _examAppService.GetAllExamsAsync();
            return Ok(exams);
        }

        // Chi tiet de thi cho Mobile render giao dien lam bai
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDetailDto>> GetExamDetail(int id)
        {
            var detail = await _examAppService.GetExamDetailAsync(id);
            return Ok(detail);
        }

        // Bat dau tinh gio lam bai tren Mobile (Tao luot thi ExamAttempt)
        [HttpPost("start")]
        public async Task<ActionResult<ExamAttemptDto>> StartAttempt([FromBody] StartAttemptInput input)
        {
            var attempt = await _examAppService.StartAttemptAsync(input);
            return Ok(attempt);
        }

        // d) Ghi nhan trang thai lam bai: Luu tru tam thoi cac dap an nguoi dung chon
        [HttpPost("save-answer")]
        public async Task<ActionResult> SaveAnswer([FromBody] SaveAnswerInput input)
        {
            await _examAppService.SaveAnswerAsync(input);
            return Ok(new { success = true, message = "Lưu đáp án thành công." });
        }

        // e) Nop bai & Cham diem tu dong
        [HttpPost("submit")]
        public async Task<ActionResult<SubmitExamResultDto>> SubmitExam([FromBody] SubmitExamInput input)
        {
            var result = await _examAppService.SubmitExamAsync(input);
            return Ok(result);
        }

        // Xem lai ket qua thi
        [HttpGet("result/{attemptId}")]
        public async Task<ActionResult<SubmitExamResultDto>> GetResult(int attemptId)
        {
            var result = await _examAppService.GetAttemptResultAsync(attemptId);
            return Ok(result);
        }
    }
}
