class Exam {
  final int id;
  final String title;
  final String? description;
  final int durationMinutes;
  final int totalQuestions;
  final DateTime createdAt;

  Exam({
    required this.id,
    required this.title,
    this.description,
    required this.durationMinutes,
    required this.totalQuestions,
    required this.createdAt,
  });

  factory Exam.fromJson(Map<String, dynamic> json) {
    return Exam(
      id: json['id'] ?? 0,
      title: json['title'] ?? '',
      description: json['description'],
      durationMinutes: json['durationMinutes'] ?? 75,
      totalQuestions: json['totalQuestions'] ?? 100,
      createdAt: json['createdAt'] != null
          ? DateTime.tryParse(json['createdAt']) ?? DateTime.now()
          : DateTime.now(),
    );
  }
}

class ExamDetail extends Exam {
  final List<Passage> passages;
  final List<Question> questions;

  ExamDetail({
    required super.id,
    required super.title,
    super.description,
    required super.durationMinutes,
    required super.totalQuestions,
    required super.createdAt,
    required this.passages,
    required this.questions,
  });

  factory ExamDetail.fromJson(Map<String, dynamic> json) {
    var pList = (json['passages'] as List<dynamic>?)
            ?.map((p) => Passage.fromJson(p))
            .toList() ??
        [];
    var qList = (json['questions'] as List<dynamic>?)
            ?.map((q) => Question.fromJson(q))
            .toList() ??
        [];

    return ExamDetail(
      id: json['id'] ?? 0,
      title: json['title'] ?? '',
      description: json['description'],
      durationMinutes: json['durationMinutes'] ?? 75,
      totalQuestions: json['totalQuestions'] ?? 100,
      createdAt: json['createdAt'] != null
          ? DateTime.tryParse(json['createdAt']) ?? DateTime.now()
          : DateTime.now(),
      passages: pList,
      questions: qList,
    );
  }
}

class Passage {
  final int id;
  final int partNumber;
  final int passageOrder;
  final String? title;
  final String? content;
  final String? imageUrl;

  Passage({
    required this.id,
    required this.partNumber,
    required this.passageOrder,
    this.title,
    this.content,
    this.imageUrl,
  });

  factory Passage.fromJson(Map<String, dynamic> json) {
    return Passage(
      id: json['id'] ?? 0,
      partNumber: json['partNumber'] ?? 6,
      passageOrder: json['passageOrder'] ?? 1,
      title: json['title'],
      content: json['content'],
      imageUrl: json['imageUrl'],
    );
  }
}

class Question {
  final int id;
  final int? passageId;
  final int partNumber;
  final int questionNumber;
  final String? content;
  final String? explanation;
  final List<QuestionOption> options;

  Question({
    required this.id,
    this.passageId,
    required this.partNumber,
    required this.questionNumber,
    this.content,
    this.explanation,
    required this.options,
  });

  factory Question.fromJson(Map<String, dynamic> json) {
    var optList = (json['options'] as List<dynamic>?)
            ?.map((o) => QuestionOption.fromJson(o))
            .toList() ??
        [];

    return Question(
      id: json['id'] ?? 0,
      passageId: json['passageId'],
      partNumber: json['partNumber'] ?? 5,
      questionNumber: json['questionNumber'] ?? 101,
      content: json['content'],
      explanation: json['explanation'],
      options: optList,
    );
  }
}

class QuestionOption {
  final int id;
  final String optionLabel;
  final String content;
  final bool isCorrect;

  QuestionOption({
    required this.id,
    required this.optionLabel,
    required this.content,
    required this.isCorrect,
  });

  factory QuestionOption.fromJson(Map<String, dynamic> json) {
    return QuestionOption(
      id: json['id'] ?? 0,
      optionLabel: json['optionLabel'] ?? '',
      content: json['content'] ?? '',
      isCorrect: json['isCorrect'] ?? false,
    );
  }
}

class SubmitResult {
  final int attemptId;
  final int totalQuestions;
  final int correctCount;
  final int incorrectCount;
  final int skippedCount;
  final int score;
  final List<QuestionResultDetail> details;

  SubmitResult({
    required this.attemptId,
    required this.totalQuestions,
    required this.correctCount,
    required this.incorrectCount,
    required this.skippedCount,
    required this.score,
    required this.details,
  });

  factory SubmitResult.fromJson(Map<String, dynamic> json) {
    var rawList = json['details'] ?? json['Details'];
    var dList = (rawList as List<dynamic>?)
            ?.map((d) => QuestionResultDetail.fromJson(d))
            .toList() ??
        [];

    return SubmitResult(
      attemptId: json['attemptId'] ?? json['AttemptId'] ?? 0,
      totalQuestions: json['totalQuestions'] ?? json['TotalQuestions'] ?? 100,
      correctCount: json['correctCount'] ?? json['CorrectCount'] ?? 0,
      incorrectCount: json['incorrectCount'] ?? json['IncorrectCount'] ?? 0,
      skippedCount: json['skippedCount'] ?? json['SkippedCount'] ?? 0,
      score: json['score'] ?? json['Score'] ?? 0,
      details: dList,
    );
  }
}

class QuestionResultDetail {
  final int questionNumber;
  final int partNumber;
  final String? selectedLabel;
  final String correctLabel;
  final bool isCorrect;
  final String? explanation;

  QuestionResultDetail({
    required this.questionNumber,
    required this.partNumber,
    this.selectedLabel,
    required this.correctLabel,
    required this.isCorrect,
    this.explanation,
  });

  factory QuestionResultDetail.fromJson(Map<String, dynamic> json) {
    return QuestionResultDetail(
      questionNumber: json['questionNumber'] ?? json['QuestionNumber'] ?? 0,
      partNumber: json['partNumber'] ?? json['PartNumber'] ?? 5,
      selectedLabel: json['selectedLabel'] ?? json['SelectedLabel'],
      correctLabel: json['correctLabel'] ?? json['CorrectLabel'] ?? '',
      isCorrect: json['isCorrect'] ?? json['IsCorrect'] ?? false,
      explanation: json['explanation'] ?? json['Explanation'],
    );
  }
}
