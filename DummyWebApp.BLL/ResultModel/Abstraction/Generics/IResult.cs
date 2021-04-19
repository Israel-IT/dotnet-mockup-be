namespace DummyWebApp.BLL.ResultModel.Abstraction.Generics
{
    public interface IResult<out TData> : IResult
    {
        TData Data { get; }
    }
}