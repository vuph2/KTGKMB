import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../models/exam_model.dart';

class ApiService {
  // Ho tro localhost cho ca Windows/Web va Android emulator (10.0.2.2)
  static String get baseUrl {
    if (defaultTargetPlatform == TargetPlatform.android) {
      return 'http://10.0.2.2:5000/api';
    }
    return 'http://localhost:5000/api';
  }

  // 1. Lay danh sach de thi
  static Future<List<Exam>> getExams() async {
    final response = await http.get(Uri.parse('$baseUrl/exams'));
    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      final List list = (data is Map && data.containsKey('result'))
          ? data['result']
          : (data is List ? data : []);
      return list.map((e) => Exam.fromJson(e)).toList();
    } else {
      throw Exception('Không thể tải danh sách đề thi (Mã: ${response.statusCode})');
    }
  }

  // 2. Lay chi tiet de thi
  static Future<ExamDetail> getExamDetail(int examId) async {
    final response = await http.get(Uri.parse('$baseUrl/exams/$examId'));
    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      final map = (data is Map && data.containsKey('result')) ? data['result'] : data;
      return ExamDetail.fromJson(map);
    } else {
      throw Exception('Không thể tải nội dung đề thi (Mã: ${response.statusCode})');
    }
  }

  // 3. Bat dau luot thi (khoi tao bo dem 75 phut)
  static Future<int> startAttempt(int examId) async {
    final response = await http.post(
      Uri.parse('$baseUrl/exams/start'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode({'examId': examId}),
    );
    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      final map = (data is Map && data.containsKey('result')) ? data['result'] : data;
      return map['attemptId'] ?? 0;
    } else {
      throw Exception('Không thể bắt đầu lượt thi.');
    }
  }

  // 4. Luu tam thoi dap an (Auto-save khi thi sinh chon)
  static Future<void> saveAnswer(int attemptId, int questionId, String? label) async {
    try {
      await http.post(
        Uri.parse('$baseUrl/exams/save-answer'),
        headers: {'Content-Type': 'application/json'},
        body: json.encode({
          'attemptId': attemptId,
          'questionId': questionId,
          'selectedLabel': label,
        }),
      );
    } catch (e) {
      debugPrint('Auto-save error: $e');
    }
  }

  // 5. Nop bai va cham diem tu dong
  static Future<SubmitResult> submitExam(
      int attemptId, int remainingSeconds, Map<int, String> answers) async {
    final answersPayload = answers.entries.map((e) => {
          'questionId': e.key,
          'selectedLabel': e.value,
        }).toList();

    final response = await http.post(
      Uri.parse('$baseUrl/exams/submit'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode({
        'attemptId': attemptId,
        'remainingSeconds': remainingSeconds,
        'answers': answersPayload,
      }),
    );

    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      final map = (data is Map && data.containsKey('result')) ? data['result'] : data;
      return SubmitResult.fromJson(map);
    } else {
      throw Exception('Nộp bài không thành công (Mã: ${response.statusCode})');
    }
  }
}
