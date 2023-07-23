﻿using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.ListDialogQuery;

public record ListDialogQuery(long From, long To) : IRequest<ListDialogQueryResponse>;
