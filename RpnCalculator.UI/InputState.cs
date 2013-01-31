﻿using System;
using System.ComponentModel;
using System.Globalization;
using RpnCalculator.Logic;
using RpnCalculator.UI.Annotations;

namespace RpnCalculator.UI
{
	public class InputState : INotifyPropertyChanged
	{
		private string valueString;
		private Position currentPosition;
		private bool isGhost;
		private bool isResult;
		private Entry entry;

		public Sign CurrentSign
		{
			get { return Entry.Value >= 0 ? Sign.Positive : Sign.Negative; }
		}

		public Position CurrentPosition
		{
			get { return currentPosition; }
			set
			{
				if ( value == currentPosition )
					return;
				currentPosition = value;
				OnPropertyChanged( "CurrentPosition" );
			}
		}

		public string ValueString
		{
			get { return valueString; }
			set
			{
				if ( value == valueString )
					return;
				valueString = value;
				OnPropertyChanged( "ValueString" );
			}
		}

		public bool IsGhost
		{
			get { return isGhost; }
			set
			{
				if ( value.Equals( isGhost ) )
					return;
				isGhost = value;
				OnPropertyChanged( "IsGhost" );
			}
		}

		public bool IsResult
		{
			get { return isResult; }
			set
			{
				if ( value.Equals( isResult ) )
					return;
				isResult = value;
				OnPropertyChanged( "IsResult" );
			}
		}

		public Entry Entry
		{
			get { return entry; }
			set
			{
				if ( Equals( value, entry ) )
					return;
				entry = value;
				OnPropertyChanged( "Entry" );
				OnPropertyChanged( "CurrentSign" );
			}
		}

		public enum Sign
		{
			Positive = 0,
			Negative = 1
		}

		public enum Position
		{
			Integer = 0,
			Decimal = 1
		}

		public InputState( Entry entry )
		{
			Entry = entry;
			ValueString = entry.Value.ToString( "0.####################", CultureInfo.InvariantCulture );
			CurrentPosition = Entry.Value - Math.Truncate( Entry.Value ) > 0 ? Position.Decimal : Position.Integer;
		}

		public override string ToString()
		{
			return string.Format(
				"{0}{1}",
				CurrentSign == Sign.Negative ? "-" : string.Empty,
				ValueString );
		}

		public decimal ToDecimal()
		{
			return decimal.Parse( ValueString, CultureInfo.InvariantCulture );
		}

		public void Push( char character )
		{
			if ( IsGhost )
			{
				Reset();
			}

			if ( character == '.' && CurrentPosition == Position.Decimal )
			{
				return;
			}

			if ( character == '.' )
			{
				ValueString += ".";
				CurrentPosition = Position.Decimal;
				return;
			}

			if ( char.IsDigit( character ) == false )
			{
				return;
			}

			ValueString += character;
			Entry.Value = ToDecimal();
		}

		public void Backspace()
		{
			ValueString = ValueString.Substring( 0, ValueString.Length - 1 );

			if ( ValueString.LastIndexOf( '.' ) == ValueString.Length )
			{
				ValueString = ValueString.Substring( 0, ValueString.Length - 1 );
			}

			Entry.Value = decimal.Parse( ValueString, CultureInfo.InvariantCulture );

			CurrentPosition = Entry.Value - Math.Truncate( Entry.Value ) > 0
				? Position.Decimal
				: Position.Integer;
		}

		public void Reset()
		{
			IsGhost = false;
			CurrentPosition = Position.Integer;
			ValueString = string.Empty;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged( string propertyName )
		{
			var handler = PropertyChanged;
			if ( handler != null )
				handler( this, new PropertyChangedEventArgs( propertyName ) );
		}
	}
}