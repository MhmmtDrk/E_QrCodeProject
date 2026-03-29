FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    libfontconfig1 \
    libfreetype6 \
    libpng16-16 \
    libjpeg62-turbo \
    libx11-6 \
    libxrender1 \
    libgif7 \
    && ldconfig && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["QrCode.WebUi/QrCode.WebUi.csproj", "QrCode.WebUi/"]
COPY ["QrCode.Application/QrCode.Application.csproj", "QrCode.Application/"]
COPY ["QrCode.Domain/QrCode.Domain.csproj", "QrCode.Domain/"]
COPY ["QrCode.Infrastructure/QrCode.Infrastructure.csproj", "QrCode.Infrastructure/"]
RUN dotnet restore "QrCode.WebUi/QrCode.WebUi.csproj"
COPY . .
RUN dotnet build "QrCode.WebUi/QrCode.WebUi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QrCode.WebUi/QrCode.WebUi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QrCode.WebUi.dll"]
