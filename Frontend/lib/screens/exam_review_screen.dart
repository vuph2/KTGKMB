import 'package:flutter/material.dart';
import '../models/exam_model.dart';

class ExamReviewScreen extends StatelessWidget {
  final List<QuestionResultDetail> details;

  const ExamReviewScreen({super.key, required this.details});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFFAF8FF),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Color(0xFF131B2E)),
          onPressed: () => Navigator.pop(context),
        ),
        title: const Text(
          'Chi tiết câu trả lời',
          style: TextStyle(color: Color(0xFF131B2E), fontSize: 16, fontWeight: FontWeight.w800),
        ),
      ),
      body: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: details.length,
        itemBuilder: (context, idx) {
          final d = details[idx];

          Color badgeBg = const Color(0xFFECFDF5);
          Color badgeText = const Color(0xFF065F46);
          String statusTitle = 'ĐÚNG';

          if (d.selectedLabel == null || d.selectedLabel!.isEmpty) {
            badgeBg = const Color(0xFFF3F4F6);
            badgeText = const Color(0xFF374151);
            statusTitle = 'BỎ QUA';
          } else if (!d.isCorrect) {
            badgeBg = const Color(0xFFFFF1F2);
            badgeText = const Color(0xFF9F1239);
            statusTitle = 'SAI';
          }

          return Container(
            margin: const EdgeInsets.only(bottom: 12),
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: Colors.black.withOpacity(0.06)),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Câu ${d.questionNumber} (Part ${d.partNumber})',
                      style: const TextStyle(fontWeight: FontWeight.w800, fontSize: 14),
                    ),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                      decoration: BoxDecoration(
                        color: badgeBg,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        statusTitle,
                        style: TextStyle(color: badgeText, fontSize: 10, fontWeight: FontWeight.bold),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                Row(
                  children: [
                    Text(
                      'Bạn chọn: ${d.selectedLabel ?? "Chưa chọn"}',
                      style: TextStyle(
                        fontSize: 12.5,
                        fontWeight: FontWeight.bold,
                        color: d.isCorrect
                            ? const Color(0xFF059669)
                            : (d.selectedLabel != null ? const Color(0xFFE11D48) : Colors.grey),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Text(
                      '•   Đáp án đúng: ${d.correctLabel}',
                      style: const TextStyle(
                        fontSize: 12.5,
                        fontWeight: FontWeight.bold,
                        color: Color(0xFF059669),
                      ),
                    ),
                  ],
                ),
                if (d.explanation != null && d.explanation!.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: const Color(0xFFF2F3FF),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Text(
                      '💡 Giải thích: ${d.explanation}',
                      style: const TextStyle(fontSize: 11.5, color: Colors.black87, height: 1.35),
                    ),
                  ),
                ],
              ],
            ),
          );
        },
      ),
    );
  }
}
