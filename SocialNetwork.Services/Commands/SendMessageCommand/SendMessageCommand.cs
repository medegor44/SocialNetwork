using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.SendMessageCommand;

public record SendMessageCommand(long From, long To, string Message) : IRequest;