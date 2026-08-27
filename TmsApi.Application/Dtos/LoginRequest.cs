namespace TmsApi.Application.Dtos;

public record LoginRequest(string Email, string Password); 
public record UserProfileDto(string DisplayName, string Role);