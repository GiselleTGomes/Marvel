namespace Marvel;

public partial class CharacterPage : ContentPage
{
	public CharacterPage()
	{
		InitializeComponent();
		BindingContext = new CharacterViewModel();
    }
}