using System;
using System.Collections;
using UISystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;


public class LocalizationManager : SingletonMonoBehaviour<LocalizationManager>
{
    private StringTable[] localizationTables = null;
    const string tableName = "Localization Table";

    private int localIndex = 0;
    private bool isCompleted = false;

    protected override void OnAwakeSingleton()
    {
        base.OnAwakeSingleton();
        DontDestroyOnLoad(this);
    }
    // 로컬라이징 데이터 다운
    public IEnumerator LoadData()
    {
        if (isCompleted == false)
        {
            localizationTables = new StringTable[(int)LanguageType.Max];
            yield return LocalizationSettings.InitializationOperation;

            for (int i = 0; i < localizationTables.Length; i++)
            {
                var tableOp = LocalizationSettings.StringDatabase.GetTableAsync(tableName, LocalizationSettings.AvailableLocales.Locales[i]);
                yield return tableOp;


                if (tableOp.Status == AsyncOperationStatus.Succeeded)
                {
                    localizationTables[i] = tableOp.Result;
                }
            }

            isCompleted = true;
        }
    }

    public string GetLocalizeText(string key)
    {
        var tableEntry = LocalizationSettings.StringDatabase.GetTableEntry(tableName, key, LocalizationSettings.AvailableLocales.Locales[localIndex]);
        StringTableEntry entry = tableEntry.Entry;

        if (entry != null)
            return entry.GetLocalizedString();

        return key;
    }
    // 언어 변경
    public void ChangeLanguage(int index, Action call = null)
    {
        StartCoroutine(_ChangeLanguage(index, call));
    }
    IEnumerator _ChangeLanguage(int index, Action call = null)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        localIndex = index;
        call?.Invoke();
    }

}