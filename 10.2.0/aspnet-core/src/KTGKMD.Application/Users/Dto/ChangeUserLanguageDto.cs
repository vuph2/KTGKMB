using System.ComponentModel.DataAnnotations;

namespace KTGKMD.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}