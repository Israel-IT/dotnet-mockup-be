namespace DummyWebApp.DAL.Entities.Abstract
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}