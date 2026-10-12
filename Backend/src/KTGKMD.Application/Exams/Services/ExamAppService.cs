using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using KTGKMD.Entities;
using KTGKMD.Exams.Dto;
using Microsoft.EntityFrameworkCore;

namespace KTGKMD.Exams.Services
{
    public class ExamAppService : ApplicationService, IExamAppService
    {
        private readonly IRepository<Exam, int> _examRepository;
        private readonly IRepository<Passage, int> _passageRepository;
        private readonly IRepository<Question, int> _questionRepository;
        private readonly IRepository<QuestionOption, int> _optionRepository;
        private readonly IRepository<ExamAttempt, int> _attemptRepository;
        private readonly IRepository<AttemptAnswer, int> _answerRepository;
        private readonly IDocxExamParserService _docxParser;

        public ExamAppService(
            IRepository<Exam, int> examRepository,
            IRepository<Passage, int> passageRepository,
            IRepository<Question, int> questionRepository,
            IRepository<QuestionOption, int> optionRepository,
            IRepository<ExamAttempt, int> attemptRepository,
            IRepository<AttemptAnswer, int> answerRepository,
            IDocxExamParserService docxParser)
        {
            _examRepository = examRepository;
            _passageRepository = passageRepository;
            _questionRepository = questionRepository;
            _optionRepository = optionRepository;
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
            _docxParser = docxParser;
        }

