# aggregatorAPI

 This is an API aggregator project that uses 3 external APIs, fetching all data and return them simultaneously.

 We currently use 3 APIs:
 1) Pokemon Api (no api key needed): an api that returns data about pokemons 
 2) Bored Api   (no api key needed): an api that returns data about a random activity to do if you are bored
 3) ...

# set up

In order to run the project :
1) Clone the repo locally
2) Open appsettings.json and replace all "apiKeys" with your api keys for each external Api
3) Build project with `dotnet build`
4) Run the project with `dotnet run` or `dotnet watch` to open swagger or by click start debugging in VS code IDE

# return values
The api call returns N json objects seperated by comma, where N is the number of API calls. If the response failed the object contains the message error . Currently the n = 2 for 2 API calls

Example Response:

{"activity":"Play a video game","type":"recreational","participants":1},{"name":"ditto","height":3,"weight":40,"location_area_encounters":"https://pokeapi.co/api/v2/pokemon/132/encounters"}