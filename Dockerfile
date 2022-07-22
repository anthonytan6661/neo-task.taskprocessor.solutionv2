#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

ENV RABBITMQ_HOST rabbitmq
ENV RABBITMQ_PORT 5672
ENV RABBITMQ_USER guest
ENV RABBITMQ_PASSWORD guest
ENV RABBITMQ_VHOST="/"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["neo-task.taskprocessor.solution.sln","neo-task.taskprocessor.solution/"]
COPY ["neo-task.taskprocessor/neo-task.taskprocessor.csproj", "neo-task.taskprocessor/"]
COPY ["neo-task.rabbitmq/neo-task.rabbitmq.csproj", "neo-task.rabbitmq/"]

RUN dotnet restore "neo-task.taskprocessor/neo-task.taskprocessor.csproj"
COPY . .
WORKDIR "/src/neo-task.taskprocessor"
RUN dotnet build "neo-task.taskprocessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "neo-task.taskprocessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "neo-task.taskprocessor.dll"]