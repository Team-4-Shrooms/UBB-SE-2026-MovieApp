namespace MovieApp.Logic.Interfaces.Services
{
    public interface IReferralCodeGenerator
    {
        string Generate(string username, int userIdentifier);
    }
}
