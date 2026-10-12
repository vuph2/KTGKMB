using System;
using System.IO;
using System.Threading.Tasks;
using KTGKMD.Controllers;
using KTGKMD.Exams.Dto;
using KTGKMD.Exams.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KTGKMD.Web.Controllers
{
    [IgnoreAntiforgeryToken]
    public class ExamsController : KTGKMDControllerBase
    {
        private readonly IExamAppService _examAppService;

        public ExamsController(IExamAppService examAppService)
        {
            _examAppService = examAppService;
        }

        // 1. Danh sach de thi da tao
        public async Task<IActionResult> Index()
        {
            var exams = await _examAppService.GetAllExamsAsync();
            return View(exams);
        }

        // 2. Giao dien upload file .docx tao de thi
        [HttpGet]
        public IActionResult Upload()
        {
            return View(new DocxParseResultDto { Success = true });
        }

        // Xu ly upload file .docx
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile docxFile)
        {
            if (docxFile == null || docxFile.Length == 0)
            {
                var errorResult = new DocxParseResultDto
                {
                    Success = false,
                    ValidationErrors = new System.Collections.Generic.List<string> { "Vui lòng chọn file Word (.docx) để tải lên." }
                };
                return View(errorResult);
            }

            var extension = Path.GetExtension(docxFile.FileName).ToLowerInvariant();
            if (extension != ".docx")
            {
                var errorResult = new DocxParseResultDto
                {
                    Success = false,
                    ValidationErrors = new System.Collections.Generic.List<string> { "Định dạng file không được hỗ trợ. Chỉ chấp nhận file Word định dạng .docx." }
                };
                return View(errorResult);
            }

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(docxFile.FileName);

            using (var stream = docxFile.OpenReadStream())
            {
                var parseResult = await _examAppService.ImportExamFromDocxAsync(stream, fileNameWithoutExt);

                if (!parseResult.Success || parseResult.ValidationErrors.Count > 0)
                {
                    // HỦY BỎ TOÀN BỘ KHÔNG LƯU VÀO CSDL VÀ HIỂN THỊ LỖI ĐỂ GIẢNG VIÊN SỬA FILE
                    return View(parseResult);
                }
            }

            TempData["SuccessMessage"] = "Nhập đề thi từ file Word thành công 100%!";
            return RedirectToAction(nameof(Index));
        }

        // 3. Xem truoc de thi (Preview): Ra soat loi chinh ta, hinh anh, dap an
        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            try
            {
                var examDetail = await _examAppService.GetExamDetailAsync(id);
                return View(examDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // 4. Chinh sua noi dung de thi
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var examDetail = await _examAppService.GetExamDetailAsync(id);
                return View(examDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Ajax cap nhat cau hoi
        [HttpPost]
        public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionInput input)
        {
            try
            {
                await _examAppService.UpdateQuestionAsync(input);
                return Json(new { success = true, message = "Cập nhật câu hỏi thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Xoa de thi
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _examAppService.DeleteExamAsync(id);
                TempData["SuccessMessage"] = "Đã xóa đề thi thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
