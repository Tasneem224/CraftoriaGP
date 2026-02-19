using Google.Cloud.Translation.V2;
using GTranslate.Translators;
using Microsoft.Extensions.Configuration;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TranslationService : ITranslationService
    {
        private readonly GoogleTranslator _translator;

        public TranslationService()
        {
            _translator = new GoogleTranslator();
        }
        public async Task<string> TranslateAsync(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            try
            {
                
                var result = await _translator.TranslateAsync(text, targetLanguage);

                return result.Translation;
            }
            catch (Exception)
            {
                return text;
            }
        }
    }
}
