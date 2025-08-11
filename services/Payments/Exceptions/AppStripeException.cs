namespace Payments.Exceptions;

public class AppStripeException : Exception
{
    public AppStripeException() : base() { }
    public AppStripeException(string msg): base(msg) {}
}