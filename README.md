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
3) build project with `dotnet build`
4) run the project with `dotnet run` or `dotnet watch` to open swagger or by click start debugging in VS code IDE

# return values
The api call returns a json object with  key "ResList" and a value a *List* of objects (`AggregateResult` objects). Each one item in the list has:
1) The name or url of api that has been called and
2) The return value as a string - that is a serialized form of the JSON response of the api call

Example Response:

{"ResList":[{"ApiUrl":"test - bored","Result":"{\u0022activity\u0022:\u0022Do yoga\u0022,\u0022type\u0022:\u0022recreational\u0022,\u0022participants\u0022:1}"},{"ApiUrl":"test - pokemon","Result":"{\u0022name\u0022:\u0022ditto\u0022,\u0022height\u0022:3,\u0022weight\u0022:40,\u0022location_area_encounters\u0022:\u0022https://pokeapi.co/api/v2/pokemon/132/encounters\u0022}"}]}

