using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KTGKMD.Exams.Dto;

namespace KTGKMD.Exams.Services
{
    public class DocxExamParserService : IDocxExamParserService
    {
        public DocxParseResultDto ParseDocx(Stream docxStream, string defaultTitle)
        {
            var result = new DocxParseResultDto
            {
                ExamTitle = defaultTitle,
                ExamDescription = "TOEIC Reading Test imported from Word document",
                DurationMinutes = 75,
                Success = true
            };

            var lines = ExtractLinesFromDocx(docxStream);
            if (lines.Count == 0)
            {
                result.Success = false;
                result.ValidationErrors.Add("File Word không có nội dung hoặc không thể đọc được văn bản.");
                return result;
            }

            ParseLines(lines, result);

            // Kiểm tra tổng quát nếu không có lỗi cú pháp
            if (result.Questions.Count == 0)
            {
                result.Success = false;
                result.ValidationErrors.Add("Không tìm thấy câu hỏi nào trong file Word. Vui lòng đảm bảo các câu hỏi có thẻ [Q: <số câu>].");
            }
            else
            {
                // Kiểm tra trùng lặp số câu hỏi
                var duplicateQuestions = result.Questions
                    .GroupBy(q => q.QuestionNumber)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                foreach (var dup in duplicateQuestions)
                {
                    result.ValidationErrors.Add($"Trùng lặp số thứ tự câu hỏi: Câu {dup} xuất hiện nhiều hơn 1 lần.");
                }

                // Sắp xếp các câu hỏi theo thứ tự số câu tăng dần
                result.Questions = result.Questions.OrderBy(q => q.QuestionNumber).ToList();
            }

            if (result.ValidationErrors.Count > 0)
            {
                result.Success = false;
            }

            return result;
        }

