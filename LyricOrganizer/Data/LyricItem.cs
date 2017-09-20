using System;

namespace LyricOrganizer
{
	public class LyricItem
	{
		public LyricItem(String songTitle, String songArtist, ILyricProvider provider, Uri uri)
		{
			this.SongTitle = songTitle;
			this.SongArtist = songArtist;
			this.Provider = provider;
			this.URI = uri;
		}

		public String SongTitle { get; }
		public String SongArtist { get; }
		public ILyricProvider Provider { get; }
		public Uri URI { get; }
	}
}