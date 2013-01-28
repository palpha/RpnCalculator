using System;
using System.ComponentModel;
using System.Globalization;
using RpnCalculator.UI.Annotations;

namespace RpnCalculator.UI
{
	public class InputState : INotifyPropertyChanged
	{
		private int? integerValue;
		private Sign currentSign;
		private Position currentPosition;
		private int? decimalValue;

		public Sign CurrentSign
		{
			get { return currentSign; }
			set
			{
				currentSign = value;
				Notify( "CurrentSign" );
			}
		}

		public Position CurrentPosition
		{
			get { return currentPosition; }
			set
			{
				currentPosition = value;
				Notify( "CurrentPosition" );
			}
		}

		public int? Integer
		{
			get { return integerValue; }
			set
			{
				integerValue = value;
				Notify( "Integer" );
			}
		}

		public int? Decimal
		{
			get { return decimalValue; }
			set
			{
				decimalValue = value;
				Notify( "Decimal" );
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

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void Notify( string propertyName )
		{
			var handler = PropertyChanged;
			if ( handler != null )
				handler( this, new PropertyChangedEventArgs( propertyName ) );
		}

		public override string ToString()
		{
			return string.Format(
				"{0}{1}{2}{3}",
				CurrentSign == Sign.Negative ? "-" : string.Empty,
				Integer.HasValue ? Integer.Value.ToString( CultureInfo.InvariantCulture ) : string.Empty,
				CurrentPosition == Position.Decimal ? "." : string.Empty,
				Decimal.HasValue ? Decimal.Value.ToString( CultureInfo.InvariantCulture ) : string.Empty );
		}

		public decimal ToDecimal()
		{
			var intPart = Integer ?? 0;
			var decPart = Decimal ?? 0;

			// Yes, we have no math skills!
			return
				(CurrentSign == Sign.Negative ? -intPart : intPart)
					+ decPart / (decimal) Math.Pow( 10, decPart.ToString( CultureInfo.InvariantCulture ).Length );
		}

		public void Push( string character )
		{
			if ( character == "." && CurrentPosition == Position.Decimal )
			{
				return;
			}

			if ( character == "." )
			{
				CurrentPosition = Position.Decimal;
				return;
			}

			if ( CurrentPosition == Position.Integer )
			{
				Integer = Append( Integer, character );
			}
			else
			{
				Decimal = Append( Decimal, character );
			}
		}

		public void Backspace()
		{
			if ( CurrentPosition == Position.Integer )
			{
				Integer = Backspace( Integer );
			}
			else if ( Decimal.HasValue )
			{
				Decimal = Backspace( Decimal );
			}
			else
			{
				CurrentPosition = Position.Integer;
			}
		}

		private int? Backspace( int? part )
		{
			if ( part.HasValue == false )
			{
				return null;
			}

			var partStr = part.Value.ToString( CultureInfo.InvariantCulture );
			int result;
			return int.TryParse( partStr.Substring( 0, partStr.Length - 1 ), out result )
				? result
				: (int?) null;
		}

		private int? Append( int? part, string character )
		{
			var partStr = part.HasValue
				? part.Value.ToString( CultureInfo.InvariantCulture )
				: string.Empty;

			var result = int.Parse( partStr + character );
			return result > 0
				? result
				: (int?) null;
		}

		public void Reset()
		{
			CurrentSign = Sign.Positive;
			CurrentPosition = Position.Integer;
			Integer = null;
			Decimal = null;
		}
	}
}