        private List<string> ExtractLinesFromDocx(Stream docxStream)
        {
            var lines = new List<string>();
            try
            {
                using (var doc = WordprocessingDocument.Open(docxStream, false))
                {
                    var body = doc.MainDocumentPart?.Document?.Body;
                    if (body != null)
                    {
                        foreach (var paragraph in body.Descendants<Paragraph>())
                        {
                            var sb = new StringBuilder();
                            foreach (var text in paragraph.Descendants<Text>())
                            {
                                sb.Append(text.Text);
                            }
                            var line = sb.ToString().Trim();
                            if (!string.IsNullOrEmpty(line))
                            {
                                lines.Add(line);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lines.Clear();
            }
            return lines;
        }

        private void ParseLines(List<string> lines, DocxParseResultDto result)
        {
            int currentPart = 5;
            PassageDto currentPassage = null;
            QuestionDto currentQuestion = null;
            string currentTag = null; // "PASSAGE", "A", "B", "C", "D", "EXPLANATION"
            var passageTextBuilder = new StringBuilder();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Thẻ tiêu đề đề thi: [EXAM_TITLE: ...]
                if (line.StartsWith("[EXAM_TITLE:", StringComparison.OrdinalIgnoreCase))
                {
                    result.ExamTitle = ExtractTagValue(line, "[EXAM_TITLE:").Trim();
                    continue;
                }

                // Thẻ mô tả đề thi: [EXAM_DESC: ...]
                if (line.StartsWith("[EXAM_DESC:", StringComparison.OrdinalIgnoreCase))
                {
                    result.ExamDescription = ExtractTagValue(line, "[EXAM_DESC:").Trim();
                    continue;
                }

                // Thẻ thời gian thi: [DURATION: ...]
                if (line.StartsWith("[DURATION:", StringComparison.OrdinalIgnoreCase))
                {
                    var durStr = ExtractTagValue(line, "[DURATION:").Trim();
                    if (int.TryParse(durStr, out int dur) && dur > 0)
                    {
                        result.DurationMinutes = dur;
                    }
                    continue;
                }

                // Thẻ phần thi: [PART: 5], [PART: 6], [PART: 7]
                if (line.StartsWith("[PART:", StringComparison.OrdinalIgnoreCase))
                {
                    ValidateAndCompleteCurrentQuestion(currentQuestion, result);
                    currentQuestion = null;
                    currentPassage = null;

                    var partStr = ExtractTagValue(line, "[PART:").Trim();
                    if (int.TryParse(partStr, out int p) && (p == 5 || p == 6 || p == 7))
                    {
                        currentPart = p;
                    }
                    else
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Phần thi không hợp lệ '{line}'. Phải là [PART: 5], [PART: 6], hoặc [PART: 7].");
                    }
                    continue;
                }

                // Thẻ tiêu đề bài đọc: [PASSAGE_TITLE: ...]
                if (line.StartsWith("[PASSAGE_TITLE:", StringComparison.OrdinalIgnoreCase))
                {
                    ValidateAndCompleteCurrentQuestion(currentQuestion, result);
                    currentQuestion = null;

                    if (currentPart != 6 && currentPart != 7)
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Thẻ [PASSAGE_TITLE] chỉ hợp lệ trong Part 6 hoặc Part 7.");
                    }

                    currentPassage = new PassageDto
                    {
                        PartNumber = currentPart,
                        PassageOrder = result.Passages.Count + 1,
                        Title = ExtractTagValue(line, "[PASSAGE_TITLE:").Trim()
                    };
                    result.Passages.Add(currentPassage);
                    continue;
                }

                // Thẻ ảnh bài đọc: [IMAGE: ...]
                if (line.StartsWith("[IMAGE:", StringComparison.OrdinalIgnoreCase))
                {
                    var imgUrl = ExtractTagValue(line, "[IMAGE:").Trim();
                    if (currentPassage != null)
                    {
                        currentPassage.ImageUrl = imgUrl;
                    }
                    continue;
                }

                // Thẻ bắt đầu bài đọc: [PASSAGE_START]
                if (line.StartsWith("[PASSAGE_START]", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentPassage == null)
                    {
                        currentPassage = new PassageDto
                        {
                            PartNumber = currentPart,
                            PassageOrder = result.Passages.Count + 1,
                            Title = $"Passage {result.Passages.Count + 1}"
                        };
                        result.Passages.Add(currentPassage);
                    }
                    currentTag = "PASSAGE";
                    passageTextBuilder.Clear();
                    continue;
                }

                // Thẻ kết thúc bài đọc: [PASSAGE_END]
                if (line.StartsWith("[PASSAGE_END]", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentPassage != null)
                    {
                        currentPassage.Content = passageTextBuilder.ToString().Trim();
                    }
                    currentTag = null;
                    continue;
                }

                // Nếu đang trong vùng bài đọc
                if (currentTag == "PASSAGE")
                {
                    if (passageTextBuilder.Length > 0) passageTextBuilder.AppendLine();
                    passageTextBuilder.Append(line);
                    continue;
                }

                // Thẻ câu hỏi: [Q: 101] hoặc [Q: 101] Content...
                var matchQ = Regex.Match(line, @"^\[Q:\s*(\d+)\](.*)$", RegexOptions.IgnoreCase);
                if (matchQ.Success)
                {
                    ValidateAndCompleteCurrentQuestion(currentQuestion, result);

                    int qNum = int.Parse(matchQ.Groups[1].Value);
                    string qContent = matchQ.Groups[2].Value.Trim();

                    currentQuestion = new QuestionDto
                    {
                        QuestionNumber = qNum,
                        PartNumber = currentPart,
                        Content = qContent
                    };

                    if (currentPart == 6 || currentPart == 7)
                    {
                        if (currentPassage == null)
                        {
                            result.ValidationErrors.Add($"Câu {qNum} (Part {currentPart}) thiếu bài đọc kèm theo ([PASSAGE_START] ... [PASSAGE_END]).");
                        }
                        else
                        {
                            currentQuestion.PassageId = currentPassage.PassageOrder; // Tạm dùng index để link
                            currentPassage.Questions.Add(currentQuestion);
                        }
                    }

                    result.Questions.Add(currentQuestion);
                    currentTag = "Q";
                    continue;
                }

                // Thẻ đáp án [A]
                if (line.StartsWith("[A]", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion == null)
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Gặp đáp án [A] nhưng chưa có thẻ câu hỏi [Q: ...].");
                        continue;
                    }
                    var optContent = line.Substring(3).Trim();
                    AddOrUpdateOption(currentQuestion, "A", optContent);
                    currentTag = "A";
                    continue;
                }

                // Thẻ đáp án [B]
                if (line.StartsWith("[B]", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion == null)
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Gặp đáp án [B] nhưng chưa có thẻ câu hỏi [Q: ...].");
                        continue;
                    }
                    var optContent = line.Substring(3).Trim();
                    AddOrUpdateOption(currentQuestion, "B", optContent);
                    currentTag = "B";
                    continue;
                }

                // Thẻ đáp án [C]
                if (line.StartsWith("[C]", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion == null)
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Gặp đáp án [C] nhưng chưa có thẻ câu hỏi [Q: ...].");
                        continue;
                    }
                    var optContent = line.Substring(3).Trim();
                    AddOrUpdateOption(currentQuestion, "C", optContent);
                    currentTag = "C";
                    continue;
                }

