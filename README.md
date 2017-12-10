ファイル  
MainWindow.xaml - OverParseのUIになるxaml  
MainWindow.xaml.cs - 起動時に呼ばれ、設定の読み込みやループ処理の開始を担当  
Log.cs - インストール関連、ログ書き込み関連、.csv読み込み関連  
Click.cs - MainWindow.xaml.csからClick関連の処理を分割したもの partial class  
Hotkey.cs - ホットキー関連  
WindowsServices.cs - HideIfInactiveから呼ばれているアクティブなアプリのウィンドゥタイトル取得  
Detalis - ListViewItemをダブルクリックした時のウィンドゥ  
FontDialogEx - フォントの選択ウィンドゥ  
inputbox - 自動エンカウント終了の秒数入力欄(WPF)  

  

処理の流れ  
MainWindow.xaml.cs / MainWindow.MainWindow() - 起動時読み込み  
  ↓  
 Log.cs / Log.Log() - MainWindow()から呼ばれる、インストール関連  
  ↓  
 UpdateForm - 500ms毎に情報・画面更新のループ処理  
 HideIfInactive - 1s毎にアクティブウィンドゥのタイトルを取得するループ処理  
 CheckForNewLog - 1s毎に新しい.csvファイルが無いかどうかを確認するループ処理  
   
イベントハンドラまみれだったMainWindow.xaml.csをClick.csに分けたので大分見やすくなったと思いますがまだまだ見通しが悪いと言えば悪いです  
 
