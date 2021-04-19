namespace DummyWebApp.Extensions
{
    using ActionResults;
    using BLL.ResultModel.Abstraction;
    using Microsoft.AspNetCore.Mvc;

    public static class ActionResultExtensions
    {
        public static IActionResult ToActionResult(this IResult result)
            => new ErrorableActionResult(result);
    }
}