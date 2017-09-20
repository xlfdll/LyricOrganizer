using System;
using System.IO;
using System.Text;

namespace LyricOrganizer
{
	public class TextLyricWriter : ILyricWriter
	{
		public TextLyricWriter(LyricContent lyricContent)
		{
			this.LyricContent = lyricContent;

			this.Encoding = Encoding.UTF8;
			this.WriteAdditionalInformation = true;
		}

		public LyricContent LyricContent { get; }

		public void WriteTo(String fileName)
		{
			File.WriteAllText(fileName, this.Generate(), this.Encoding);
		}

		public Encoding Encoding { get; set; }
		public Boolean WriteAdditionalInformation { get; set; }

		public String Generate()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(this.LyricContent.Title);
			sb.AppendLine(this.LyricContent.Artist);
			sb.AppendLine();

			if (this.WriteAdditionalInformation)
			{
				sb.AppendLine(this.LyricContent.Composer);
				sb.AppendLine(this.LyricContent.Writer);
				sb.AppendLine();
			}

			foreach (String line in this.LyricContent.Lyric)
			{
				sb.AppendLine(line);
			}

			return sb.ToString();
		}
	}
}