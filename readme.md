CLI interface for the new chronos time logging app for enco.

Usage for now
```
cd ChronosCli
dotnet build
// example
// dotnet run ChronosCli <command> <flags>
dotnet run ChronosCli me
```
this will prompt a log in, then will get your user information from the api

Running without command will give options
```
dotnet run ChronosCli
Required command was not provided.                                                                                                                                                 
                                                                                                                                                                                   
Description:                                                                                                                                                                       
  CLI for Enco Chronos                                                                                                                                                             
                                                                                                                                                                                   
Usage:                                                                                                                                                                             
  ChronosCli [command] [options]                                                                                                                                                   
                                                                                                                                                                                   
Options:                                                                                                                                                                           
  --version       Show version information                                                                                                                                         
  -?, -h, --help  Show help and usage information                                                                                                                                  
                                                                                                                                                                                   
Commands:                                                                                                                                                                          
  me  Fetch my user info    

```

