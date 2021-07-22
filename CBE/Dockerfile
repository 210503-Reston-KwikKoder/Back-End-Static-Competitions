FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app 

COPY *.sln .

COPY StaticCompetitionRest/*.csproj ./StaticCompetitionRest/
COPY StaticCompetitionBL/*.csproj ./StaticCompetitionBL/
COPY StaticCompetitionDL/*.csproj ./StaticCompetitionDL/
COPY StaticCompetitionModels/*.csproj ./StaticCompetitionModels/
COPY CBETests/*.csproj ./CBETests/

RUN cd StaticCompetitionRest && dotnet restore

COPY . ./


RUN dotnet publish StaticCompetitionRest -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app 

COPY --from=build /app/publish ./
CMD ["dotnet", "StaticCompetitionRest.dll"]