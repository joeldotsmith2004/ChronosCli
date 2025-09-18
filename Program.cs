using System.CommandLine;
using Spectre.Console;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;


await ApiService.InitAsync();
RootCommandService.Init();

var api = ApiService.Instance;
var root = RootCommandService.Instance;

await root.InvokeAsync(args);
