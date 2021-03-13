namespace DummyWebApp.DAL.Entities.Abstract
{
    public abstract class EntityBase : IEntity<int>
    {
        public int Id { get; set; }
    }
}