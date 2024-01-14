namespace Marvel
{
    public partial class MainPage : ContentPage
    {
        MainViewModel mainViewModel;

        public MainPage()
        {
            InitializeComponent();
            mainViewModel = new MainViewModel();
            BindingContext = mainViewModel;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await mainViewModel.Initialize();
        }

    }
}
