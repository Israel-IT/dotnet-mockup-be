namespace DummyWebApp.Extensions
{
    using System;

    public static class EnvironmentExtensions
    {
        public static T GetValueOrThrow<T>(string variable)
        {
            var environmentVariable = Environment.GetEnvironmentVariable(variable);

            if (environmentVariable is null)
                throw new Exception($"Please configure {variable} environment variable");

            return (T)Convert.ChangeType(environmentVariable, typeof(T));
        }
    }
}