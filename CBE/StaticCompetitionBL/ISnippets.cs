using System.Threading.Tasks;
using StaticCompetitionModels;
using Octokit;

namespace StaticCompetitionBL
{
    public interface ISnippets
    {
        Task<TestMaterial> GetRandomQuote();
        Task<TestMaterial> GetCodeSnippet(int id);
        Task<string> GetAuth0String();
    }
}