        public async Task<List<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetAll()
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new ExamDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    DurationMinutes = e.DurationMinutes,
                    TotalQuestions = e.TotalQuestions,
                    CreatedAt = e.CreatedAt,
                    IsActive = e.IsActive
                })
                .ToListAsync();

            return exams;
        }

        public async Task<ExamDetailDto> GetExamDetailAsync(int examId)
        {
            var exam = await _examRepository.GetAll()
                .Include(e => e.Passages)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
            {
                throw new UserFriendlyException("Không tìm thấy đề thi yêu cầu.");
            }

            var result = new ExamDetailDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                TotalQuestions = exam.TotalQuestions,
                CreatedAt = exam.CreatedAt,
                IsActive = exam.IsActive,
                Passages = exam.Passages
                    .OrderBy(p => p.PartNumber)
                    .ThenBy(p => p.PassageOrder)
                    .Select(p => new PassageDto
                    {
                        Id = p.Id,
                        PartNumber = p.PartNumber,
                        PassageOrder = p.PassageOrder,
                        Title = p.Title,
                        Content = p.Content,
                        ImageUrl = p.ImageUrl
                    }).ToList(),
                Questions = exam.Questions
                    .OrderBy(q => q.QuestionNumber)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        PassageId = q.PassageId,
                        PartNumber = q.PartNumber,
                        QuestionNumber = q.QuestionNumber,
                        Content = q.Content,
                        Explanation = q.Explanation,
                        Options = q.Options
                            .OrderBy(o => o.OptionLabel)
                            .Select(o => new OptionDto
                            {
                                Id = o.Id,
                                OptionLabel = o.OptionLabel,
                                Content = o.Content,
                                IsCorrect = o.IsCorrect
                            }).ToList()
                    }).ToList()
            };

            // Gán câu hỏi vào danh sách Questions của từng Passage để tiện render giao diện
            foreach (var passage in result.Passages)
            {
                passage.Questions = result.Questions.Where(q => q.PassageId == passage.Id).ToList();
            }

            return result;
        }

        public async Task<DocxParseResultDto> ImportExamFromDocxAsync(Stream docxStream, string defaultTitle)
        {
            var parseResult = _docxParser.ParseDocx(docxStream, defaultTitle);

            // NẾU SAI ĐỊNH DẠNG HOẶC CÓ LỖI: HỦY BỎ TOÀN BỘ, KHÔNG LƯU VÀO CSDL
            if (!parseResult.Success || parseResult.ValidationErrors.Count > 0)
            {
                return parseResult;
            }

            // CHỈ LƯU KHI ĐÚNG 100% CHUẨN
            var exam = new Exam
            {
                Title = parseResult.ExamTitle,
                Description = parseResult.ExamDescription,
                DurationMinutes = parseResult.DurationMinutes,
                TotalQuestions = parseResult.Questions.Count,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var examId = await _examRepository.InsertAndGetIdAsync(exam);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Lưu các Passages (cho Part 6 và 7)
            var passageIdMap = new Dictionary<int, int>(); // map từ PassageOrder sang PassageId trong DB
            foreach (var pDto in parseResult.Passages)
            {
                var passage = new Passage
                {
                    ExamId = examId,
                    PartNumber = pDto.PartNumber,
                    PassageOrder = pDto.PassageOrder,
                    Title = pDto.Title,
                    Content = pDto.Content,
                    ImageUrl = pDto.ImageUrl,
                    CreatedAt = DateTime.Now
                };

                int newPassageId = await _passageRepository.InsertAndGetIdAsync(passage);
                passageIdMap[pDto.PassageOrder] = newPassageId;
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // Lưu các Questions và Options
            foreach (var qDto in parseResult.Questions)
            {
                int? actualPassageId = null;
                if (qDto.PassageId.HasValue && passageIdMap.ContainsKey(qDto.PassageId.Value))
                {
                    actualPassageId = passageIdMap[qDto.PassageId.Value];
                }

                var question = new Question
                {
                    ExamId = examId,
                    PassageId = actualPassageId,
                    PartNumber = qDto.PartNumber,
                    QuestionNumber = qDto.QuestionNumber,
                    Content = qDto.Content,
                    Explanation = qDto.Explanation,
                    CreatedAt = DateTime.Now
                };

                int newQuestionId = await _questionRepository.InsertAndGetIdAsync(question);

                foreach (var optDto in qDto.Options)
                {
                    var option = new QuestionOption
                    {
                        QuestionId = newQuestionId,
                        OptionLabel = optDto.OptionLabel,
                        Content = optDto.Content,
                        IsCorrect = optDto.IsCorrect
                    };
                    await _optionRepository.InsertAsync(option);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return parseResult;
        }

        public async Task UpdateQuestionAsync(UpdateQuestionInput input)
        {
            var question = await _questionRepository.GetAll()
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == input.QuestionId);

            if (question == null)
            {
                throw new UserFriendlyException("Không tìm thấy câu hỏi để cập nhật.");
            }

            question.Content = input.Content;
            question.Explanation = input.Explanation;

            foreach (var optInput in input.Options)
            {
                var opt = question.Options.FirstOrDefault(o => o.Id == optInput.OptionId);
                if (opt != null)
                {
                    opt.Content = optInput.Content;
                    opt.IsCorrect = optInput.IsCorrect;
                }
            }

            await _questionRepository.UpdateAsync(question);
        }

        public async Task DeleteExamAsync(int examId)
        {
            var exam = await _examRepository.FirstOrDefaultAsync(examId);
            if (exam == null) return;

            // 1. Xoa AttemptAnswers va ExamAttempts lien quan
            var attempts = await _attemptRepository.GetAll()
                .Where(a => a.ExamId == examId)
                .ToListAsync();

            foreach (var attempt in attempts)
            {
                var answers = await _answerRepository.GetAll()
                    .Where(a => a.AttemptId == attempt.Id)
                    .ToListAsync();
                foreach (var answer in answers)
                    await _answerRepository.DeleteAsync(answer);

                await _attemptRepository.DeleteAsync(attempt);
            }

            // 2. Xoa QuestionOptions va Questions
            var questions = await _questionRepository.GetAll()
                .Where(q => q.ExamId == examId)
                .ToListAsync();

            foreach (var question in questions)
            {
                var options = await _optionRepository.GetAll()
                    .Where(o => o.QuestionId == question.Id)
                    .ToListAsync();
                foreach (var option in options)
                    await _optionRepository.DeleteAsync(option);

                await _questionRepository.DeleteAsync(question);
            }

            // 3. Xoa Passages
            var passages = await _passageRepository.GetAll()
                .Where(p => p.ExamId == examId)
                .ToListAsync();
            foreach (var passage in passages)
                await _passageRepository.DeleteAsync(passage);

            // 4. Xoa Exam
            await _examRepository.DeleteAsync(exam);
        }


        // ==============================================================
        // PHÂN HỆ MOBILE (KHÔNG CẦN LOGIN)
        // ==============================================================

        public async Task<ExamAttemptDto> StartAttemptAsync(StartAttemptInput input)
        {
            var exam = await _examRepository.FirstOrDefaultAsync(input.ExamId);
            if (exam == null)
            {
                throw new UserFriendlyException("Đề thi không tồn tại.");
            }

            var attempt = new ExamAttempt
            {
                ExamId = exam.Id,
                StartTime = DateTime.Now,
                RemainingSeconds = exam.DurationMinutes * 60,
                TotalQuestions = exam.TotalQuestions,
                Status = "InProgress",
                CreatedAt = DateTime.Now
            };

            int attemptId = await _attemptRepository.InsertAndGetIdAsync(attempt);

            return new ExamAttemptDto
            {
                AttemptId = attemptId,
                ExamId = exam.Id,
                StartTime = attempt.StartTime,
                DurationSeconds = exam.DurationMinutes * 60,
                TotalQuestions = exam.TotalQuestions,
                Status = attempt.Status
            };
        }

        public async Task SaveAnswerAsync(SaveAnswerInput input)
        {
            var attempt = await _attemptRepository.FirstOrDefaultAsync(input.AttemptId);
            if (attempt == null || attempt.Status != "InProgress")
            {
                throw new UserFriendlyException("Lượt làm bài không hợp lệ hoặc đã kết thúc.");
            }

            var existingAnswer = await _answerRepository.GetAll()
                .FirstOrDefaultAsync(a => a.AttemptId == input.AttemptId && a.QuestionId == input.QuestionId);

            int? selectedOptionId = null;
            if (!string.IsNullOrEmpty(input.SelectedLabel))
            {
                var option = await _optionRepository.GetAll()
                    .FirstOrDefaultAsync(o => o.QuestionId == input.QuestionId && o.OptionLabel == input.SelectedLabel);
                selectedOptionId = option?.Id;
            }

            if (existingAnswer == null)
            {
                var newAnswer = new AttemptAnswer
                {
                    AttemptId = input.AttemptId,
                    QuestionId = input.QuestionId,
                    SelectedLabel = input.SelectedLabel,
                    SelectedOptionId = selectedOptionId,
                    AnsweredAt = DateTime.Now
                };
                await _answerRepository.InsertAsync(newAnswer);
            }
            else
            {
                // Cho phép thay đổi hoặc hủy lựa chọn trước khi nộp
                existingAnswer.SelectedLabel = input.SelectedLabel;
                existingAnswer.SelectedOptionId = selectedOptionId;
                existingAnswer.AnsweredAt = DateTime.Now;
                await _answerRepository.UpdateAsync(existingAnswer);
            }
        }

        public async Task<SubmitExamResultDto> SubmitExamAsync(SubmitExamInput input)
        {
            var attempt = await _attemptRepository.GetAll()
                .Include(a => a.Exam)
                    .ThenInclude(e => e.Questions)
                        .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(a => a.Id == input.AttemptId);

            if (attempt == null)
            {
                throw new UserFriendlyException("Không tìm thấy lượt làm bài.");
            }

            // Đồng bộ các câu trả lời nếu gửi kèm danh sách
            if (input.Answers != null && input.Answers.Count > 0)
            {
                foreach (var ans in input.Answers)
                {
                    await SaveAnswerAsync(new SaveAnswerInput
                    {
                        AttemptId = input.AttemptId,
                        QuestionId = ans.QuestionId,
                        SelectedLabel = ans.SelectedLabel
                    });
                }
            }

            var savedAnswers = await _answerRepository.GetAll()
                .Where(a => a.AttemptId == input.AttemptId)
                .ToListAsync();

            int correctCount = 0;
            int incorrectCount = 0;
            int skippedCount = 0;

            var details = new List<QuestionResultDto>();

            foreach (var q in attempt.Exam.Questions.OrderBy(q => q.QuestionNumber))
            {
                var userAns = savedAnswers.FirstOrDefault(a => a.QuestionId == q.Id);
                var correctOpt = q.Options.FirstOrDefault(o => o.IsCorrect);
                string correctLabel = correctOpt?.OptionLabel ?? "A";

                string selected = userAns?.SelectedLabel;
                bool isCorrect = false;

                if (string.IsNullOrEmpty(selected))
                {
                    skippedCount++;
                }
                else if (selected.Equals(correctLabel, StringComparison.OrdinalIgnoreCase))
                {
                    correctCount++;
                    isCorrect = true;
                }
                else
                {
                    incorrectCount++;
                }

                if (userAns != null)
                {
                    userAns.IsCorrect = isCorrect;
                    await _answerRepository.UpdateAsync(userAns);
                }

                details.Add(new QuestionResultDto
                {
                    QuestionNumber = q.QuestionNumber,
                    PartNumber = q.PartNumber,
                    SelectedLabel = selected,
                    CorrectLabel = correctLabel,
                    IsCorrect = isCorrect,
                    Explanation = q.Explanation
                });
            }

            int score = CalculateToeicReadingScore(correctCount);

            attempt.EndTime = DateTime.Now;
            attempt.RemainingSeconds = input.RemainingSeconds;
            attempt.CorrectCount = correctCount;
            attempt.IncorrectCount = incorrectCount;
            attempt.SkippedCount = skippedCount;
            attempt.Score = score;
            attempt.Status = "Completed";

            await _attemptRepository.UpdateAsync(attempt);

            return new SubmitExamResultDto
            {
                AttemptId = attempt.Id,
                TotalQuestions = attempt.TotalQuestions,
                CorrectCount = correctCount,
                IncorrectCount = incorrectCount,
                SkippedCount = skippedCount,
                Score = score,
                SubmittedAt = attempt.EndTime.Value,
                Details = details
            };
        }

        public async Task<SubmitExamResultDto> GetAttemptResultAsync(int attemptId)
        {
            var attempt = await _attemptRepository.GetAll()
                .Include(a => a.Exam)
                    .ThenInclude(e => e.Questions)
                        .ThenInclude(q => q.Options)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null)
            {
                throw new UserFriendlyException("Không tìm thấy kết quả lượt thi.");
            }

            var details = new List<QuestionResultDto>();
            foreach (var q in attempt.Exam.Questions.OrderBy(q => q.QuestionNumber))
            {
                var userAns = attempt.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
                var correctOpt = q.Options.FirstOrDefault(o => o.IsCorrect);
                string correctLabel = correctOpt?.OptionLabel ?? "A";
                string selected = userAns?.SelectedLabel;

                details.Add(new QuestionResultDto
                {
                    QuestionNumber = q.QuestionNumber,
                    PartNumber = q.PartNumber,
                    SelectedLabel = selected,
                    CorrectLabel = correctLabel,
                    IsCorrect = userAns?.IsCorrect ?? false,
                    Explanation = q.Explanation
                });
            }

            return new SubmitExamResultDto
            {
                AttemptId = attempt.Id,
                TotalQuestions = attempt.TotalQuestions,
                CorrectCount = attempt.CorrectCount,
                IncorrectCount = attempt.IncorrectCount,
                SkippedCount = attempt.SkippedCount,
                Score = attempt.Score,
                SubmittedAt = attempt.EndTime ?? attempt.StartTime,
                Details = details
            };
        }

        private int CalculateToeicReadingScore(int correctCount)
        {
            if (correctCount <= 0) return 5;
            if (correctCount >= 100) return 495;
            // Quy đổi thang điểm TOEIC Reading chuẩn (5 - 495, bội số của 5)
            int rawScore = (int)Math.Round((double)correctCount / 100.0 * 490.0) + 5;
            int rounded = (int)Math.Round(rawScore / 5.0) * 5;
            return Math.Clamp(rounded, 5, 495);
        }
    }
}
