FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SocialNetwork/SocialNetwork.csproj", "SocialNetwork/"]
RUN dotnet restore "SocialNetwork/SocialNetwork.csproj"
COPY . .
WORKDIR "/src/SocialNetwork"
RUN dotnet build "SocialNetwork.csproj" -c Release -o /app/build

FROM base AS run
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "SocialNetwork.dll"]
