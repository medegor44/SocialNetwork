using SocialNetwork.Domain.Dialogs;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.ListDialogQuery;

public class ListDialogQueryHandler : IRequestHandler<ListDialogQuery, ListDialogQueryResponse>
{
    private readonly IDialogsRepository _repository;

    public ListDialogQueryHandler(IDialogsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ListDialogQueryResponse> HandleAsync(ListDialogQuery request, CancellationToken cancellationToken)
    {
        var dialog = await _repository.GetAsync(new(request.From, request.To), cancellationToken);

        return new(dialog.Messages.Select(x => new MessageDto(x.From, x.To, x.Text)).ToList());
    }
}