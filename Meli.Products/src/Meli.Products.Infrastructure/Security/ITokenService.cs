namespace Meli.Products.Infrastructure.Security
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email);
    }
}
