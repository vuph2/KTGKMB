import 'dart:math' as math;
import 'package:flutter/material.dart';
import '../models/exam_model.dart';
import 'exam_review_screen.dart';

class ExamResultScreen extends StatelessWidget {
  final SubmitResult result;
  final String examTitle;

  const ExamResultScreen({
    super.key,
    required this.result,
    required this.examTitle,
  });

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFF004AC6);

    String feedback = "Bạn đã hoàn thành bài thi TOEIC Reading.";
    if (result.score >= 400) {
      feedback = "Xuất sắc! Trình độ Reading của bạn đạt mức nâng cao.";
    } else if (result.score >= 300) {
      feedback = "Khá tốt! Bạn có nền tảng ngữ pháp và từ vựng ổn định.";
    } else {
      feedback = "Cần cố gắng thêm để cải thiện từ vựng và tốc độ đọc hiểu.";
    }

    return Scaffold(
      backgroundColor: const Color(0xFFFAF8FF),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0.5,
        automaticallyImplyLeading: false,
        title: const Text(
          'Kết quả bài thi',
          style: TextStyle(color: Color(0xFF131B2E), fontSize: 16, fontWeight: FontWeight.w800),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.home, color: primaryColor),
            onPressed: () => Navigator.popUntil(context, (route) => route.isFirst),
          ),
        ],
      ),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          // Score Card Vòng tròn lớn
          Container(
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(24),
              border: Border.all(color: Colors.black.withOpacity(0.06)),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.02),
                  blurRadius: 10,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: Column(
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                  decoration: BoxDecoration(
                    color: const Color(0xFF7CF994).withOpacity(0.3),
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: const Text(
                    'HOÀN THÀNH BÀI THI',
                    style: TextStyle(
                      color: Color(0xFF007230),
                      fontSize: 11,
                      fontWeight: FontWeight.w800,
                      letterSpacing: 0.5,
                    ),
                  ),
                ),
                const SizedBox(height: 16),

                // Vòng tròn điểm hoạt họa (Animated Circular Progress Gauge)
                TweenAnimationBuilder<double>(
                  tween: Tween<double>(begin: 0.0, end: (result.score / 495.0).clamp(0.0, 1.0)),
                  duration: const Duration(milliseconds: 1400),
                  curve: Curves.easeOutCubic,
                  builder: (context, value, child) {
                    final currentDisplayScore = (value * result.score).round();
                    return SizedBox(
                      width: 160,
                      height: 160,
                      child: Stack(
                        alignment: Alignment.center,
                        children: [
                          CustomPaint(
                            size: const Size(160, 160),
                            painter: _ScoreGaugePainter(progress: value),
                          ),
                          Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Text(
                                '$currentDisplayScore',
                                style: const TextStyle(
                                  fontSize: 36,
                                  fontWeight: FontWeight.w900,
                                  color: primaryColor,
                                  fontFamily: 'monospace',
                                  height: 1,
                                ),
                              ),
                              const SizedBox(height: 4),
                              const Text(
                                '/ 495 ĐIỂM',
                                style: TextStyle(
                                  fontSize: 10,
                                  color: Colors.grey,
                                  fontWeight: FontWeight.bold,
                                  letterSpacing: 0.5,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    );
                  },
                ),

                const SizedBox(height: 16),

                // Badge trình độ chuẩn Stitch
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 6),
                  decoration: BoxDecoration(
                    color: const Color(0xFFEAEDFF),
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Icon(Icons.verified, size: 16, color: primaryColor),
                      const SizedBox(width: 6),
                      Text(
                        result.score >= 400
                            ? 'Trình độ C1 • Xuất sắc'
                            : (result.score >= 300
                                ? 'Trình độ B2 • Khá tốt'
                                : (result.score >= 200
                                    ? 'Trình độ B1 • Cơ bản'
                                    : 'Trình độ A2 • Cần cố gắng')),
                        style: const TextStyle(
                          color: primaryColor,
                          fontSize: 12,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 12),
                Text(
                  examTitle,
                  textAlign: TextAlign.center,
                  style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 6),
                Text(
                  feedback,
                  textAlign: TextAlign.center,
                  style: const TextStyle(fontSize: 12, color: Colors.black54),
                ),
              ],
            ),
          ),

          const SizedBox(height: 16),

          // 3 Thẻ thống kê: Đúng, Sai, Bỏ qua
          Row(
            children: [
              Expanded(
                child: Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: const Color(0xFFECFDF5),
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: const Color(0xFFA7F3D0)),
                  ),
                  child: Column(
                    children: [
                      const Icon(Icons.check_circle, color: Color(0xFF059669), size: 22),
                      const SizedBox(height: 4),
                      Text(
                        '${result.correctCount}',
                        style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w900, color: Color(0xFF065F46)),
                      ),
                      const Text('Câu đúng', style: TextStyle(fontSize: 10, color: Color(0xFF047857), fontWeight: FontWeight.bold)),
                    ],
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: const Color(0xFFFFF1F2),
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: const Color(0xFFFECDD3)),
                  ),
                  child: Column(
                    children: [
                      const Icon(Icons.cancel, color: Color(0xFFE11D48), size: 22),
                      const SizedBox(height: 4),
                      Text(
                        '${result.incorrectCount}',
                        style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w900, color: Color(0xFF9F1239)),
                      ),
                      const Text('Câu sai', style: TextStyle(fontSize: 10, color: Color(0xFFBE123C), fontWeight: FontWeight.bold)),
                    ],
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: const Color(0xFFF3F4F6),
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: const Color(0xFFE5E7EB)),
                  ),
                  child: Column(
                    children: [
                      const Icon(Icons.help, color: Color(0xFF6B7280), size: 22),
                      const SizedBox(height: 4),
                      Text(
                        '${result.skippedCount}',
                        style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w900, color: Color(0xFF374151)),
                      ),
                      const Text('Bỏ qua', style: TextStyle(fontSize: 10, color: Color(0xFF4B5563), fontWeight: FontWeight.bold)),
                    ],
                  ),
                ),
              ),
            ],
          ),

          const SizedBox(height: 20),

          // Nút xem chi tiết đáp án & giải thích
          OutlinedButton.icon(
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => ExamReviewScreen(details: result.details),
                ),
              );
            },
            icon: const Icon(Icons.rule, color: primaryColor),
            label: const Text(
              'Xem chi tiết đáp án & giải thích',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13, color: primaryColor),
            ),
            style: OutlinedButton.styleFrom(
              padding: const EdgeInsets.symmetric(vertical: 14),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
              side: const BorderSide(color: primaryColor),
            ),
          ),

          const SizedBox(height: 10),

          // Nút chọn đề khác
          ElevatedButton(
            onPressed: () => Navigator.popUntil(context, (route) => route.isFirst),
            style: ElevatedButton.styleFrom(
              backgroundColor: primaryColor,
              foregroundColor: Colors.white,
              padding: const EdgeInsets.symmetric(vertical: 14),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
            ),
            child: const Text('Chọn đề thi khác', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
          ),
        ],
      ),
    );
  }
}

