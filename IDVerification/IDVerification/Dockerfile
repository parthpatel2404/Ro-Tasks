#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IDVerification/IDVerification.csproj", "IDVerification/"]
RUN dotnet restore "IDVerification/IDVerification.csproj"
COPY . .
WORKDIR "/src/IDVerification"
RUN dotnet build "IDVerification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IDVerification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IDVerification.dll"]
