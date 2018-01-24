# Miss Mysty's OverParse
A complete overhaul of OverParse, based on Remon-7L's design. This is a standalone log reader and overlay (for Variant's PSO2DamageDump plugin). This tool shows damage statistics for you, the user, and your MPA in realtime, and provides detailed damage breakdowns for later analysis.

## Latest Release
You can find latest version on our [releases here](https://github.com/mysterious64/OverParse/releases).

## A Word Of Warning
This tool and any other damage parser currently violates PSO2's Terms of Service. [It is a bannable offense according to SEGA](http://pso2.jp/players/news/9224/)), and legal action could potentially be applied to your account. Please do not misuse information given to you by this tool (or any other 3rd party parsing tool for that matter) **You have been warned.**

## Credits
- [Variant](https://github.com/VariantXYZ/PSO2ACT) for keeping the damage logs up to date.
- [TyroneSama](https://github.com/TyroneSama/OverParse) for making the original version of OverParse.
- [Remon-7L](https://github.com/Remon-7L/OverParse) for his design overhaul and new details (Damage Taken, JA, Crit).
- [SkrubZer0](https://github.com/SkrubZer0/OverParse) for sharing his fixes on optimization.

---

## Developers Section
###### Files Explained:
`MainWindow.xaml`
> OverParse’s UI is in .xaml format.
`MainWindow.xaml.cs`
> On startup, the settings is loaded from here, and is responsible for starting the iteration.
`Log.cs`
> Connects the installation and process logs, and .csv file reading.
`Click.cs`
> After MainWindow.xaml.cs is ran, this is relevant for processing and partitioning objects into partial classes.
`WindowsServices.cs`
> After HideIfInactive is ran, it calls on Visual Studios’s generated window title.
`Details` 
> Window for when they double-click on options from ListViewItem.
`FontDialogEx` 
> Font Selection Window.
`Inputbox` 
> The field based on inputs on the number of seconds the “auto-encounter” ends. (Rendered with WPF)

###### Process Flow:
`MainWindow.xaml.cs/MainWindow.MainWindow()` loads on startup
 ↓
`Calls on Log.cs / Log.Log() - MainWindow()` and installation connects with PSO2
 ↓
`UpdateForm` - New info updated every 500ms, the screen is updated through looping.
`HideIfInactive` - For every second, the active window’s title is obtained through iterations.
`CheckForNewLog` - For every second, Confirms and loops if there is no new .csv file.
Event handlers are covered by `MainWindow.xaml.cs`, and Click.cs splits it up for easier use. However, in foresight, there’s still room for improvement, as it’s still pretty bad, at least bad in my opinion.
 
