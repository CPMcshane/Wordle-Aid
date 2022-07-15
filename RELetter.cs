

namespace Wordle_Aid.RECaracter
{
    public class RECharacter
    {
        public RECharacter()
        {
            PossibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            CharacterFound = false;
        }
        internal void RemoveCharacter(string character)
        {
            if (CharacterFound)
                return;

            PossibleCharacters = PossibleCharacters.Replace(character, "");
        }
        internal void AddCorrectAnswer(string character)
        {
            PossibleCharacters = "";
            PossibleCharacters += character;
            CharacterFound = true;
        }
        public string FinalResult
        {
            get { return $"[{PossibleCharacters}]"; }
        }

        private string PossibleCharacters;

        private bool CharacterFound;
    }
}
