FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy tất cả csproj để restore trước (giúp tối ưu Docker cache)
COPY ["src/YourExam.Api/YourExam.Api.csproj", "src/YourExam.Api/"]
COPY ["src/YourExam.Application/YourExam.Application.csproj", "src/YourExam.Application/"]
COPY ["src/YourExam.Domain/YourExam.Domain.csproj", "src/YourExam.Domain/"]
COPY ["src/YourExam.Infrastructure/YourExam.Infrastructure.csproj", "src/YourExam.Infrastructure/"]
RUN dotnet restore "./src/YourExam.Api/YourExam.Api.csproj"

# Copy toàn bộ mã nguồn
COPY . .
WORKDIR "/src/src/YourExam.Api"
RUN dotnet build "./YourExam.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./YourExam.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourExam.Api.dll"]
