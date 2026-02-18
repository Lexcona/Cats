# Cats
This is a little widget I made for class, but it works.\
\
It will show a new cat image every 10 seconds. From 2 different APIS.
- https://cataas.com/cat
- https://some-random-api.com/animal/cat

Note, for the current version it does download every image to your temp folder, this is something I will fix later.\

## Keybinds
F1 - DvD Bouncing Mode (why not)
F2 - Stop Getting New Images
F3 - Get New Image (Doesn't work if F2 is enabled)

## Modification
If you want a new api added to the app, all you got to do is go to [SimpleImageWidgit/Views/MainWindow.axaml.cs](https://github.com/Lexcona/Cats/blob/master/SimpleImageWidgit/Views/MainWindow.axaml.cs)\
In the MainWindow() function add a new function called AddAPI, which is used with AddAPI(API_URL, JSON_PATH), the path format is thing/thing/thing, or if there is a list thing/0/thing or 0/thing/thing.\
\
If the API returns the image without a json output, just keep it empty.
