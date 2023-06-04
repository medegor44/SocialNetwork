using SocialNetwork.Domain.Friends;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Exceptions;

namespace SocialNetwork.Services.Commands.RemoveFriendsCommand;

public class RemoveFriendsCommandHandler : IRequestHandler<RemoveFriendsCommand>
{
    private readonly IFriendsRepository _friendsRepository;

    public RemoveFriendsCommandHandler(IFriendsRepository friendsRepository)
    {
        _friendsRepository = friendsRepository;
    }
    
    public async Task HandleAsync(RemoveFriendsCommand request, CancellationToken cancellationToken)
    {
        var user = await _friendsRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException(nameof(User), request.UserId.ToString());

        var friends = await _friendsRepository.GetFriendsByIdsAsync(request.FriendsIds, cancellationToken);

        var updated = user.DeleteFriends(friends);
        
        await _friendsRepository.UpdateUserAsync(user, updated, cancellationToken);
    }
}