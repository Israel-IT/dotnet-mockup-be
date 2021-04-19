namespace DummyWebApp.ActionResults
{
    using BLL.ResultModel.Abstraction;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorableActionResult : ActionResult
    {
        public ErrorableActionResult(IResult result)
        {
            Result = result;
        }

        public IResult Result { get; }
    }
}