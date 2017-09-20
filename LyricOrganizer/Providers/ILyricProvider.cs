using System;
using System.Collections.ObjectModel;

namespace LyricOrganizer
{
	public interface ILyricProvider
	{
		String Name { get; }

		ReadOnlyCollection<LyricItem> Search(String keyword, LyricSearchType type);
		ReadOnlyCollection<LyricItem> Search(String keyword, LyricSearchType type, Int32? page);

		LyricContent Retrieve(LyricItem item);
	}
}