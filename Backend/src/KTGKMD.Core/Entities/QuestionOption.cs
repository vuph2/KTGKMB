using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace KTGKMD.Entities
{
    [Table("QuestionOptions")]
    public class QuestionOption : Entity<int>
    {
        [Column("OptionId")]
        public override int Id { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [Required]
        [StringLength(1)]
        public string OptionLabel { get; set; } // 'A', 'B', 'C', 'D'

        [Required]
        public string Content { get; set; }

        public bool IsCorrect { get; set; } = false;
    }
}
