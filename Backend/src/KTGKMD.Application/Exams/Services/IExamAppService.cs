using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using KTGKMD.Exams.Dto;

namespace KTGKMD.Exams.Services
{
    public interface IExamAppService : IApplicationService
    {
        Task<List<ExamDto>> GetAllExamsAsync();
        Task<ExamDetailDto> GetExamDetailAsync(int examId);
        Task<DocxParseResultDto> ImportExamFromDocxAsync(Stream docxStream, string defaultTitle);
        Task UpdateQuestionAsync(UpdateQuestionInput input);
        Task DeleteExamAsync(int examId);

        // API cho Mobile (Không cần login)
        Task<ExamAttemptDto> StartAttemptAsync(StartAttemptInput input);
        Task SaveAnswerAsync(SaveAnswerInput input);
        Task<SubmitExamResultDto> SubmitExamAsync(SubmitExamInput input);
        Task<SubmitExamResultDto> GetAttemptResultAsync(int attemptId);
    }
}
