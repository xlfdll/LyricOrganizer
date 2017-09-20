using System;

namespace LyricOrganizer
{
	public class LyricContent
	{
		public LyricContent(String title, String artist, String writer, String composer, String[] lyric, LyricItem item)
		{
			this.Title = title;
			this.Artist = artist;
			this.Writer = writer;
			this.Composer = composer;
			this.Lyric = lyric;
			this.Item = item;
		}

		public String Title { get; }
		public String Artist { get; }
		public String Writer { get; }
		public String Composer { get; }
		public String[] Lyric { get; }
		public LyricItem Item { get; }
	}
}