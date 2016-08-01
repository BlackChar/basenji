// ThumbnailGenerator.cs
// 
// Copyright (C) 2008, 2012, 2016 Patrick Ulbrich
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

// This file contains code generated by the Gtk# code generator.

#if GNOME
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Platform.Gnome
{
	internal class ThumbnailGenerator : GLib.Object, Platform.Common.IThumbnailGenerator
	{	
		private enum DesktopThumbnailSize  {
			Normal,
			Large,
		}

		private const DesktopThumbnailSize THUMB_SIZE = DesktopThumbnailSize.Normal; // 100 pix

		private GLib.Object thumbnail = null;
		private bool disposed = false;

		public ThumbnailGenerator(IntPtr raw) : base(raw) {}

		public ThumbnailGenerator() {
			Raw = gnome_desktop_thumbnail_factory_new((int)THUMB_SIZE);
		}
		
		public bool GenerateThumbnail(FileInfo fi, string mimeType) {
			if (thumbnail != null) {
				thumbnail.Dispose();
				thumbnail = null;
			}
			
			string uri = new Uri(fi.FullName).ToString();
			if (CanThumbnail(uri, mimeType, fi.LastWriteTime)) {
				thumbnail = GenerateThumbnail(uri, mimeType);
				if (thumbnail != null)
					return true;
			}
			return false;
		}
		
		public void SaveThumbnail(string filename) {
			if (thumbnail == null)
				throw new InvalidOperationException("No thumbnail generated");
			
			IntPtr native_filename = GLib.Marshaller.StringToPtrGStrdup (filename);
			IntPtr native_type = GLib.Marshaller.StringToPtrGStrdup ("png");
			gdk_pixbuf_save(thumbnail.Handle, native_filename, native_type, IntPtr.Zero, IntPtr.Zero);
			GLib.Marshaller.Free(native_filename);
			GLib.Marshaller.Free(native_type);

		}

		public override void Dispose() {
			Dispose(true);
			base.Dispose();
		}

		private void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing && (thumbnail != null)) {
					thumbnail.Dispose();
				}
				thumbnail = null;
			}
			disposed = true;
		}

		private bool CanThumbnail(string uri, string mime_type, System.DateTime mtime) {
			IntPtr native_uri = GLib.Marshaller.StringToPtrGStrdup (uri);
			IntPtr native_mime_type = GLib.Marshaller.StringToPtrGStrdup (mime_type);
			bool raw_ret = gnome_desktop_thumbnail_factory_can_thumbnail(Handle, native_uri, native_mime_type, GLib.Marshaller.DateTimeTotime_t (mtime));
			bool ret = raw_ret;
			GLib.Marshaller.Free (native_uri);
			GLib.Marshaller.Free (native_mime_type);
			return ret;
		}

		private GLib.Object GenerateThumbnail(string uri, string mime_type) {
			IntPtr native_uri = GLib.Marshaller.StringToPtrGStrdup (uri);
			IntPtr native_mime_type = GLib.Marshaller.StringToPtrGStrdup (mime_type);
			IntPtr raw_ret = gnome_desktop_thumbnail_factory_generate_thumbnail(Handle, native_uri, native_mime_type);
			GLib.Object ret = GLib.Object.GetObject(raw_ret);
			GLib.Marshaller.Free (native_uri);
			GLib.Marshaller.Free (native_mime_type);
			return ret;
		}

		[DllImport("gnome-desktop-3")]
		private static extern IntPtr gnome_desktop_thumbnail_factory_new(int size);

		[DllImport("gnome-desktop-3")]
		private static extern bool gnome_desktop_thumbnail_factory_can_thumbnail(IntPtr raw, IntPtr uri, IntPtr mime_type, IntPtr mtime);

		[DllImport("gnome-desktop-3")]
		private static extern IntPtr gnome_desktop_thumbnail_factory_generate_thumbnail(IntPtr raw, IntPtr uri, IntPtr mime_type);
	
		[DllImport("libgdk-pixbuf2")]
		private static extern bool gdk_pixbuf_save(IntPtr raw, IntPtr filename, IntPtr type, IntPtr error, IntPtr var_args);
	}
}
#endif