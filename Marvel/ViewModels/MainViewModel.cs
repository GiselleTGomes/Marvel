using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Marvel
{
    public class MainViewModel : ObservableObject
    {
        #region Fields
        private const string baseUrl = "https://gateway.marvel.com/v1/public/";
        private const string PublicKey = "fc2d2a5c3d38d16310d99a41c5f078b2";
        private const string PrivateKey = "a46eb3eedd8ea003d8a7daa7b45353cc90d18d15";

        private readonly HttpClient _httpClient;

        private int total;
        private Character character;
        private string search;
        private int pages;

        private bool isLoading;
        #endregion




        public Character Character
        {
            get => character;
            set => SetProperty(ref character, value);
        }
        public ObservableCollection<Character> Characters { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ObservableCollection<int> Amount { get; set; }

        public string Search
        {
            get => search;
            set => SetProperty(ref search, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public MainViewModel()
        {
            _httpClient = new HttpClient();

            Characters = new ObservableCollection<Character>();
            Amount = new ObservableCollection<int>();
            SelectCommand = new Command(SelectCharacter);
            SearchCommand = new Command(OnSearch);

        }


        public async Task<MarvelResponse> GetServiceAsync(int offset, int limit, string nameFilter = "")
        {
            string endpoint = "characters";
            string timestamp = DateTime.Now.Ticks.ToString();
            string hash = GenerateMd5Hash($"{timestamp}{PrivateKey}{PublicKey}");

            string url = $"{baseUrl}{endpoint}?apikey={PublicKey}&ts={timestamp}&hash={hash}&offset={offset}&limit={limit}";

            if (!string.IsNullOrEmpty(nameFilter))
            {
                url += $"&nameStartsWith={nameFilter}";
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MarvelResponse>(json);
            }
            else
            {
                throw new HttpRequestException($"Status code: {response.StatusCode}");
            }
        }

        public static string GenerateMd5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public async Task Initialize()
        {
            if (Characters.Count == 0)
            {
                await Load();

                pages = (int)Math.Ceiling((double)total / 4);

                for (int i = 0; i < pages; i++)
                    Amount.Add(i + 1);

            }
        }

        private async void SelectCharacter()
        {
            if (Character != null)
            {
                await Shell.Current.GoToAsync($"{nameof(CharacterPage)}");
                (Shell.Current.CurrentPage.BindingContext as CharacterViewModel).Character = Character;

                Character = null;
            }
        }


        private async void OnSearch()
        {
            await Load();

            pages = (int)Math.Ceiling((double)total / 4);
            Amount.Clear();

            for (int i = 0; i < pages; i++)
                Amount.Add(i + 1);
        }

        private async Task Load(int offset = 0)
        {
            if (IsLoading)
                return;

            IsLoading = true;
            await Task.Delay(100);

            var result = await GetServiceAsync(offset, 4, Search);

            if (result?.data == null)
                return;

            total = result.data.total;
            Characters.Clear();

            foreach (var character in result?.data?.results)
                Characters.Add(character);

            IsLoading = false;
        }
    }
}
