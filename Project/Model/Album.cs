/*
 * User: Thibault MONTAUFRAY
 */
using System;
using System.Collections.Generic;

namespace Droid_Audio
{
	/// <summary>
	/// Description of Album.
	/// </summary>
	public class Album
	{
		#region Attributes
		private List<Track> listTrack;
		private string path_cover_smart;
		private string path_cover_large;
		private string name;
		private Artist artist;
		#endregion
		
		#region Properties
		public List<Track> ListTrack
		{
			get { return listTrack; }
			set { listTrack = value; }
		}
		public string Path_cover_smart
		{
			get { return path_cover_smart; }
			set { path_cover_smart = value; }
		}
		public string Path_cover_large
		{
			get { return path_cover_large; }
			set { path_cover_large = value; }
		}
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		public Artist My_Artist
		{
			get { return artist; }
			set { artist = value; }
		}
		#endregion
		
		#region Constructor
		public Album(Artist art)
		{
			name = "Single track";
			artist = art;
			listTrack = new List<Track>();
		}
		#endregion
		
		#region Methods
		public void Add(Track t)
		{
			listTrack.Add(t);
		}
		#endregion
	}
}
