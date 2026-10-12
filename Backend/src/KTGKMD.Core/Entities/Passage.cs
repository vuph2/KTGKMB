using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace KTGKMD.Entities
{
    [Table("Passages")]
    public class Passage : Entity<int>
    {
        [Column("PassageId")]
        public override int Id { get; set; }

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        public int PartNumber { get; set; } // 6 hoac 7

        public int PassageOrder { get; set; } = 1;

        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
