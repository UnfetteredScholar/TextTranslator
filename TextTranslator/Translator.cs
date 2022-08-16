﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json;


namespace TextTranslator
{
    public enum Language { Arabic, Czech, German, English, Spanish, Estonian, Finnish, French, Gujarati, Hindi, Italian, Japanese, Kazakh, Korean, Lithuanian, Latvian, Burmese, Nepali, Dutch, Romanian, Russian, Sinhala, Turkish, Vietnamese, Chinese, Afrikaans, Azerbaijani, Bengali, Persian, Hebrew, Croatian, Indonesian, Georgian, Khmer, Macedonian, Malayalam, Mongolian, Marathi, Polish, Pashto, Portuguese, Swedish, Swahili, Tamil, Telugu, Thai, Tagalog, Ukrainian, Urdu, Xhosa, Galician, Slovene };

    [DisplayName("Text Translator")]
    [Description("Translates input text from source language to target language")]
    public class Translator: AsyncCodeActivity
    {
        private static readonly HttpClient client = new HttpClient();

        [Category("Input")]
        [DisplayName("Source Language")]
        [Description("Enter the source language")]
        [RequiredArgument]
        public InArgument<Language> SourceLanguage { get; set; }

        [Category("Input")]
        [DisplayName("Conversion Language")]
        [Description("Enter the conversion language")]
        [RequiredArgument]
        public InArgument<Language> ConversionLanguage { get; set; }

        [Category("Input")]
        [DisplayName("Sentence")]
        [Description("Enter the text to be translated")]
        [RequiredArgument]
        public InArgument<string> Sentence { get; set; }

        [Category("Output")]
        public OutArgument<string> TranslatedText { get; set; }

        public Translator()
        {
            client.BaseAddress = new System.Uri("https://text-translation-fairseq-1.ai-sandbox.4th-ir.io/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Translates text from source language to specified language
        /// </summary>
        /// <param name="sentence">Text to be translated</param>
        /// <param name="sourceLanguage">Language of text to be translated</param>
        /// <param name="conversionLanguage">Target language for translation</param>
        /// <exception cref="Exception">Thrown when translation fails</exception>
        /// <returns>Returns the translated text </returns>
        public async Task<string> TranslateText(string sentence, Language sourceLanguage, Language conversionLanguage)
        {
            RequestContent request = new RequestContent(sentence);

            string requestUri = $"api/v1/sentence?source_lang={sourceLanguage}&conversion_lang={conversionLanguage}";

            var requestJson = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, requestJson);

            try
            {
                response.EnsureSuccessStatusCode();

                string r = await response.Content.ReadAsStringAsync();
                char[] param = { '[', ']' };
                ResponseContent responseContent = JsonConvert.DeserializeObject<ResponseContent>(r.Trim(param));

                return responseContent.conversion_text;
            }
            catch(Exception exception)
            {
                Exception ex = new Exception("Translation Failed\n"+response,exception);
                throw ex;
            }
        

        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Language sourceLang = SourceLanguage.Get(context);
            Language conversionLang = ConversionLanguage.Get(context);
            string sentence = Sentence.Get(context);

            return this.TranslateText(sentence, sourceLang, conversionLang);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            string translatedText=(string)context.UserState;
            TranslatedText.Set(context, translatedText);
        }

    }
}
