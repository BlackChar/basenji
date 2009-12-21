/// SearchEntry.cs
// 
// Copyright (C) 2009 Patrick Ulbrich
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

using System;
using GLib;
using Basenji.Icons;

namespace Basenji.Gui.Widgets
{
	public class SearchEntry : IconEntry
	{
		private string placeholderText;
		
		public SearchEntry () {
			this.placeholderText = null;
			
			this.SetIconFromStock(Icon.Stock_Find.Name,
			                      EntryIconPosition.Primary);
		
			this.KeyPressEvent	+= OnKeyPressEvent;
			this.Changed		+= OnChanged;
			this.IconPress		+= OnIconPressEvent;
			this.Shown			+= OnShown;
			this.FocusInEvent	+= OnFocusInEvent;
			this.FocusOutEvent	+= OnFocusOutEvent;
		}
		
		public string PlaceholderText {
			get { return placeholderText; }
			set {
				placeholderText = value;
				if (Parent != null) {
					// Only applies if the widget has a parent yet.
					// if not, OnShown() calls this method anyways.
					SetPlaceholderText(true);
				}
			}
		}
		
		protected virtual void OnSearch() {
			if (Search != null)
				Search(this, new SearchEventArgs(Text));
		}
		
		public event SearchEventHandler Search;
		
		private void SetPlaceholderText(bool set) {
			Gdk.Color a = Parent.Style.Base(Gtk.StateType.Normal);
			Gdk.Color b = Parent.Style.Text(Gtk.StateType.Normal);
			
			if (set) {
				if ((Text.Length == 0) && !string.IsNullOrEmpty(placeholderText)) {
					ModifyText(Gtk.StateType.Normal, ColorBlend(a, b));
					Text = placeholderText;
				}
			} else {
				ModifyText(Gtk.StateType.Normal, b);
				if (IsPlaceholderTextActive())
					Text = string.Empty;
			}
		}
		
		private bool IsPlaceholderTextActive() {
			return (Text.Length > 0) && (Text == placeholderText);
		}
		
		// Copied from Banshee.Hyena.Gui.GtkUtilities
		// Copyright (C) 2007 Aaron Bockover <abockover@novell.com>
		private static Gdk.Color ColorBlend(Gdk.Color a, Gdk.Color b) {
			// at some point, might be nice to allow any blend?
			double blend = 0.5;
			
			if (blend < 0.0 || blend > 1.0) {
				throw new ApplicationException ("blend < 0.0 || blend > 1.0");
			}
			
			double blendRatio = 1.0 - blend;
			
			int aR = a.Red >> 8;
			int aG = a.Green >> 8;
			int aB = a.Blue >> 8;
			
			int bR = b.Red >> 8;
			int bG = b.Green >> 8;
			int bB = b.Blue >> 8;
			
			double mR = aR + bR;
			double mG = aG + bG;
			double mB = aB + bB;
			
			double blR = mR * blendRatio;
			double blG = mG * blendRatio;
			double blB = mB * blendRatio;
			
			Gdk.Color color = new Gdk.Color ((byte)blR, (byte)blG, (byte)blB);
			Gdk.Colormap.System.AllocColor (ref color, true, true);
			
			return color;
        }
		
		[GLib.ConnectBefore()]
		private void OnKeyPressEvent(object o, Gtk.KeyPressEventArgs args) {                    
			if (args.Event.Key != Gdk.Key.Return)
				return;
			
			// update search results
			OnSearch();
		}
		
		private void OnIconPressEvent(object o, IconPressReleaseEventArgs args) {
			if (IsPlaceholderTextActive()) {
				GrabFocus();
			} else {				
				// clear-button pressed
				if (args.IconPos == EntryIconPosition.Secondary)
					Text = String.Empty;
				
				// update search results
				OnSearch();
			}
		}
		
		private void OnChanged(object o, EventArgs args) {
			if ((Text.Length > 0) && !IsPlaceholderTextActive())
				SetIconFromStock(Icon.Stock_Clear.Name,
				                 EntryIconPosition.Secondary);
			else
				SetIconFromStock(null,
				                 EntryIconPosition.Secondary);
		}
		
		private void OnShown(object o, EventArgs e) {
			SetPlaceholderText(true);
		}
		
		void OnFocusInEvent (object o, Gtk.FocusInEventArgs args) {
			SetPlaceholderText(false);
		}
		
		void OnFocusOutEvent (object o, Gtk.FocusOutEventArgs args) {
			SetPlaceholderText(true);
		}
	}
	
	public delegate void SearchEventHandler(object o, SearchEventArgs args);
	
	public class SearchEventArgs : EventArgs
	{	
		private string searchString;
		
		public SearchEventArgs(string searchString) : base() {
			this.searchString = searchString;
		}
		
		public string SearchString { get {return searchString; } }
	}
}