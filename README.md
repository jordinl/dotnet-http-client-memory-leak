## Description

Show memory leak in HttpClient.

The program reads the `urls.txt` file containing a subset of the [top 10 million domains](https://www.domcop.com/top-10-million-domains) and concurrently makes a GET request to each URL. 
The number of URLs and concurrency can be customized with environment variables.

With concurrency set to 100, fetching 15000 URLs a 1 vCPU / 2GB RAM AWS instance runs out of memory.

## Running program

The program can be run:
* From the editor.
* Running `dotnet run` from a terminal.
* With docker, running `docker compose run --build`.

Concurrency and number of urls can be customized with the environment variables `CONCURRENCY` and `LIMIT` respectively, ie `CONCURRENCY=100 LIMIT=10000 dotnet run` or `CONCURRENCY=100 LIMIT=10000 docker compose run --build`. 
These values are set by default to 10 for concurrency and 1000 for limit. Probably best not to use a high value for concurrency at home.
