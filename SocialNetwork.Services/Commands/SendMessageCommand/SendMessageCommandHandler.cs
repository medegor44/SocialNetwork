using SocialNetwork.Domain.Messages;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.SendMessageCommand;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
{
    private readonly IMessageRepository _repository;

    public SendMessageCommandHandler(IMessageRepository repository)
    {
        _repository = repository;
    }

    public Task HandleAsync(SendMessageCommand request, CancellationToken cancellationToken) =>
        _repository.CreateAsync(new Message(request.From, request.To, request.Message), cancellationToken);
}