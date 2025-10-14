FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/TrainingManagementAPI/TrainingManagementAPI.csproj ./src/TrainingManagementAPI/
RUN dotnet restore ./src/TrainingManagementAPI/TrainingManagementAPI.csproj
COPY . .
RUN dotnet publish ./src/TrainingManagementAPI/TrainingManagementAPI.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TrainingManagementAPI.dll"]
