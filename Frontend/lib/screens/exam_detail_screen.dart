import 'package:flutter/material.dart';
import '../models/exam_model.dart';
import '../services/api_service.dart';
import 'exam_room_screen.dart';

class ExamDetailScreen extends StatefulWidget {
  final int examId;

  const ExamDetailScreen({super.key, required this.examId});

  @override
  State<ExamDetailScreen> createState() => _ExamDetailScreenState();
}

class _ExamDetailScreenState extends State<ExamDetailScreen> {
  late Future<ExamDetail> _detailFuture;
  bool _isStarting = false;

  @override
  void initState() {
    super.initState();
    _detailFuture = ApiService.getExamDetail(widget.examId);
  }

  Future<void> _startExam(ExamDetail detail) async {
    setState(() => _isStarting = true);
    try {
      final attemptId = await ApiService.startAttempt(detail.id);
      if (!mounted) return;
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => ExamRoomScreen(
            examDetail: detail,
            attemptId: attemptId,
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Lỗi khởi tạo phòng thi: $e')),
      );
    } finally {
      if (mounted) setState(() => _isStarting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFF004AC6);

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
          'Thông tin đề thi',
          style: TextStyle(color: Color(0xFF131B2E), fontSize: 16, fontWeight: FontWeight.w800),
        ),
      ),
      body: FutureBuilder<ExamDetail>(
        future: _detailFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator(color: primaryColor));
          }

          if (snapshot.hasError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Text('Lỗi tải đề thi: ${snapshot.error}', textAlign: TextAlign.center),
              ),
            );
          }

          final detail = snapshot.data!;

          return Column(
            children: [
              Expanded(
                child: ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    // Card thông tin tổng quát
                    Container(
                      padding: const EdgeInsets.all(18),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(color: Colors.black.withOpacity(0.06)),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.02),
                            blurRadius: 8,
                            offset: const Offset(0, 2),
                          ),
                        ],
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                            decoration: BoxDecoration(
                              color: primaryColor.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(20),
                            ),
                            child: const Text(
                              'FULL TEST TOEIC READING',
                              style: TextStyle(
                                color: primaryColor,
                                fontSize: 10,
                                fontWeight: FontWeight.w800,
                              ),
                            ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            detail.title,
                            style: const TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.w900,
                              color: Color(0xFF131B2E),
                            ),
                          ),
                          const SizedBox(height: 6),
                          Text(
                            detail.description ?? 'Đề thi chuẩn format TOEIC Reading.',
                            style: const TextStyle(fontSize: 13, color: Colors.black54, height: 1.4),
                          ),
                          const SizedBox(height: 16),
                          Row(
                            children: [
                              Expanded(
                                child: Container(
                                  padding: const EdgeInsets.all(12),
                                  decoration: BoxDecoration(
                                    color: const Color(0xFFF2F3FF),
                                    borderRadius: BorderRadius.circular(14),
                                  ),
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      const Row(
                                        children: [
                                          Icon(Icons.schedule, size: 14, color: primaryColor),
                                          SizedBox(width: 4),
                                          Text('Thời gian', style: TextStyle(fontSize: 11, color: Colors.black54)),
                                        ],
                                      ),
                                      const SizedBox(height: 4),
                                      Text(
                                        '${detail.durationMinutes} phút',
                                        style: const TextStyle(fontSize: 15, fontWeight: FontWeight.w900),
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                              const SizedBox(width: 10),
                              Expanded(
                                child: Container(
                                  padding: const EdgeInsets.all(12),
                                  decoration: BoxDecoration(
                                    color: const Color(0xFFF2F3FF),
                                    borderRadius: BorderRadius.circular(14),
                                  ),
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      const Row(
                                        children: [
                                          Icon(Icons.quiz, size: 14, color: primaryColor),
                                          SizedBox(width: 4),
                                          Text('Số câu hỏi', style: TextStyle(fontSize: 11, color: Colors.black54)),
                                        ],
                                      ),
                                      const SizedBox(height: 4),
                                      Text(
                                        '${detail.questions.length} câu',
                                        style: const TextStyle(fontSize: 15, fontWeight: FontWeight.w900),
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),

                    const SizedBox(height: 16),

                    // Cấu trúc phân cấp chuẩn TOEIC Reading
                    Container(
                      padding: const EdgeInsets.all(18),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(color: Colors.black.withOpacity(0.06)),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'CẤU TRÚC ĐỀ THI READING',
                            style: TextStyle(
                              fontSize: 11,
                              fontWeight: FontWeight.w800,
                              color: Colors.grey,
                              letterSpacing: 0.5,
                            ),
                          ),
                          const SizedBox(height: 12),
                          _buildPartRow(
                            'Part 5: Incomplete Sentences',
                            '30 câu (101 - 130)',
                            const Color(0xFF2563EB),
                          ),
                          const Divider(height: 16),
                          _buildPartRow(
                            'Part 6: Text Completion',
                            '16 câu (131 - 146)',
                            const Color(0xFF4F46E5),
                          ),
                          const Divider(height: 16),
                          _buildPartRow(
                            'Part 7: Reading Comprehension',
                            '54 câu (147 - 200)',
                            const Color(0xFF059669),
                          ),
                        ],
                      ),
                    ),

                    const SizedBox(height: 16),

                    // Quy chế phòng thi
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: const Color(0xFFFFFBEB),
                        borderRadius: BorderRadius.circular(20),
                        border: Border.all(color: const Color(0xFFFDE68A)),
                      ),
                      child: Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Icon(Icons.info_outline, color: Color(0xFFD97706), size: 20),
                          const SizedBox(width: 10),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: const [
                                Text(
                                  'Quy chế tính giờ & Nộp bài',
                                  style: TextStyle(
                                    fontWeight: FontWeight.w800,
                                    fontSize: 13,
                                    color: Color(0xFF92400E),
                                  ),
                                ),
                                SizedBox(height: 4),
                                Text(
                                  'Đồng hồ đếm ngược 75 phút sẽ kích hoạt ngay khi bạn bấm nút bắt đầu. Hệ thống tự động thu bài và khóa quyền chọn khi hết thời gian thi.',
                                  style: TextStyle(fontSize: 11.5, color: Color(0xFFB45309), height: 1.4),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),

              // Bottom Action Bar
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.white,
                  border: Border(top: BorderSide(color: Colors.black.withOpacity(0.06))),
                ),
                child: SafeArea(
                  child: SizedBox(
                    width: double.infinity,
                    height: 52,
                    child: ElevatedButton.icon(
                      onPressed: _isStarting ? null : () => _startExam(detail),
                      icon: _isStarting
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                            )
                          : const Icon(Icons.play_arrow),
                      label: Text(
                        _isStarting ? 'Đang chuẩn bị phòng thi...' : 'Bắt đầu làm bài (${detail.durationMinutes} phút)',
                        style: const TextStyle(fontWeight: FontWeight.w800, fontSize: 14),
                      ),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: primaryColor,
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
                        elevation: 0,
                      ),
                    ),
                  ),
                ),
              ),
            ],
          );
        },
      ),
    );
  }

  Widget _buildPartRow(String title, String count, Color color) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Row(
          children: [
            Container(width: 8, height: 8, decoration: BoxDecoration(color: color, shape: BoxShape.circle)),
            const SizedBox(width: 8),
            Text(title, style: const TextStyle(fontSize: 12.5, fontWeight: FontWeight.w700)),
          ],
        ),
        Text(count, style: const TextStyle(fontSize: 12, color: Colors.black54, fontFamily: 'monospace')),
      ],
    );
  }
}
