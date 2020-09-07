FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "ToDoList.sln" "ToDoList.sln"

COPY "ToDoList.API/ToDoList.API.csproj" "ToDoList.API/ToDoList.API.csproj"
COPY "ToDoList.Data/ToDoList.Data.csproj" "ToDoList.Data/ToDoList.Data.csproj"
COPY "ToDoList.Core/ToDoList.Core.csproj" "ToDoList.Core/ToDoList.Core.csproj"
COPY "ToDoList.API.UnitTests/ToDoList.API.UnitTests.csproj" "ToDoList.API.UnitTests/ToDoList.API.UnitTests.csproj"

RUN dotnet restore "ToDoList.sln"

COPY . .
WORKDIR /src/ToDoList.API
RUN dotnet publish "ToDoList.API.csproj" -c Release -o /app/publish

FROM build AS unittest
WORKDIR "/src/ToDoList.API.UnitTests"

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoList.API.dll"]