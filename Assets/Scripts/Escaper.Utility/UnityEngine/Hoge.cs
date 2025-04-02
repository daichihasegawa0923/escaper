using Escaper.Utility.Domain;
using Escaper.Utility.Domain.DomainService;

public class Hoge : IIdentifiedDomainRepository
{
    public T findById<T>(string id) where T : IdentifiedDomain
    {
        throw new System.NotImplementedException();
    }

    public void register<T>(T domain) where T : IdentifiedDomain
    {
        throw new System.NotImplementedException();
    }
}