# Miss Mysty's OverParse
A complete overhaul of OverParse, based on Remon-7L's design (forked from Tyrone's parser). 
This is a standalone log reader and overlay tool. This tool shows damage statistics for you, the user, and your MPA in realtime, and provides detailed damage breakdowns for later analysis.

## Word Of Warning
This tool and any other damage parser currently violates PSO2's Terms of Service. [It is a bannable offense according to SEGA](http://pso2.jp/players/news/9224/), and legal action could potentially be applied to your account. Please do not misuse information given to you by this tool (or any other 3rd party parsing tool for that matter) **You have been warned.**

## Latest Release
You can find the latest version on our [releases here](https://github.com/mysterious64/OverParse/releases).

## Credits
- [Variant](https://github.com/VariantXYZ/PSO2ACT) for keeping the damage logs up to date.
- [TyroneSama](https://github.com/TyroneSama/OverParse) for making the original version of OverParse.
- [Remon-7L](https://github.com/Remon-7L/OverParse) for his design overhaul and new details (Damage Taken, JA, Crit).
- [SkrubZer0](https://github.com/SkrubZer0/OverParse) for sharing his fixes on optimization.

## MIT License

**Copyright (c) 2018 Mysterious64 (Mysty)**

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

## Developers Section
### Requirements
This project is made possible and built with these requirements: 
* Microsoft's [Visual Studio 2017](https://www.visualstudio.com/vs/whatsnew/) IDE.
* Microsoft's [.NET Framework 4.7.1](https://www.microsoft.com/net/download/dotnet-framework-runtime) Runtime.
* The [NHotkey Library](https://github.com/thomaslevesque/NHotkey), for hot key managing.
* The [Fody Costura](https://github.com/Fody/Costura) addon, for compiling DLLs into executable.

###### Files Explained:
1. `MainWindow.xaml` OverParse’s UI is in .xaml format.

2. `MainWindow.xaml.cs` On startup, the settings is loaded from here, and is responsible for starting the iteration.

3. `Log.cs` Connects the installation and process logs, and .csv file reading.

4. `Click.cs` After MainWindow.xaml.cs is ran, this is relevant for processing and partitioning objects into partial classes.

5. `WindowsServices.cs` After HideIfInactive is ran, it calls on Visual Studios’s generated window title.

6. `Details.xaml.cs` Window for when they double-click on options from ListViewItem.

7. `FontDialogEx.xaml.cs` Font Selection Window.

8. `Inputbox.xaml.cs` The field based on inputs on the number of seconds the “auto-encounter” ends. (Rendered with WPF)

###### Process Flow:
1. `MainWindow.xaml.cs/MainWindow.MainWindow()` loads on startup

2. Calls on `Log.cs / Log.Log() - MainWindow()` and installation connects with PSO2

3. `UpdateForm()` New info updated every 1000ms, the screen is updated through looping.

4. `HideIfInactive()` For every second, the active window’s title is obtained through iterations.

5. `CheckForNewLog()` For every second, Confirms and loops if there is no new .csv file.

6. Event handlers are covered by `MainWindow.xaml.cs`, and Click.cs splits it up for easier use. However, in foresight, there’s still room for improvement, as it’s still pretty bad, at least bad in my opinion.
 
