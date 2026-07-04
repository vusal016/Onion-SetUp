namespace OnionSetUp.Application.Common.Constants
{
    public static class ErrorMessages
    {
        // General
        public const string UnexpectedError = "An unexpected error occurred.";
        public const string OperationFailed = "The operation failed.";
        public const string InvalidOperation = "The operation is invalid.";

        // Validation
        public const string ValidationFailed = "Validation failed.";
        public const string InvalidRequest = "Invalid request.";
        public const string InvalidInput = "Invalid input provided.";

        // Not Found
        public const string ResourceNotFound = "Resource not found.";
        public const string UserNotFound = "User not found.";
        public const string RoleNotFound = "Role not found.";
        public const string ProductNotFound = "Product not found.";

        // Conflict
        public const string ResourceAlreadyExists = "Resource already exists.";
        public const string EmailAlreadyExists = "Email already exists.";
        public const string UsernameAlreadyExists = "Username already exists.";

        // Authentication
        public const string InvalidCredentials = "Invalid email or password.";
        public const string Unauthorized = "Unauthorized access.";
        public const string InvalidToken = "Invalid token.";
        public const string TokenExpired = "Token has expired.";

        // Authorization
        public const string Forbidden = "You do not have permission to perform this action.";

        // Database
        public const string DatabaseError = "Database operation failed.";
        public const string SaveChangesFailed = "Failed to save changes.";
        public const string ConcurrencyConflict = "Concurrency conflict occurred.";

        // Delete
        public const string DeleteFailed = "Failed to delete resource.";
        public const string ResourceCannotBeDeleted = "Resource cannot be deleted.";

        // Create
        public const string CreateFailed = "Failed to create resource.";

        // Update
        public const string UpdateFailed = "Failed to update resource.";

        // Business
        public const string BusinessRuleViolation = "Business rule violation.";
        public const string OperationNotAllowed = "Operation is not allowed.";

        // File
        public const string FileNotFound = "File not found.";
        public const string InvalidFileFormat = "Invalid file format.";
        public const string FileUploadFailed = "File upload failed.";

        // Duplicate
        public const string DuplicateRecord = "Duplicate record found.";

        // Null
        public const string NullReference = "The requested object is null.";

        // Timeout
        public const string RequestTimeout = "The request timed out.";

        // External Services
        public const string ExternalServiceUnavailable = "External service is unavailable.";
    }
}

