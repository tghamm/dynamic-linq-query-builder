using System;
using System.Diagnostics.CodeAnalysis;

namespace Castle.DynamicLinqQueryBuilder.Tests
{
    [ExcludeFromCodeCoverage]
    public static class ExceptionAssert
    {
        public static void Throws<T>(Action func) where T : Exception
        {
            var exceptionThrown = false;
            try
            {
                func.Invoke();
            }

            catch (T)
            {
                exceptionThrown = true;
            }
            catch (Exception)
            {

            }
            if (!exceptionThrown)
            {
                throw new Exception(
                    string.Format("An exception of type {0} was expected, but not thrown", typeof(T))
                    );
            }
        }
    }
}
