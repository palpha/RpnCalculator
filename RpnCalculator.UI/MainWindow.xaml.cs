using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.FSharp.Core;
using RpnCalculator.Collections;
using RpnCalculator.Logic;

namespace RpnCalculator.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Random rnd = new Random();
		private readonly Calculator calculator = new Calculator();

		private readonly IDictionary<Key, char> inputMap = new Dictionary<Key, char> { };
		private InputState inputState;

		public ObservableStack<Entry> Stack
		{
			get { return calculator.Stack; }
		}

		public InputState InputState
		{
			get { return inputState; }
			set
			{
				if ( inputState != null )
				{
					inputState.PropertyChanged -= InputStateOnPropertyChanged;
				}

				inputState = value;
				inputState.PropertyChanged += InputStateOnPropertyChanged;

				RegX.Text = inputState.ToString();
				ResizeGridViewColumn( StackValues );
			}
		}

		private void InputStateOnPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if ( e.PropertyName != "ValueString" )
			{
				return;
			}

			RegX.Text = InputState.ToString();
			ResizeGridViewColumn( StackValues );
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e );

			SetNewInputState();

			WireNumpadButtons();
			CreateNumericInputMap();

			Num0.Click += NumPadOnClick;
			Decimal.Click += NumPadOnClick;

			Back.Click += BackOnClick;
			Push.Click += PushOnClick;

			Add.Click += OpOnClick( Operation.Addition );
			Sub.Click += OpOnClick( Operation.Subtraction );
			Mul.Click += OpOnClick( Operation.Multiplication );
			Div.Click += OpOnClick( Operation.Division );

			StackView.ItemsSource = Stack;
			Stack.CollectionChanged += StackOnCollectionChanged;

			GotFocus += DisableButtonFocus;
		}

		private void DisableButtonFocus( object sender, RoutedEventArgs e )
		{
			var button = e.Source as Button;
			if ( button == null || Equals( button, Push ) )
				return;

			Push.Focus();
			e.Handled = true;
		}

		protected override void OnPreviewKeyDown( KeyEventArgs e )
		{
			if ( e.Key == Key.Tab )
			{
				e.Handled = true;
			}

			base.OnPreviewKeyDown( e );
		}

		private void CreateNumericInputMap()
		{
			Func<string, int, Key> getKey =
				( prefix, num ) => (Key) Enum.Parse(
					typeof( Key ),
					prefix + num.ToString( CultureInfo.InvariantCulture ) );

			Func<int, char> getChar = x => x.ToString( CultureInfo.InvariantCulture )[0];

			Enumerable.Range( 0, 10 ).ToList()
				.ForEach( x =>
					{
						inputMap.Add( getKey( "D", x ), getChar( x ) );
						inputMap.Add( getKey( "NumPad", x ), getChar( x ) );
					} );
		}

		private void WireNumpadButtons()
		{
			foreach ( var btn in
				from i in new[]
					{
						7, 8, 9,
						4, 5, 6,
						1, 2, 3
					}
				select new Button
					{
						Name = "Num" + i,
						Content = i,
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Top,
						Width = 20
					} )
			{
				btn.Click += NumPadOnClick;
				PositiveNumbersPanel.Children.Add( btn );
			}
		}

		private void StackOnCollectionChanged(
			object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs )
		{
			RegY.Text = EntryToString( calculator.Y );
			RegZ.Text = EntryToString( calculator.Z );
			RegT.Text = EntryToString( calculator.T );
		}

		private static string EntryToString( FSharpOption<Entry> entry )
		{
			return FSharpOption<Entry>.get_IsNone( entry )
				? string.Empty
				: entry.Value.Value.ToString( CultureInfo.InvariantCulture );
		}

		private void BackOnClick( object sender, RoutedEventArgs e )
		{
			InputState.Backspace();
		}

		private void PushOnClick( object sender, RoutedEventArgs e )
		{
			SetGhostInputState();
		}

		private void NumPadOnClick( object sender, RoutedEventArgs e )
		{
			var btn = e.Source as Button;
			if ( btn == null )
				return;

			var content = btn.Content.ToString();

			PushToInputState( content[0] );
		}

		private RoutedEventHandler OpOnClick( Operation op )
		{
			return ( s, e ) =>
				{
					calculator.Perform( op );
					SetResultInputState();
				};
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			var shiftIsDown = (e.KeyboardDevice.GetKeyStates( Key.LeftShift )
				| e.KeyboardDevice.GetKeyStates( Key.RightShift )).HasFlag( KeyStates.Down );

			do
			{
				switch ( e.Key )
				{
					case Key.Enter:
						if ( shiftIsDown )
						{
							calculator.Perform( Operation.Swap );
							SetResultInputState();
						}
						else
						{
							SetGhostInputState();
						}

						break;
					case Key.Back:
						if ( shiftIsDown )
						{
							calculator.Perform( Operation.Drop );
							SetResultInputState();
						}
						else
						{
							InputState.Backspace();
						}
						break;
					case Key.Oem2:
					case Key.Divide:
						calculator.Perform( Operation.Division );
						SetResultInputState();
						break;
					case Key.Multiply:
						calculator.Perform( Operation.Multiplication );
						SetResultInputState();
						break;
					case Key.OemMinus:
					case Key.Subtract:
						if ( shiftIsDown )
						{
							InputState.Invert();
						}
						else
						{
							calculator.Perform( Operation.Subtraction );
							SetResultInputState();
						}

						break;
					case Key.OemPlus:
					case Key.Add:
						calculator.Perform( Operation.Addition );
						SetResultInputState();
						break;
					case Key.OemPeriod:
					case Key.Decimal:
						PushToInputState( '.' );
						break;
					default:
						continue;
				}

				e.Handled = true;
			} while ( false );

			if ( !e.Handled && inputMap.ContainsKey( e.Key ) )
			{
				var input = inputMap[e.Key];
				PushToInputState( input );
				e.Handled = true;
			}

			base.OnKeyDown( e );
		}

		private void SetNewInputState()
		{
			var entry = new Entry( 0 );
			calculator.Push( entry );
			InputState = new InputState( entry ) { IsGhost = true };
		}

		private void SetResultInputState()
		{
			InputState = new InputState( calculator.X.Value ) { IsResult = true };
		}

		private void SetGhostInputState()
		{
			var entry = new Entry( calculator.X.Value.Value );
			calculator.Push( entry );
			InputState = new InputState( entry ) { IsGhost = true };
		}

		private void PushToInputState( char input )
		{
			if ( InputState.IsResult )
			{
				SetNewInputState();
			}

			InputState.Push( input );
		}

		private void ResizeGridViewColumn( GridViewColumn column )
		{
			if ( double.IsNaN( column.Width ) )
			{
				column.Width = column.ActualWidth;
			}

			column.Width = double.NaN;
		}
	}
}