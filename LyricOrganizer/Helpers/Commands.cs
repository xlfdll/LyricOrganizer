using System;
using System.ComponentModel;

using Microsoft.Win32;

using Xlfdll.Diagnostics;
using Xlfdll.IO;
using Xlfdll.Windows.Presentation;

namespace LyricOrganizer
{
    public static class Commands
    {
        static Commands()
        {
            Commands.SearchCommand = new RelayCommand<String>
            (
                delegate (String direction)
                {
                    AppState.Current.IsBusy = true;

                    if (Int32.Parse(direction) == 0)
                    {
                        AppState.Current.ActiveKeyword = AppState.Current.Keyword;
                        AppState.Current.Page = 1;
                    }
                    else
                    {
                        AppState.Current.Keyword = AppState.Current.ActiveKeyword;
                    }

                    using (BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true })
                    {
                        worker.DoWork += Routines.Search_DoWork;
                        worker.ProgressChanged += Routines.Search_ProgressChanged;
                        worker.RunWorkerCompleted += Routines.Search_RunWorkerCompleted;

                        worker.RunWorkerAsync(direction);
                    }
                }
            );

            Commands.GenerateDocumentCommand = new RelayCommand<LyricItem>
            (
                delegate (LyricItem item)
                {
                    SaveFileDialog dlg = new SaveFileDialog()
                    {
                        FileName = PathExtensions.GetSafeFileName($"{item.SongArtist} - {item.SongTitle}"),
                        Filter = "Text Document (*.txt)|*.txt|All Files (*.*)|*.*",
                        RestoreDirectory = true
                    };

                    if (dlg.ShowDialog() == true)
                    {
                        AppState.Current.IsBusy = true;

                        using (BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true })
                        {
                            worker.DoWork += Routines.Generate_DoWork;
                            worker.ProgressChanged += Routines.Generate_ProgressChanged;
                            worker.RunWorkerCompleted += Routines.Generate_RunWorkerCompleted;

                            worker.RunWorkerAsync(new Object[] { item, dlg.FileName });
                        }
                    }
                },
                delegate (LyricItem item)
                {
                    return (item != null);
                }
            );

            Commands.OpenLyricURLCommand = new RelayCommand<LyricItem>
            (
                delegate (LyricItem item)
                {
                    ProcessHelper.Start(item.URI.ToString());
                },
                delegate (LyricItem item)
                {
                    return (item != null);
                }
            );
        }

        public static RelayCommand<String> SearchCommand { get; }
        public static RelayCommand<LyricItem> GenerateDocumentCommand { get; }
        public static RelayCommand<LyricItem> OpenLyricURLCommand { get; }
    }
}