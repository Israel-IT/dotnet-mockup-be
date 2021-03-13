namespace DummyWebApp.DAL.Entities
{
    using System.Diagnostics.CodeAnalysis;
    using Abstract;
    using Microsoft.AspNetCore.Identity;

    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "EF data model")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "EF data property")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "EF data property")]
    public class User : IdentityUser<int>, IEntity<int>
    {
        public string? Language { get; set; }
    }
}