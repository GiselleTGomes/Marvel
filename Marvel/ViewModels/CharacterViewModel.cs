using MvvmHelpers;

namespace Marvel
{
    public class CharacterViewModel : ObservableObject
    {
        Character character;

        public Character Character
        {
            get => character;
            set => SetProperty(ref character, value);
        }
    }
}
