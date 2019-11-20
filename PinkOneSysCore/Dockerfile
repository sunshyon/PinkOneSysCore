FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["PinkOneSysCore/PinkOneSysCore.csproj", "PinkOneSysCore/"]
COPY ["DataService/DataService.csproj", "DataService/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Utility/Utility.csproj", "Utility/"]
RUN dotnet restore "PinkOneSysCore/PinkOneSysCore.csproj"
COPY . .
WORKDIR "/src/PinkOneSysCore"
RUN dotnet build "PinkOneSysCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PinkOneSysCore.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PinkOneSysCore.dll"]