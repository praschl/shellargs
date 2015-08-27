using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// source: http://blogs.msdn.com/b/ddietric/archive/2009/01/06/the-ultimate-exceptionassert-throws-method.aspx

// TODO: remove ExceptionAssert when finished migrating to fluent assertions

namespace MiP.ShellArgs.Tests.TestHelpers
{
    public static class ExceptionAssert
    {
        // The type parameter is used for passing in the expected exception type.
        // The where clause is used to ensure that T is Exception or a subclass.
        // Thus an explicit check is not necessary.
        public static void Throws<T>(Action action, Action<T> validator) where T : Exception
        {
            if (action == null)
                throw new ArgumentNullException("action");

            // Executing the action in a try block since we expect it to throw.
            try
            {
                action();
            }

            // Catching the exception regardless of its type.
            catch (Exception e)
            {
                // Comparing the type of the exception to the type of the type
                // parameter, failing the assert in case they are different,
                // even if the type of the exception is derived from the expected type.
                Console.WriteLine(e);
                if (e.GetType() != typeof (T))
                    throw
                        new AssertFailedException(String.Format(
                        CultureInfo.CurrentCulture,
                        "ExceptionAssert.Throws failed. Expected exception type: {0}. Actual exception type: {1}. Exception message: {2}",
                        typeof(T).FullName,
                        e.GetType().FullName,
                        e.Message), e);

                // Calling the validator for the exception object if one was
                // provided by the caller. The validator is expected to use the
                // Assert class for performing the verification.
                validator?.Invoke((T) e);

                // Type check passed and validator did not throw.
                // Everything is fine.
                return;
            }

            // Failing the assert since there was no exception.
            throw new AssertFailedException(String.Format(
                CultureInfo.CurrentCulture,
                "ExceptionAssert.Throws failed. No exception was thrown (expected exception type: {0}).",
                typeof (T).FullName));
        }
    }
}