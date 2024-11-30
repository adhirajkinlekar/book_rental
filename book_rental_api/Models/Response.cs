using System.ComponentModel.DataAnnotations;

namespace book_rental_api.Models
{

    // Return Unit as the type parameter for Response to represent "no value"
    public struct Unit
    {
        public static readonly Unit Value = new();
    }

    public class Response<T>
    {
        public T? Data { get; set; }

        [Required]
        public bool Success { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public required string Message { get; set; }
        public string? StackTrace { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
