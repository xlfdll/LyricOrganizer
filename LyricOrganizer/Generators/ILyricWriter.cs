using System;

namespace LyricOrganizer
{
	public interface ILyricWriter
    {
		LyricContent LyricContent { get; }

		void WriteTo(String fileName);
    }
}