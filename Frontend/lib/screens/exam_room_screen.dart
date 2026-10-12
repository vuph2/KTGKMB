import 'dart:async';
import 'package:flutter/material.dart';
import '../models/exam_model.dart';
import '../services/api_service.dart';
import 'exam_result_screen.dart';

class ExamRoomScreen extends StatefulWidget {
  final ExamDetail examDetail;
  final int attemptId;

  const ExamRoomScreen({
    super.key,
    required this.examDetail,
    required this.attemptId,
  });

  @override
  State<ExamRoomScreen> createState() => _ExamRoomScreenState();
}

class _ExamRoomScreenState extends State<ExamRoomScreen> {
  late List<Question> _questions;
  int _currentIndex = 0;
  final Map<int, String> _userAnswers = {}; // { questionId: 'A' | 'B' | 'C' | 'D' }

  Timer? _timer;
  late int _remainingSeconds;
  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    _questions = widget.examDetail.questions..sort((a, b) => a.questionNumber.compareTo(b.questionNumber));
    _remainingSeconds = widget.examDetail.durationMinutes * 60;
    _startTimer();
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  void _startTimer() {
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (_remainingSeconds > 0) {
        setState(() => _remainingSeconds--);
      } else {
        _timer?.cancel();
        _onTimeOut();
      }
    });
  }

  void _onTimeOut() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => AlertDialog(
        title: const Row(
          children: [
            Icon(Icons.timer_off, color: Color(0xFFBA1A1A)),
            SizedBox(width: 8),
            Text('Hết giờ làm bài!', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
          ],
        ),
        content: const Text(
          'Đã hết thời gian 75 phút quy định. Hệ thống sẽ tự động thu bài và chấm điểm của bạn.',
          style: TextStyle(fontSize: 13),
        ),
        actions: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(context);
              _submitExam();
            },
            style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFF004AC6)),
            child: const Text('Xem kết quả', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  void _selectOption(int questionId, String label) {
    setState(() {
      _userAnswers[questionId] = label;
    });
    // Auto-save lên backend API
    ApiService.saveAnswer(widget.attemptId, questionId, label);
  }

  void _clearAnswer(int questionId) {
    setState(() {
      _userAnswers.remove(questionId);
    });
    ApiService.saveAnswer(widget.attemptId, questionId, null);
  }

  Future<void> _submitExam() async {
    if (_isSubmitting) return;
    setState(() => _isSubmitting = true);
    _timer?.cancel();

    try {
      final result = await ApiService.submitExam(
        widget.attemptId,
        _remainingSeconds,
        _userAnswers,
      );

      if (!mounted) return;
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => ExamResultScreen(
            result: result,
            examTitle: widget.examDetail.title,
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Lỗi nộp bài: $e')),
      );
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  void _openConfirmDialog() {
    final total = _questions.length;
    final answered = _userAnswers.length;
    final unanswered = total - answered;

    final m = _remainingSeconds ~/ 60;
    final s = _remainingSeconds % 60;
    final timeStr = '${m.toString().padLeft(2, '0')}:${s.toString().padLeft(2, '0')}';

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(24)),
        title: Column(
          children: [
            Container(
              width: 52,
              height: 52,
              decoration: const BoxDecoration(
                color: Color(0xFFFEF3C7),
                shape: BoxShape.circle,
              ),
              child: const Icon(Icons.assignment_turned_in, color: Color(0xFFD97706), size: 28),
            ),
            const SizedBox(height: 12),
            const Text(
              'Xác nhận nộp bài?',
              style: TextStyle(fontWeight: FontWeight.w900, fontSize: 18),
            ),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              'Bạn còn $unanswered câu chưa hoàn thành.',
              textAlign: TextAlign.center,
              style: TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.bold,
                color: unanswered > 0 ? const Color(0xFFBA1A1A) : const Color(0xFF007230),
              ),
            ),
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: const Color(0xFFF2F3FF),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Đã trả lời:', style: TextStyle(fontSize: 12, color: Colors.black54)),
                      Text('$answered/$total', style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Thời gian còn lại:', style: TextStyle(fontSize: 12, color: Colors.black54)),
                      Text(timeStr, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Color(0xFF004AC6))),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
        actions: [
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: () => Navigator.pop(ctx),
                  style: OutlinedButton.styleFrom(
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                  child: const Text('Làm tiếp', style: TextStyle(fontWeight: FontWeight.bold)),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.pop(ctx);
                    _submitExam();
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF004AC6),
                    foregroundColor: Colors.white,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                  child: const Text('Nộp luôn', style: TextStyle(fontWeight: FontWeight.bold)),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  void _openPaletteSheet() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => Container(
        height: MediaQuery.of(context).size.height * 0.75,
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(28)),
        ),
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: const [
                    Text('Bảng câu hỏi (101 - 200)', style: TextStyle(fontSize: 16, fontWeight: FontWeight.w900)),
                    Text('Chạm vào số câu để chuyển đến câu hỏi đó', style: TextStyle(fontSize: 11, color: Colors.grey)),
                  ],
                ),
                IconButton(
                  icon: const Icon(Icons.close),
                  onPressed: () => Navigator.pop(ctx),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                _buildLegendItem(const Color(0xFF004AC6), 'Đã làm'),
                const SizedBox(width: 16),
                _buildLegendItem(const Color(0xFFEAEDFF), 'Chưa làm', border: true),
                const SizedBox(width: 16),
                _buildLegendItem(const Color(0xFF93C5FD), 'Đang chọn', border: true),
              ],
            ),
            const SizedBox(height: 16),
            Expanded(
              child: GridView.builder(
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 5,
                  crossAxisSpacing: 8,
                  mainAxisSpacing: 8,
                  childAspectRatio: 1.3,
                ),
                itemCount: _questions.length,
                itemBuilder: (context, idx) {
                  final q = _questions[idx];
                  final isAnswered = _userAnswers.containsKey(q.id);
                  final isCurrent = (idx == _currentIndex);

                  Color bg = const Color(0xFFF2F3FF);
                  Color textCol = Colors.black87;

                  if (isAnswered) {
                    bg = const Color(0xFF004AC6);
                    textCol = Colors.white;
                  }
                  if (isCurrent) {
                    bg = isAnswered ? const Color(0xFF1D4ED8) : const Color(0xFFDBEAFE);
                  }

                  return InkWell(
                    onTap: () {
                      setState(() => _currentIndex = idx);
                      Navigator.pop(ctx);
                    },
                    borderRadius: BorderRadius.circular(10),
                    child: Container(
                      decoration: BoxDecoration(
                        color: bg,
                        borderRadius: BorderRadius.circular(10),
                        border: isCurrent
                            ? Border.all(color: const Color(0xFF2563EB), width: 2)
                            : Border.all(color: Colors.black.withOpacity(0.06)),
                      ),
                      alignment: Alignment.center,
                      child: Text(
                        '${q.questionNumber}',
                        style: TextStyle(
                          color: textCol,
                          fontWeight: FontWeight.bold,
                          fontSize: 13,
                        ),
                      ),
                    ),
                  );
                },
              ),
            ),
            SafeArea(
              child: SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.pop(ctx);
                    _openConfirmDialog();
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF006E2D),
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                  ),
                  child: const Text('Nộp bài ngay', style: TextStyle(fontWeight: FontWeight.bold)),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLegendItem(Color color, String label, {bool border = false}) {
    return Row(
      children: [
        Container(
          width: 14,
          height: 14,
          decoration: BoxDecoration(
            color: color,
            borderRadius: BorderRadius.circular(4),
            border: border ? Border.all(color: Colors.black26) : null,
          ),
        ),
        const SizedBox(width: 4),
        Text(label, style: const TextStyle(fontSize: 11, color: Colors.black54)),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_questions.isEmpty) {
      return const Scaffold(body: Center(child: Text('Đề thi không có câu hỏi.')));
    }

    final q = _questions[_currentIndex];
    final selectedOption = _userAnswers[q.id];

    final m = _remainingSeconds ~/ 60;
    final s = _remainingSeconds % 60;
    final timeStr = '${m.toString().padLeft(2, '0')}:${s.toString().padLeft(2, '0')}';
    final isNearEnd = _remainingSeconds < 300;

    Passage? currentPassage;
    if (q.partNumber == 6 || q.partNumber == 7) {
      currentPassage = widget.examDetail.passages.firstWhere(
        (p) => p.id == q.passageId,
        orElse: () => Passage(id: 0, partNumber: q.partNumber, passageOrder: 1),
      );
    }

    return Scaffold(
      backgroundColor: const Color(0xFFFAF8FF),
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.close, color: Colors.black87),
          onPressed: () {
            showDialog(
              context: context,
              builder: (ctx) => AlertDialog(
                title: const Text('Thoát bài thi?'),
                content: const Text('Thời gian vẫn sẽ tiếp tục tính nếu bạn rời phòng thi.'),
                actions: [
                  TextButton(onPressed: () => Navigator.pop(ctx), child: const Text('Ở lại')),
                  ElevatedButton(
                    onPressed: () {
                      Navigator.pop(ctx);
                      Navigator.pop(context);
                    },
                    style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFFBA1A1A)),
                    child: const Text('Thoát', style: TextStyle(color: Colors.white)),
                  ),
                ],
              ),
            );
          },
        ),
        title: Container(
          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
          decoration: BoxDecoration(
            color: isNearEnd ? const Color(0xFFFFDAD6) : const Color(0xFFDBEAFE),
            borderRadius: BorderRadius.circular(20),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.timer, size: 16, color: isNearEnd ? const Color(0xFFBA1A1A) : const Color(0xFF004AC6)),
              const SizedBox(width: 4),
              Text(
                timeStr,
                style: TextStyle(
                  color: isNearEnd ? const Color(0xFFBA1A1A) : const Color(0xFF004AC6),
                  fontWeight: FontWeight.w900,
                  fontFamily: 'monospace',
                ),
              ),
            ],
          ),
        ),
        actions: [
          TextButton.icon(
            onPressed: _openPaletteSheet,
            icon: const Icon(Icons.grid_view, size: 16, color: Color(0xFF004AC6)),
            label: Text(
              '${_userAnswers.length}/${_questions.length}',
              style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12, color: Color(0xFF004AC6)),
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(right: 12),
            child: ElevatedButton(
              onPressed: _openConfirmDialog,
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFF006E2D),
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                elevation: 0,
              ),
              child: const Text('Nộp bài', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
            ),
          ),
        ],
      ),
      body: Column(
        children: [
          // Sub-header thông tin Part
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            color: const Color(0xFFF2F3FF),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                  decoration: BoxDecoration(
                    color: const Color(0xFF004AC6),
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: Text(
                    'PART ${q.partNumber}',
                    style: const TextStyle(color: Colors.white, fontSize: 11, fontWeight: FontWeight.bold),
                  ),
                ),
                Text(
                  'Câu ${q.questionNumber} / ${_questions.last.questionNumber}',
                  style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: Colors.black54),
                ),
              ],
            ),
          ),

          // Nội dung câu hỏi và đoạn văn
          Expanded(
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                // Bài đọc (Passage) cho Part 6 và Part 7
                if ((q.partNumber == 6 || q.partNumber == 7) && currentPassage != null && (currentPassage.content != null || currentPassage.imageUrl != null))
                  Container(
                    margin: const EdgeInsets.only(bottom: 16),
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: Colors.black.withOpacity(0.06)),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            const Icon(Icons.article, size: 16, color: Color(0xFF004AC6)),
                            const SizedBox(width: 6),
                            Expanded(
                              child: Text(
                                currentPassage.title ?? 'Đoạn văn đọc hiểu',
                                style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13, color: Color(0xFF004AC6)),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 10),
                        if (currentPassage.imageUrl != null && currentPassage.imageUrl!.isNotEmpty)
                          Padding(
                            padding: const EdgeInsets.only(bottom: 8),
                            child: Image.network(
                              currentPassage.imageUrl!,
                              fit: BoxFit.contain,
                              errorBuilder: (context, error, stackTrace) => const SizedBox(),
                            ),
                          ),
                        Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            color: const Color(0xFFFAF8FF),
                            borderRadius: BorderRadius.circular(10),
                          ),
                          child: Text(
                            currentPassage.content ?? '',
                            style: const TextStyle(fontSize: 12.5, height: 1.5),
                          ),
                        ),
                      ],
                    ),
                  ),

                // Question Box
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: Colors.black.withOpacity(0.06)),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        q.content ?? 'Câu hỏi ${q.questionNumber}',
                        style: const TextStyle(fontSize: 14.5, fontWeight: FontWeight.bold, height: 1.35),
                      ),
                      const SizedBox(height: 16),

                      // Options A, B, C, D
                      ...q.options.map((opt) {
                        final isSelected = (selectedOption == opt.optionLabel);

                        return Container(
                          margin: const EdgeInsets.only(bottom: 10),
                          child: InkWell(
                            onTap: () => _selectOption(q.id, opt.optionLabel),
                            borderRadius: BorderRadius.circular(14),
                            child: Container(
                              padding: const EdgeInsets.all(12),
                              decoration: BoxDecoration(
                                color: isSelected ? const Color(0xFFDBEAFE) : Colors.white,
                                borderRadius: BorderRadius.circular(14),
                                border: Border.all(
                                  color: isSelected ? const Color(0xFF004AC6) : Colors.black.withOpacity(0.12),
                                  width: isSelected ? 1.8 : 1,
                                ),
                              ),
                              child: Row(
                                children: [
                                  Container(
                                    width: 32,
                                    height: 32,
                                    decoration: BoxDecoration(
                                      color: isSelected ? const Color(0xFF004AC6) : const Color(0xFFF2F3FF),
                                      shape: BoxShape.circle,
                                    ),
                                    alignment: Alignment.center,
                                    child: Text(
                                      opt.optionLabel,
                                      style: TextStyle(
                                        color: isSelected ? Colors.white : Colors.black87,
                                        fontWeight: FontWeight.bold,
                                        fontSize: 13,
                                      ),
                                    ),
                                  ),
                                  const SizedBox(width: 12),
                                  Expanded(
                                    child: Text(
                                      opt.content,
                                      style: TextStyle(
                                        fontSize: 13,
                                        fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
                                        color: isSelected ? const Color(0xFF004AC6) : Colors.black87,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        );
                      }),
                    ],
                  ),
                ),
              ],
            ),
          ),

          // Bottom Bar: Câu trước, Hủy chọn, Câu tiếp
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            decoration: BoxDecoration(
              color: Colors.white,
              border: Border(top: BorderSide(color: Colors.black.withOpacity(0.06))),
            ),
            child: SafeArea(
              child: Row(
                children: [
                  Expanded(
                    child: OutlinedButton.icon(
                      onPressed: _currentIndex > 0 ? () => setState(() => _currentIndex--) : null,
                      icon: const Icon(Icons.chevron_left, size: 18),
                      label: const Text('Câu trước', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
                      style: OutlinedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 12),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  IconButton(
                    onPressed: selectedOption != null ? () => _clearAnswer(q.id) : null,
                    icon: const Icon(Icons.backspace_outlined),
                    tooltip: 'Hủy chọn',
                    color: Colors.grey,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: ElevatedButton.icon(
                      onPressed: _currentIndex < _questions.length - 1
                          ? () => setState(() => _currentIndex++)
                          : null,
                      icon: const Icon(Icons.chevron_right, size: 18),
                      label: const Text('Câu tiếp', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF004AC6),
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 12),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
