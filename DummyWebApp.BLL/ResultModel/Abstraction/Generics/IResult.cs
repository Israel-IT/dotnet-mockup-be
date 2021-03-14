namespace BorsaLive.Core.Models.Abstraction.Generics
{
    public interface IResult<out TData> : IResult
    {
        TData Data { get; }
    }
}