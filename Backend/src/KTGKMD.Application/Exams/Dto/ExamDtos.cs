using System;
using System.Collections.Generic;

namespace KTGKMD.Exams.Dto
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class ExamDetailDto : ExamDto
    {
        public List<PassageDto> Passages { get; set; } = new List<PassageDto>();
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    public class PassageDto
    {
        public int Id { get; set; }
        public int PartNumber { get; set; }
        public int PassageOrder { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public int? PassageId { get; set; }
        public int PartNumber { get; set; }
        public int QuestionNumber { get; set; }
        public string Content { get; set; }
        public string Explanation { get; set; }
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }

    public class OptionDto
    {
        public int Id { get; set; }
        public string OptionLabel { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class UpdateQuestionInput
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public string Explanation { get; set; }
        public List<UpdateOptionInput> Options { get; set; } = new List<UpdateOptionInput>();
    }

    public class UpdateOptionInput
    {
        public int OptionId { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class StartAttemptInput
    {
        public int ExamId { get; set; }
    }

    public class ExamAttemptDto
    {
        public int AttemptId { get; set; }
        public int ExamId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationSeconds { get; set; }
        public int TotalQuestions { get; set; }
        public string Status { get; set; }
    }

    public class SaveAnswerInput
    {
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public string SelectedLabel { get; set; } // 'A', 'B', 'C', 'D' hoac null neu huy chon
    }

    public class SubmitExamInput
    {
        public int AttemptId { get; set; }
        public int RemainingSeconds { get; set; }
        public List<SubmitAnswerItem> Answers { get; set; } = new List<SubmitAnswerItem>();
    }

    public class SubmitAnswerItem
    {
        public int QuestionId { get; set; }
        public string SelectedLabel { get; set; }
    }

    public class SubmitExamResultDto
    {
        public int AttemptId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }
        public int SkippedCount { get; set; }
        public int Score { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResultDto> Details { get; set; } = new List<QuestionResultDto>();
    }

    public class QuestionResultDto
    {
        public int QuestionNumber { get; set; }
        public int PartNumber { get; set; }
        public string SelectedLabel { get; set; }
        public string CorrectLabel { get; set; }
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; }
    }

    public class DocxParseResultDto
    {
        public bool Success { get; set; }
        public string ExamTitle { get; set; }
        public string ExamDescription { get; set; }
        public int DurationMinutes { get; set; } = 75;
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public List<PassageDto> Passages { get; set; } = new List<PassageDto>();
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
