using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

		public ObservableStack<decimal> Stack
		{
			get { return calculator.Stack; }
		}

		public InputState InputState { get; set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e );

			InputState = new InputState();

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

			InputState.PropertyChanged += InputStateOnPropertyChanged;
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
			RegY.Text = OptionToString( calculator.Y );
			RegZ.Text = OptionToString( calculator.Z );
			RegT.Text = OptionToString( calculator.T );
		}

		private static string OptionToString( FSharpOption<decimal> opt )
		{
			return FSharpOption<decimal>.get_IsNone( opt )
				? string.Empty
				: opt.Value.ToString( CultureInfo.InvariantCulture );
		}

		private void BackOnClick( object sender, RoutedEventArgs e )
		{
			InputState.Backspace();
		}

		private void PushOnClick( object sender, RoutedEventArgs e )
		{
			calculator.Push( InputState.ToDecimal() );
			InputState.Reset();
		}

		private void InputStateOnPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			RegX.Text = InputState.ToString();
		}

		private void NumPadOnClick( object sender, RoutedEventArgs e )
		{
			var btn = e.Source as Button;
			var content = btn.Content.ToString();
			InputState.Push( content[0] );
		}

		private RoutedEventHandler OpOnClick( Operation op )
		{
			//TODO: DRY
			calculator.Push( InputState.ToDecimal() );
			InputState.Reset();

			return ( s, e ) => calculator.Perform( op );
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			do
			{
				switch ( e.Key )
				{
					case Key.Enter:
						calculator.Push( InputState.ToDecimal() );
						InputState.Reset();
						break;
					case Key.Back:
						InputState.Backspace();
						break;
					case Key.Divide:
						calculator.Push( InputState.ToDecimal() );
						calculator.Perform( Operation.Division );
						break;
					case Key.Multiply:
						calculator.Push( InputState.ToDecimal() );
						calculator.Perform( Operation.Multiplication );
						break;
					case Key.Subtract:
						calculator.Push( InputState.ToDecimal() );
						calculator.Perform( Operation.Subtraction );
						break;
					case Key.Add:
						calculator.Push( InputState.ToDecimal() );
						calculator.Perform( Operation.Addition );
						break;
					default:
						continue;
				}

				e.Handled = true;
			} while ( false );

			if ( !e.Handled && inputMap.ContainsKey( e.Key ) )
			{
				var input = inputMap[e.Key];
				InputState.Push( input );
				e.Handled = true;
			}

			base.OnKeyDown( e );
		}
	}
}