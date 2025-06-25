# Cài ??t base image cho runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5285
ENV ASPNETCORE_URLS=http://+:5285

# Cài ??t base image cho build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DoAnTotNghiep.csproj", "."]
RUN dotnet restore "./DoAnTotNghiep.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DoAnTotNghiep.csproj" -c Release -o /app/build

# Giai ?o?n publish
FROM build AS publish
RUN dotnet publish "DoAnTotNghiep.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai ?o?n final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# S? d?ng c?ng ??ng t? Heroku ($PORT) ho?c m?c ??nh 5285
ENTRYPOINT ["sh", "-c", "dotnet DoAnTotNghiep.dll --urls=http://0.0.0.0:${PORT:-5285}"]