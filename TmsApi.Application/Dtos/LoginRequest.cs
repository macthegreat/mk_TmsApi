namespace TmsApi.Application.Dtos;

public record LoginRequest(string Username, string Password); 
public record UserProfileDto(string DisplayName, string Role);