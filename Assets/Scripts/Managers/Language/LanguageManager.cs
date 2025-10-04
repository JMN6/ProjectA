//#define Steam

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Steamworks;
using DataParseAndPlayer;
using Path;
using UnityEngine.TextCore.Text;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class LanguageDataRow : DataRow
    {
        public string StringID;
        public string Text;

        public override void SetValue(string[] datas)
        {
            StringID = datas[0];
            Text = datas[1];
        }
    }

    public class LanguageManager : MonoSingleton<LanguageManager>
    {
        /// <summary>
        /// Must be same as steam api language code
        /// https://partner.steamgames.com/doc/store/localization/languages
        /// </summary>
        public enum LanguageType
        {
            koreana,
            english,
            schinese,
            japanese,
            MAX
        }

        [field: SerializeField] public LanguageType LangType { get; private set; }
        public UnityEvent<LanguageType> OnLangChanged { get; private set; }

        private List<Dictionary<string, string>> _langDictionary = new List<Dictionary<string, string>>();

        public LanguageSO LanguageSo { get; set; }

        private bool isInit = false;

        protected override void Init()
        {
            base.Init();

            for (int i = 0; i < (int)LanguageType.MAX; ++i)
            {
                _langDictionary.Add(new Dictionary<string, string>());
            }

            LanguageSo = Resources.Load<LanguageSO>(Path.Data.Language);
            SetDefaultLanguage();

            LangType = LanguageType.MAX;
            OnLangChanged = new UnityEvent<LanguageType>();
            SetLanguageType(GetLanguageSetting());

            isInit = true;
        }

        private void LoadLanguage(LanguageType type)
        {
            if (_langDictionary[(int)type].Count > 0)
                return;

            string root;
            switch (type)
            {
                case LanguageType.koreana: root = Language.KOR; break;
                case LanguageType.english: root = Language.ENG; break;
                case LanguageType.schinese: root = Language.CHN; break;
                case LanguageType.japanese: root = Language.JPN; break;
                default: root = string.Empty; break;
            }

            var data = Parser<LanguageDataRow>.Convert(root);
            foreach (var row in data)
            {
                _langDictionary[(int)type].Add(row.StringID, row.Text);
            }
        }

        private void SetDefaultLanguage()
        {
            if (UnityEngine.PlayerPrefs.HasKey(Path.PlayerPrefs.LanguageSetting) == true &&
                GetLanguageSetting() != LanguageType.MAX)
                return;

#if Steam
            string l = SteamApps.GetCurrentGameLanguage();
            if (l.CompareTo(LanguageType.koreana.ToString()) == 0)
                UnityEngine.PlayerPrefs.SetInt(Path.PlayerPrefs.LanguageSetting, (int)LanguageType.koreana);
            else if (l.CompareTo(LanguageType.english.ToString()) == 0)
                UnityEngine.PlayerPrefs.SetInt(Path.PlayerPrefs.LanguageSetting, (int)LanguageType.english);
            else if (l.CompareTo(LanguageType.schinese.ToString()) == 0)
                UnityEngine.PlayerPrefs.SetInt(Path.PlayerPrefs.LanguageSetting, (int)LanguageType.schinese);
            else if (l.CompareTo(LanguageType.japanese.ToString()) == 0)
                UnityEngine.PlayerPrefs.SetInt(Path.PlayerPrefs.LanguageSetting, (int)LanguageType.japanese);
#else
                UnityEngine.PlayerPrefs.SetInt(Path.PlayerPrefs.LanguageSetting, (int)LanguageType.koreana);
#endif
        }

        private LanguageType GetLanguageSetting()
        {
            return (LanguageType) UnityEngine.PlayerPrefs.GetInt(Path.PlayerPrefs.LanguageSetting);
        }

        public void SetLanguageType(LanguageType type)
        {
            if (type == LangType)
                return;

            UnityEngine.PlayerPrefs.SetInt(Path.PlayerPrefs.LanguageSetting, (int)type);
            LangType = type;
            LoadLanguage(type);
            OnLangChanged?.Invoke(LangType);
        }

        public LanguageType GetCurrentLanguage()
        {
            if (isInit == false)
                Init();

            return LangType;
        }

        public string GetString(string key, LanguageType type = LanguageType.MAX)
        {
            if (isInit == false)
                Init();

            if (type == LanguageType.MAX) type = LangType;
            return _langDictionary[(int)type][key].Replace("\\n", "\n");
        }

        public void SetTextFont(TextMeshProUGUI text)
        {
            if (text == null) return;
            if(isInit == false) Init();

            text.font = LanguageSo.LanguageSets[(int)LangType].FontAsset;
            text.UpdateFontAsset();
        }

        public void SetTextFont(Text text)
        {
            if (text == null) return;
            if (isInit == false) Init();

            text.font = LanguageSo.LanguageSets[(int)LangType].Font;
        }
    }
}