using SocialNetwork.Domain.Friends;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Exceptions;

namespace SocialNetwork.Services.Commands.CreateFriendsCommand;

public class CreateFriendsCommandHandler : IRequestHandler<CreateFriendsCommand>
{
    private readonly IFriendsRepository _friendsRepository;

    public CreateFriendsCommandHandler(IFriendsRepository friendsRepository)
    {
        _friendsRepository = friendsRepository;
    }
    
    public async Task HandleAsync(CreateFriendsCommand request, CancellationToken cancellationToken)
    {
        var user = await _friendsRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException(nameof(User), request.UserId.ToString());

        var friends = await _friendsRepository.GetFriendsByIdsAsync(request.FriendsIds, cancellationToken);

        var updated = user.AddFriends(friends);

        await _friendsRepository.UpdateUserAsync(user, updated, cancellationToken);
    }
}