namespace DocumentStorage.Shared.DTOs.Auth
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public AuthResponseDto? Response { get; set; }
        public string? ErrorMessage { get; set; }
        public RegistrationErrorType ErrorType { get; set; }
    }

    public enum RegistrationErrorType
    {
        None,
        EmailAlreadyExists,
        DatabaseError,
        ValidationError,
        UnknownError
    }
}