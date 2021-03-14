namespace DummyWebApp.Extensions
{
    using ActionResults;
    using BorsaLive.Core.Models.Abstraction;
    using Microsoft.AspNetCore.Mvc;

    public static class ActionResultExtensions
    {
        public static IActionResult ToActionResult(this IResult result)
            => new ErrorableActionResult(result);
    }
}