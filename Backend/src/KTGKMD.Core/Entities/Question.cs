using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace KTGKMD.Entities
{
    [Table("Questions")]
    public class Question : Entity<int>
    {
        [Column("QuestionId")]
        public override int Id { get; set; }

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        public int? PassageId { get; set; }

        [ForeignKey("PassageId")]
        public virtual Passage Passage { get; set; }

        public int PartNumber { get; set; } // 5, 6, 7

        public int QuestionNumber { get; set; } // 101 - 200

        public string Content { get; set; }

        public string Explanation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
