namespace CountryService.gRPC.Interceptors.Helpers;

public static class ExceptionHelpers
{
    public static RpcException Handle(
        this Exception exception,
        ServerCallContext context,
        ILogger logger,
        Guid correlationId) =>
        exception switch
        {
            TimeoutException timeoutException => HandleTimeoutException(timeoutException, context, logger, correlationId),
            SqlException sqlException => HandleSqlException(sqlException, context, logger, correlationId),
            RpcException rpcException => HandleRpcException(rpcException, context, logger, correlationId),
            _ => HandleDefault(exception, context, logger, correlationId)
        };
    private static RpcException HandleTimeoutException(TimeoutException exception, ServerCallContext context, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {correlationId} - A timeout occurred", correlationId);
        var status = new Status(StatusCode.Internal, "An external resource did not answer within the time limit");
        return new RpcException(status, CreateTrailers(correlationId));
    }
    private static RpcException HandleSqlException(SqlException exception, ServerCallContext context, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {correlationId} - A timeout occurred", correlationId);
        var status = exception.Number == -2
            ? new Status(StatusCode.DeadlineExceeded, "SQL timeout")
            : new Status(StatusCode.Internal, "SQL Error");
        return new RpcException(status, CreateTrailers(correlationId));
    }
    private static RpcException HandleRpcException(RpcException exception, ServerCallContext context, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {correlationId} - An error occurred", correlationId);
        var trailers = exception.Trailers;
        var newTrailers = CreateTrailers(correlationId);
        foreach (var entry in trailers)
        {
            newTrailers.Add(entry);
        }
        return new RpcException(new Status(exception.Status.StatusCode, exception.Status.Detail), newTrailers);
    }
    private static RpcException HandleDefault(Exception exception, ServerCallContext context, ILogger logger, Guid correlationId)
    {
        logger.LogError(exception, "CorrelationId: {correlationId} - An error occurred", correlationId);
        return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(correlationId));
    }
    private static Metadata CreateTrailers(Guid correlationId)
    {
        var trailers = new Metadata();
        trailers.Add("CorrelationId", correlationId.ToString());
        return trailers;
    }
}