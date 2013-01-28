using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

		private void StackOnCollectionChanged(
			object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs )
		{
			var y = calculator.Y;
			RegY.Text = y.Equals( FSharpOption<decimal>.None )
				? string.Empty
				: y.Value.ToString();
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
			InputState.Push( content );
		}

		private RoutedEventHandler OpOnClick( Operation op )
		{
			//TODO: DRY
			calculator.Push( InputState.ToDecimal() );
			InputState.Reset();

			return ( s, e ) => calculator.Perform( op );
		}
	}
}