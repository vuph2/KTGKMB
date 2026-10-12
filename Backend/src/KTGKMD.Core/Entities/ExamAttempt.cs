using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace KTGKMD.Entities
{
    [Table("ExamAttempts")]
    public class ExamAttempt : Entity<int>
    {
        [Column("AttemptId")]
        public override int Id { get; set; }

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime? EndTime { get; set; }

        public int? RemainingSeconds { get; set; } // 75 * 60 = 4500 giay

        public int TotalQuestions { get; set; } = 100;

        public int CorrectCount { get; set; } = 0;

        public int IncorrectCount { get; set; } = 0;

        public int SkippedCount { get; set; } = 0;

        public int Score { get; set; } = 0;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "InProgress"; // "InProgress", "Completed", "TimedOut", "Cancelled"

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
    }
}
