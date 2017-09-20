using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

using Xlfdll.Core.Network;

namespace LyricOrganizer
{
	public class JLyricProvider : ILyricProvider
	{
		public String Name => "J-Lyric.net";

		public ReadOnlyCollection<LyricItem> Search(String keyword, LyricSearchType type)
		{
			return this.Search(keyword, type, null);
		}

		public ReadOnlyCollection<LyricItem> Search(String keyword, LyricSearchType type, Int32? page)
		{
			List<LyricItem> results = new List<LyricItem>();

			Dictionary<String, String> query = new Dictionary<String, String>();

			switch (type)
			{
				case LyricSearchType.BySongName:
					{
						query.Add("kt", keyword);
						break;
					}
				case LyricSearchType.ByArtist:
					{
						query.Add("ka", keyword);
						break;
					}
				case LyricSearchType.ByLyric:
					{
						query.Add("kl", keyword);
						break;
					}
				default:
					break;
			}

			query.Add("ct", "2");

			if (page != null)
			{
				query.Add("p", page.ToString());
			}

			String result = WebOperations.GetContentAsString(JLyricProvider.QueryURI.ToString(), Encoding.UTF8, query);

			foreach (Match match in JLyricProvider.SearchParseRegex.Matches(result))
			{
				String songName = match.Groups["SongTitle"].Value;
				String songArtist = match.Groups["SongArtist"].Value;
				Uri uri = new Uri(JLyricProvider.QueryURI.GetLeftPart(UriPartial.Authority) + match.Groups["RelativePath"].Value);

				results.Add(new LyricItem(songName, songArtist, this, uri));
			}

			return results.AsReadOnly();
		}

		public LyricContent Retrieve(LyricItem item)
		{
			if (item.Provider.Name != this.Name)
			{
				throw new NotSupportedException("The lyric item data was not from the current provider.");
			}

			String html = WebOperations.GetContentAsString(item.URI.ToString(), Encoding.UTF8);

			Match match = JLyricProvider.LyricParseRegex.Match(html);

			String title = item.SongTitle;
			String artist = item.SongArtist;
			String writer = match.Groups["Writer"].Value;
			String composer = match.Groups["Composer"].Value;
			String[] lyric = match.Groups["Lyric"].Value.Split(new String[] { "<br>" }, StringSplitOptions.None);

			return new LyricContent(title, artist, writer, composer, lyric, item);
		}

		private static readonly Uri QueryURI = new Uri("http://j-lyric.net/index.php");
		private static readonly Regex SearchParseRegex = new Regex(@"<div class='title'>\s*<a href='(?<RelativePath>.+)'>(?<SongTitle>.+)</a></div>\s*<div class='status'>\s*.+<a href='.+'>(?<SongArtist>.+)</a>", RegexOptions.Compiled);
		private static readonly Regex LyricParseRegex = new Regex(@"</p><p class=""sml"">(?<Writer>.+)</p><p class=""sml"">(?<Composer>.+)</p><p id=""Lyric"">(?<Lyric>.+?)</p>", RegexOptions.Compiled);
	}
}