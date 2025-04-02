namespace Escaper.Utility.Domain.DomainService
{
    public interface IIdentifiedDomainRepository
    {
        public T findById<T>(string id) where T : IdentifiedDomain;

        public void register<T>(T domain) where T : IdentifiedDomain;
    }
}