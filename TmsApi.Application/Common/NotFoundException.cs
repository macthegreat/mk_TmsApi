namespace TmsApi.Application.Common;

public class NotFoundException(string message) : Exception(message);
