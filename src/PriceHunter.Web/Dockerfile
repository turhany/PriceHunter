FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PriceHunter.Web/PriceHunter.Web.csproj", "PriceHunter.Web/"]
RUN dotnet restore "PriceHunter.Web/PriceHunter.Web.csproj"
COPY . .
WORKDIR "/src/PriceHunter.Web"
RUN dotnet build "PriceHunter.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PriceHunter.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PriceHunter.Web.dll"]