class _ScoreGaugePainter extends CustomPainter {
  final double progress;

  _ScoreGaugePainter({required this.progress});

  @override
  void paint(Canvas canvas, Size size) {
    final center = Offset(size.width / 2, size.height / 2);
    final radius = (size.width - 24) / 2;

    // 1. Vòng tròn nền (Background Track)
    final bgPaint = Paint()
      ..color = const Color(0xFFEAEDFF)
      ..strokeWidth = 12
      ..style = PaintingStyle.stroke
      ..strokeCap = StrokeCap.round;

    canvas.drawCircle(center, radius, bgPaint);

    if (progress <= 0) return;

    // 2. Vòng tròn tiến độ Gradient (Progress Arc)
    final rect = Rect.fromCircle(center: center, radius: radius);
    final gradient = const SweepGradient(
      startAngle: -math.pi / 2,
      endAngle: 3 * math.pi / 2,
      colors: [Color(0xFF2563EB), Color(0xFF004AC6)],
    );

    final progressPaint = Paint()
      ..shader = gradient.createShader(rect)
      ..strokeWidth = 12
      ..style = PaintingStyle.stroke
      ..strokeCap = StrokeCap.round;

    final sweepAngle = 2 * math.pi * progress;
    canvas.drawArc(
      rect,
      -math.pi / 2, // Bắt đầu từ 12 giờ
      sweepAngle,
      false,
      progressPaint,
    );
  }

  @override
  bool shouldRepaint(covariant _ScoreGaugePainter oldDelegate) {
    return oldDelegate.progress != progress;
  }
}
