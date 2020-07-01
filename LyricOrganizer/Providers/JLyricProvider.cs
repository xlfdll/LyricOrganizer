using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Xlfdll.Network;

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
            List<LyricItem> lyricItems = new List<LyricItem>();

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

            List<dynamic> results = new List<dynamic>();
            String html = WebOperations.GetContentAsString(JLyricProvider.QueryURI.ToString(), query, Encoding.UTF8);

            if (type == LyricSearchType.ByArtist)
            {
                foreach (Match match in JLyricProvider.ArtistSearchParseRegex.Matches(html))
                {
                    html = WebOperations.GetContentAsString(match.Groups["ArtistURL"].Value, Encoding.UTF8);

                    results.AddRange(JLyricProvider.ArtistSongSearchParseRegex.Matches(html)
                        .OfType<Match>()
                        .Select(m => new
                        {
                            Title = m.Groups["SongTitle"].Value,
                            Artist = match.Groups["ArtistName"].Value,
                            URL = JLyricProvider.RootURI.GetLeftPart(UriPartial.Authority) + match.Groups["SongRelativeURL"].Value
                        }));
                }
            }
            else
            {
                results.AddRange(JLyricProvider.SongSearchParseRegex.Matches(html)
                    .OfType<Match>()
                    .Select(m => new
                    {
                        Title = m.Groups["SongTitle"].Value,
                        Artist = m.Groups["SongArtist"].Value,
                        URL = m.Groups["SongURL"].Value
                    }));
            }

            foreach (dynamic r in results)
            {
                String songName = r.Title;
                String songArtist = r.Artist;
                Uri uri = new Uri(r.URL);

                lyricItems.Add(new LyricItem(songName, songArtist, this, uri));
            }

            return lyricItems.AsReadOnly();
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

        private static readonly Uri RootURI = new Uri("http://j-lyric.net");
        private static readonly Uri QueryURI = new Uri("http://search.j-lyric.net/index.php");
        private static readonly Regex SongSearchParseRegex = new Regex(@"<p class=""mid""><a href=""(?<SongURL>.+?)"" title.+?>(?<SongTitle>.+?)</a></p><p class=""sml"">歌：<a href="".+?"" title.+?>(?<SongArtist>.+?)</a></p>", RegexOptions.Compiled);
        private static readonly Regex ArtistSearchParseRegex = new Regex(@"<a class=""artist"" href=""(?<ArtistURL>.+?)"" title.+?>(?<ArtistName>.+?)</a>", RegexOptions.Compiled);
        private static readonly Regex ArtistSongSearchParseRegex = new Regex(@"<p class=""ttl""><a href=""(?<SongRelativeURL>.+?)"" title.+?>(?<SongTitle>.+?)</a></p>", RegexOptions.Compiled);
        private static readonly Regex LyricParseRegex = new Regex(@"</p><p class=""sml"">(?<Writer>.+)</p><p class=""sml"">(?<Composer>.+)</p><p id=""Lyric"">(?<Lyric>.+?)</p>", RegexOptions.Compiled);
    }
}