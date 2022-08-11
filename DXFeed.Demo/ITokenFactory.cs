using System.Threading.Tasks;

namespace DXFeed.Demo
{
    internal interface ITokenFactory
    {
        string? GetToken(string url, string name);
        Task<string?> GetTokenAsync(string url, string name);
    }
}
