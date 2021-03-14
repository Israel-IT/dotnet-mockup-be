namespace DummyWebApp.ActionResults
{
    using BorsaLive.Core.Models.Abstraction;
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