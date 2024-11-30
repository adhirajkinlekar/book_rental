namespace book_rental_api.Exceptions
{
    public class APIException : Exception
    {
        public int StatusCode { get; }

        public APIException(string message, int statusCode) : base(message)
        {

            StatusCode = statusCode;

        }
    }

    public class ModelValidationException : APIException
{
    public IDictionary<string, string[]> Errors { get; }

    public ModelValidationException(IDictionary<string, string[]> errors)
        : base("Validation failed.", 422) // - 422 Unprocessable Entity
    {
        Errors = errors;
    }
}

}