                // Thẻ đáp án [D]
                if (line.StartsWith("[D]", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion == null)
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Gặp đáp án [D] nhưng chưa có thẻ câu hỏi [Q: ...].");
                        continue;
                    }
                    var optContent = line.Substring(3).Trim();
                    AddOrUpdateOption(currentQuestion, "D", optContent);
                    currentTag = "D";
                    continue;
                }

                // Thẻ đáp án đúng: [KEY: A|B|C|D]
                if (line.StartsWith("[KEY:", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion == null)
                    {
                        result.ValidationErrors.Add($"Dòng {i + 1}: Gặp thẻ [KEY:] nhưng chưa có thẻ câu hỏi [Q: ...].");
                        continue;
                    }
                    var keyVal = ExtractTagValue(line, "[KEY:").Trim().ToUpperInvariant();
                    if (keyVal != "A" && keyVal != "B" && keyVal != "C" && keyVal != "D")
                    {
                        result.ValidationErrors.Add($"Câu {currentQuestion.QuestionNumber}: Đáp án đúng '[KEY: {keyVal}]' không hợp lệ. Phải là A, B, C hoặc D.");
                    }
                    else
                    {
                        var opt = currentQuestion.Options.FirstOrDefault(o => o.OptionLabel == keyVal);
                        if (opt != null)
                        {
                            opt.IsCorrect = true;
                        }
                    }
                    currentTag = null;
                    continue;
                }

                // Thẻ giải thích: [EXPLANATION: ...]
                if (line.StartsWith("[EXPLANATION:", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion != null)
                    {
                        currentQuestion.Explanation = ExtractTagValue(line, "[EXPLANATION:").Trim();
                    }
                    currentTag = null;
                    continue;
                }

                // Nối nội dung mở rộng của thẻ đang đọc nếu dòng tiếp theo không có tag
                if (currentTag == "Q" && currentQuestion != null)
                {
                    currentQuestion.Content = string.IsNullOrEmpty(currentQuestion.Content)
                        ? line
                        : currentQuestion.Content + " " + line;
                }
                else if (currentTag == "A" || currentTag == "B" || currentTag == "C" || currentTag == "D")
                {
                    var opt = currentQuestion?.Options.FirstOrDefault(o => o.OptionLabel == currentTag);
                    if (opt != null)
                    {
                        opt.Content += " " + line;
                    }
                }
            }

            // Kiểm tra câu hỏi cuối cùng
            ValidateAndCompleteCurrentQuestion(currentQuestion, result);
        }

        private void AddOrUpdateOption(QuestionDto question, string label, string content)
        {
            var opt = question.Options.FirstOrDefault(o => o.OptionLabel == label);
            if (opt == null)
            {
                question.Options.Add(new OptionDto
                {
                    OptionLabel = label,
                    Content = content,
                    IsCorrect = false
                });
            }
            else
            {
                opt.Content = content;
            }
        }

        private void ValidateAndCompleteCurrentQuestion(QuestionDto question, DocxParseResultDto result)
        {
            if (question == null) return;

            // Kiểm tra đủ 4 phương án A, B, C, D
            string[] requiredLabels = { "A", "B", "C", "D" };
            foreach (var label in requiredLabels)
            {
                var opt = question.Options.FirstOrDefault(o => o.OptionLabel == label);
                if (opt == null || string.IsNullOrWhiteSpace(opt.Content))
                {
                    result.ValidationErrors.Add($"Câu {question.QuestionNumber}: Thiếu nội dung đáp án [{label}].");
                }
            }

            // Kiểm tra có đúng 1 đáp án đúng
            int correctCount = question.Options.Count(o => o.IsCorrect);
            if (correctCount == 0)
            {
                result.ValidationErrors.Add($"Câu {question.QuestionNumber}: Chưa chỉ định đáp án đúng qua thẻ [KEY: <A|B|C|D>].");
            }
            else if (correctCount > 1)
            {
                result.ValidationErrors.Add($"Câu {question.QuestionNumber}: Có nhiều hơn 1 đáp án đúng.");
            }
        }

        private string ExtractTagValue(string line, string tagPrefix)
        {
            int startIdx = tagPrefix.Length;
            int endIdx = line.LastIndexOf(']');
            if (endIdx > startIdx)
            {
                return line.Substring(startIdx, endIdx - startIdx);
            }
            return line.Substring(startIdx);
        }
    }
}
