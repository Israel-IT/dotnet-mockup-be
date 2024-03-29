﻿namespace DummyWebApp.BLL.ResultModel.Abstraction
{
    using System;
    using System.Collections.Generic;

    public interface IResult
    {
        IReadOnlyCollection<string> Messages { get; }

        bool Success { get; }

        Exception? Exception { get; }
    }
}