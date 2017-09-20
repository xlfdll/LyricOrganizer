using System.Windows;
using System.Windows.Input;

namespace LyricOrganizer
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.DataContext = new AppState();
		}

		private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Commands.GenerateDocumentCommand.Execute(AppState.Current.CurrentItem);
		}
	}
}