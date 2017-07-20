using System;
using System.Linq;
using AutoScraper.Interfaces;

namespace AutoScraper.ExcludeFilters {
    public class ExcludeFilesFilter : IExcludeFilter {
        private static readonly string[] FileTypes = {
            ".asx", // Windows video
            ".bmp", // bitmap image
            ".js", // Javascript
            ".css", // Cascading Style Sheet
            ".doc", // Microsoft Word (mostly)
            ".docx", // Microsoft Word
            ".flv", // Old Flash video format
            ".gif", // GIF image
            ".jpeg", // JPEG image
            ".jpg", // JPEG image
            ".mid", // MIDI file
            ".mov", // Quicktime movie
            ".mp3", // MP3 audio
            ".ogg", // .ogg format media
            ".pdf", // PDF files
            ".png", // image
            ".ppt", // powerpoint
            ".ra", // real media
            ".ram", // real media
            ".rm", // real media
            ".swf", // Flash files
            ".txt", // plain text
            ".wav", // WAV format sound
            ".wma", // Windows media audio
            ".wmv", // Windows media video
            ".xml", // XML files
            ".zip", // ZIP files
            ".m4a", // MP4 audio
            ".m4v", // MP4 video
            ".mov", // Quicktime movie
            ".mp4", // MP4 video or audio
            ".m4b", // MP4 video or audio
            "/www.pinterest.com/",
            "/wp-json/",
            "replytocom=",
            "/feed/",
            "/?p=",
            "/tag/",
            "/2017/",
            "/2016/",
            "/2015/",
            "/2014/",
            "/2013/",
            "/2012/",
            "/2011/",
            "/2010/",
            "/2009/",
            "/2008/",
            "/2007/",
            "/2006/",
            "/2005/",
            "/2004/",
            "/2003/",
            "/2002/",
            "/2001/",
            "/2000/",

        };

        public bool IsMatch(Uri url) {
            return FileTypes.Any(url.AbsoluteUri.Contains);
        }
    }
}