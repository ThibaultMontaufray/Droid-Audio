/*
 * User: Thibault MONTAUFRAY
 */
using System;
using System.Collections.Generic;

namespace Droid_Audio
{
	/// <summary>
	/// Description of Artist.
	/// </summary>
	public class Artist
	{
		#region Attribute
		private List<Album> listAlbum;
		private string name;
		#endregion
		
		#region Properties
		public List<Album> ListAlbum
		{
			get 
			{
				if(listAlbum.Count == 0)
				{
					Album alb = new Album(this);
					alb.Name = "No album";
					listAlbum.Add(alb);
				}
				return listAlbum; 
			}
			set { listAlbum = value; }
		}
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		#endregion
		
		#region Constructor
		public Artist(string art_name)
		{
			name = art_name;
			listAlbum = new List<Album>();
		}
		#endregion
		
		#region Methods
		public void AddAlbum(string path)
		{
			Album album = new Album(this);
			album.Name = path.Split('\\')[path.Split('\\').Length -1];
			listAlbum.Add(album);
		}
		
		public Album GetLastAlbum()
		{
			if(listAlbum.Count == 0)
			{
				AddAlbum("");
				return listAlbum[0];
			}
			else return listAlbum[listAlbum.Count - 1];
		}
		#endregion
	}
}
