using System.IO;
using Abp.Dependency;
using KTGKMD.Exams.Dto;

namespace KTGKMD.Exams.Services
{
    public interface IDocxExamParserService : ITransientDependency
    {
        DocxParseResultDto ParseDocx(Stream docxStream, string defaultTitle);
    }
}
