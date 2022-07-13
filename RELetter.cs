using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle_Aid.RECaracter
{
    public class RECharacter
    {
        public RECharacter()
        {
            PossibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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
        internal string FinalResult()
        {
            return $"[{PossibleCharacters}]";
        }

        private string PossibleCharacters { get; set; }

        private bool CharacterFound { get; set; } = false;
    }
}
