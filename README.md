# aggregatorAPI

 This is an API aggregator project that uses 3 external APIs, fetching all data and return them simultaneously.

 We currently use 3 APIs:
 1) Pokemon Api (no api key needed): an api that returns data about pokemons 
 2) Bored Api (no api key needed): an api that returns data about a random activity to do if you are bored
 3) News Api (api key is mandatory) : api that returns global news. More in https://newsapi.org/

There is an extra `AggregatorAPI\statistics` endpoint in order to get statistics about requests for each api
# set up

In order to run the project :
1) Clone the repo locally
2) Open appsettings.json and replace all "apiKeys" with your api keys for each external Api
3) Build project with `dotnet build`
4) Run the project with `dotnet run` or `dotnet watch` to open swagger or by click start debugging in VS code IDE

#request parameters
In order to use the News api the "q" parameter is required as we need to get news for specific query i.e : 
https://newsapi.org/everything?q=trump

The object from news has an array of articles in which we bind the values as source, author, title, content and returning them.

# return values
The api call returns N json objects seperated by comma, where N is the number of API calls. If the response failed the object contains the message error . Currently the n = 3 for 3 API calls. 

Example Response with 2 APIs (Bored and Pokemon Api):

{"activity":"Play a video game","type":"recreational","participants":1},{"name":"ditto","height":3,"weight":40,"location_area_encounters":"https://pokeapi.co/api/v2/pokemon/132/encounters"}


Example Response with 3 APIs (Bored, Pokemon and News Api):

{"activity":"Improve your touch typing","type":"busywork","participants":1}, {"name":"ditto","height":3,"weight":40,"location_area_encounters":"https://pokeapi.co/api/v2/pokemon/132/encounters"}, {"status":"ok","totalResults":175200,"articles":[{"source":{"id":null,"name":"Gizmodo.com"},"author":"Matt Novak","title":"Trump Plans Private Dinner for Largest Buyers of $TRUMP Crypto","content":"President Donald Trump launched his own cryptocurrency just before taking office in January in one of the most blatantly unethical financial schemes from any U.S. president of the modern era. And whi\u2026 [\u002B5004 chars]"}}]

Example error response :

{"activity":"Buy a new house decoration","type":"recreational","participants":1}, {"name":"ditto","height":3,"weight":40,"location_area_encounters":"https://pokeapi.co/api/v2/pokemon/132/encounters"}, Client Error: BadRequest