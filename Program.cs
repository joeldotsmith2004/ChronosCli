await ApiService.InitAsync();
RootCommandService.Init();

var api = ApiService.Instance;
var root = RootCommandService.Instance;

root.Run(args);
