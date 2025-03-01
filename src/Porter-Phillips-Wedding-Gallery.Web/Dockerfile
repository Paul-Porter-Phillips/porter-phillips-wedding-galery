# Use official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Copy published output
COPY build /app
COPY wwwroot /app/wwwroot

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Porter-Phillips-Wedding-Gallery.Web.dll"]