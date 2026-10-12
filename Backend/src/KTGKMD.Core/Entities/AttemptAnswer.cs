using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace KTGKMD.Entities
{
    [Table("UserAnswers")]
    public class AttemptAnswer : Entity<int>
    {
        [Column("UserAnswerId")]
        public override int Id { get; set; }

        public int AttemptId { get; set; }

        [ForeignKey("AttemptId")]
        public virtual ExamAttempt Attempt { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public int? SelectedOptionId { get; set; }

        [ForeignKey("SelectedOptionId")]
        public virtual QuestionOption SelectedOption { get; set; }

        [StringLength(1)]
        public string SelectedLabel { get; set; } // 'A', 'B', 'C', 'D'

        public bool? IsCorrect { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.Now;
    }
}
