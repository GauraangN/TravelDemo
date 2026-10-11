# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["TravelDemo.csproj", "./"]
RUN dotnet restore "TravelDemo.csproj"

# Copy remaining source code and publish
COPY . .
RUN dotnet publish "TravelDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Cloud hosts typically use port 8080 or map to $PORT
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# Run the ASP.NET Core application
ENTRYPOINT ["dotnet", "TravelDemo.dll"]
