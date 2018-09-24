using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace LyricOrganizer
{
    public static class Routines
    {
        public static void Search_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Int32 direction = Int32.Parse(e.Argument.ToString());
            List<LyricItem> results = new List<LyricItem>();
            Int32? page = AppState.Current.Page;

            switch (direction)
            {
                case -1:
                    page--;
                    break;
                case 1:
                    page++;
                    break;
                default:
                    page = null;
                    break;
            }

            foreach (ILyricProvider provider in AppState.Current.Providers)
            {
                worker.ReportProgress(0, provider.Name);

                results.AddRange(provider.Search(AppState.Current.ActiveKeyword, (LyricSearchType)AppState.Current.TypeIndex, page));
            }

            e.Result = new Object[] { results, page };
        }

        public static void Search_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AppState.Current.Status = "Getting results from " + e.UserState.ToString() + " ...";
        }

        public static void Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Object[] results = e.Result as Object[];
            List<LyricItem> lyricItems = results[0] as List<LyricItem>;

            if (lyricItems.Count > 0)
            {
                Int32? page = results[1] as Int32?;

                AppState.Current.Page = page != null ? (Int32)page : 1;
                AppState.Current.Results.Clear();

                foreach (LyricItem item in lyricItems)
                {
                    AppState.Current.Results.Add(item);
                }

                AppState.Current.Status = "Found " + lyricItems.Count.ToString() + " item(s)";
            }
            else
            {
                AppState.Current.Status = "No items found";
            }

            AppState.Current.IsBusy = false;
        }

        public static void Generate_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Object[] arguments = e.Argument as Object[];
            LyricItem item = arguments[0] as LyricItem;
            String fileName = arguments[1] as String;

            worker.ReportProgress(0, item.Provider.Name);

            LyricContent lyricContent = item.Provider.Retrieve(item);

            switch (Path.GetExtension(fileName))
            {
                case ".txt":
                    TextLyricWriter writer = new TextLyricWriter(lyricContent);

                    writer.WriteTo(fileName);

                    break;
                default:
                    break;
            }

            e.Result = fileName;
        }

        public static void Generate_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AppState.Current.Status = "Generating lyric document from " + e.UserState.ToString() + " ...";
        }

        public static void Generate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            String fileName = e.Result.ToString();

            AppState.Current.Status = "Done";

            if (MessageBox.Show(Application.Current.MainWindow,
                String.Format("Lyric document has been generated:{0}{0}{1}{0}{0}Do you want to open it?",
                    Environment.NewLine, fileName),
                Application.Current.MainWindow.Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (Process process = Process.Start(fileName)) { }
            }

            AppState.Current.IsBusy = false;
        }
    }
}