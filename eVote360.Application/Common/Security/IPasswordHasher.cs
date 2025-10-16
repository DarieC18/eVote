namespace EVote360.Application.Common.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hash, string password);
    }
}
