namespace AspectOrientedProgramming.API.Logging;

/// <summary>
/// Constants for log message templates to ensure consistent logging format
/// </summary>
public static class LogMessageTemplates
{
    // General operation templates
    public const string OperationStarted = "[{CorrelationId}] [{Timestamp}] {OperationName} started";
    public const string OperationCompleted = "[{CorrelationId}] [{Timestamp}] {OperationName} completed in {ElapsedMilliseconds}ms";
    public const string OperationFailed = "[{CorrelationId}] [{Timestamp}] {OperationName} failed after {ElapsedMilliseconds}ms: {ErrorMessage}";
    
    // Method execution templates
    public const string MethodExecuting = "[{CorrelationId}] [{Timestamp}] Executing method: {MethodName} with arguments: [{Arguments}]";
    public const string MethodExecuted = "[{CorrelationId}] [{Timestamp}] Method {MethodName} executed successfully in {ElapsedMilliseconds}ms. Result: {Result}";
    public const string MethodException = "[{CorrelationId}] [{Timestamp}] Exception occurred in method: {MethodName} after {ElapsedMilliseconds}ms";
    
    // Cache templates
    public const string CacheHit = "[{CorrelationId}] [{Timestamp}] Cache HIT for method: {MethodName} with key: {CacheKey}";
    public const string CacheMiss = "[{CorrelationId}] [{Timestamp}] Cache MISS for method: {MethodName} with key: {CacheKey}";
    public const string CacheSet = "[{CorrelationId}] [{Timestamp}] Result cached for method: {MethodName} with key: {CacheKey} for {Duration} minutes";
    
    // Validation templates
    public const string ValidationStarted = "[{CorrelationId}] [{Timestamp}] Starting validation for method: {MethodName}";
    public const string ValidationFailed = "[{CorrelationId}] [{Timestamp}] Validation failed for argument {ArgumentIndex} in method {MethodName}: {ErrorMessage}";
    public const string ValidationCompleted = "[{CorrelationId}] [{Timestamp}] Validation completed successfully for method: {MethodName}";
    
    // Security templates
    public const string SecurityCheckStarted = "[{CorrelationId}] [{Timestamp}] Starting security check for method: {MethodName}";
    public const string SecurityCheckSkipped = "[{CorrelationId}] [{Timestamp}] Security check skipped for method {MethodName} - no HTTP context available";
    public const string UnauthorizedAccess = "[{CorrelationId}] [{Timestamp}] Unauthorized access attempt to method {MethodName} by anonymous user from IP {RemoteIp}";
    public const string AuthorizedAccess = "[{CorrelationId}] [{Timestamp}] Authorized access to method {MethodName} by user {UserName} from IP {RemoteIp}";
    public const string InsufficientRoles = "[{CorrelationId}] [{Timestamp}] User {UserName} does not have required roles {Roles} for method {MethodName}";
    
    // Performance templates
    public const string PerformanceMonitoringStarted = "[{CorrelationId}] [{Timestamp}] Starting performance monitoring for method: {MethodName}";
    public const string PerformanceThresholdExceeded = "[{CorrelationId}] [{Timestamp}] Method {MethodName} took {ElapsedMilliseconds}ms to execute, which exceeds the threshold of {Threshold}ms";
    public const string PerformanceCompleted = "[{CorrelationId}] [{Timestamp}] Method {MethodName} executed in {ElapsedMilliseconds}ms";
    
    // HTTP request templates
    public const string HttpRequestStarted = "[{CorrelationId}] [{Timestamp}] Request started: {Method} {Path} from {RemoteIp}";
    public const string HttpRequestCompleted = "[{CorrelationId}] [{Timestamp}] Request completed: {Method} {Path}";
    public const string HttpRequestFailed = "[{CorrelationId}] [{Timestamp}] Request failed: {Method} {Path} after {ElapsedMilliseconds}ms";
    
    // Business operation templates
    public const string EntityCreated = "[{CorrelationId}] [{Timestamp}] {EntityType} {EntityName} created successfully with ID {EntityId}";
    public const string EntityUpdated = "[{CorrelationId}] [{Timestamp}] {EntityType} {EntityName} (ID: {EntityId}) updated successfully";
    public const string EntityDeleted = "[{CorrelationId}] [{Timestamp}] {EntityType} {EntityName} (ID: {EntityId}) deleted successfully";
    public const string EntityRetrieved = "[{CorrelationId}] [{Timestamp}] Found {EntityType} with ID: {EntityId}, Name: {EntityName}";
    public const string EntityNotFound = "[{CorrelationId}] [{Timestamp}] {EntityType} with ID: {EntityId} not found";
    public const string EntityListRetrieved = "[{CorrelationId}] [{Timestamp}] Retrieved {EntityCount} {EntityType}s from storage";
    public const string EntityListFiltered = "[{CorrelationId}] [{Timestamp}] Found {EntityCount} {EntityType}s with {FilterCriteria}: {FilterValue}";
}
