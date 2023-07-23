using SocialNetwork.Domain.Friends.Entities;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Events;

namespace SocialNetwork.Services.Commands.CreatePostCommand;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, CreatePostCommandResponse>, IDisposable
{
    private readonly IPostsRepository _repository;
    private readonly IFriendsRepository _friendsRepository;
    private readonly IPostCreatedNotificationSender? _sender;

    public CreatePostCommandHandler(IPostsRepository repository, IFriendsRepository friendsRepository, IPostCreatedNotificationSender? sender)
    {
        _repository = repository;
        _friendsRepository = friendsRepository;
        _sender = sender;
    }
    
    public async Task<CreatePostCommandResponse> HandleAsync(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.CreateAsync(new(new(request.Text), request.UserId), cancellationToken);

        await SendNotificationToFriends(request, cancellationToken, post);

        return new CreatePostCommandResponse(post.Id);
    }

    private async Task SendNotificationToFriends(CreatePostCommand request, CancellationToken cancellationToken, Post post)
    {
        var user = await _friendsRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        foreach (var friend in user?.Friends ?? ArraySegment<Friend>.Empty)
            _sender?.Send(new(post.Id, post.Text.Value, post.UserId, friend.Id), friend.Id);
    }

    public void Dispose()
    {
        _sender?.Dispose();
    }
}