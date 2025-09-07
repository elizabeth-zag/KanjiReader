FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /backend

COPY . .

RUN dotnet restore "KanjiReader.csproj"
RUN dotnet publish "KanjiReader.csproj" -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "KanjiReader.dll"]
