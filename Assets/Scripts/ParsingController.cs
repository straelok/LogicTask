using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ParsingController : MonoBehaviour
{
    public static ParsingController Instance;
    public GameObject inputField;
    public GameObject parsingResult;

    const string SYMBOLS = @"[^a-zA-Z]";

    Regex regexWordMask = new Regex(@"[&][a-zA-Z]*[\s)]");
    Regex regexWordMaskOptional = new Regex(@"[&][a-zA-Z]*[[][a-zA-Z/]*[]]");

    List<ParsingRuleObject> ruleCache = new List<ParsingRuleObject>();

    //"I like bananas, but I prefers strawberries!";
    //"&banana[s] &but &prefe[r/rs]";

    void Start()
    {
        Instance = this;
        foreach (ParsingRuleObject rule in Resources.LoadAll("Objects/Scriptable objects/Parsing Rules", typeof(ParsingRuleObject)))
        {
            ruleCache.Add(rule);
            Debug.Log("Loaded " + rule.parsingCondition + " order");
        }
        //string[] assetNames = AssetDatabase.FindAssets("Your_Filter", new[] { "Assets/YourFolder" });
    }

    public void ValidationButton()
    {
        string textForValidation = inputField.GetComponent<TextMeshProUGUI>().text;
        int maxOrder = -1;
        foreach (ParsingRuleObject rule in ruleCache)
        {
            Debug.Log("Start parsing " + rule.parsingCondition + " rule");
            string boolStringFormat = validationFormatting(rule.parsingCondition, textForValidation);
            if (ConvertBoolString(boolStringFormat))
            {
                if(maxOrder < rule.order)
                {
                    maxOrder = rule.order;
                    parsingResult.GetComponent<TextMeshProUGUI>().text = "Text is valid. Rules " + rule.parsingCondition;
                }
                Debug.Log("True, order = " + rule.order);
            }
            else
            {
                if(maxOrder == -1)
                    parsingResult.GetComponent<TextMeshProUGUI>().text = "Text is invalid";
                Debug.Log("False");
            }
        }
    }

    //validation formatting return string in bool logic like a "&banana[s] &but &prefer" -> 1 1 0 (if prefer not including in string)
    public string validationFormatting(string validation, string stringForValidation)
    {
        validation += " ";
        MatchCollection matchCollection = regexWordMask.Matches(validation);
        MatchCollection matchCollectionOptional = regexWordMaskOptional.Matches(validation);

        foreach (Match match in matchCollection)
        {
            if (CheckValidWord(match.ToString(), stringForValidation))
            {
                string buffer = match.ToString().Replace(")", "");
                validation = validation.Replace(buffer, "1 ");
            }
            else
            {
                string buffer = match.ToString().Replace(")", "");
                validation = validation.Replace(buffer, "0 ");
            }
        }
        foreach (Match match in matchCollectionOptional)
        {
            if (CheckValidWord(match.ToString(), stringForValidation))
                validation = validation.Replace(match.ToString(), "1");
            else
                validation = validation.Replace(match.ToString(), "0");
        }
        return validation;
    }

    // check mask and convert in to words
    public bool CheckValidWord(string word, string stringForValidation)
    {
        if (regexWordMaskOptional.Matches(word).Count == 0)
        {
            //default mask

            //delete & symbol in start and [\s,.-] in end (only for default mask)
            word = word.Substring(1, word.Length - 2);
            return(SearchWordInString(word, stringForValidation));
        }
        else
        {
            //optional mask

            //delete "&" symbol in start
            word = word.Substring(1);

            List<string> wordsList = new List<string>();

            while (!regexWordMaskOptional.Match(word).Equals(""))
            {
                //add formated word in to list
                wordsList.Add(FormatOptionalWord(word));

                //delete using optional for using next one. strawberr[y/ies] -> strawberr[ies]
                string replaceString = new Regex("[a-zA-Z]*/").Match(word).ToString();
                if (!replaceString.Equals(""))
                {
                    word = word.Replace(replaceString, "");
                }
                else
                {
                    //if optionals is out
                    break;
                }
            }

            //check all optional word in main string
            return(SearchWordInString(wordsList, stringForValidation));
        }
    }

    //format word, using optional. banana[s] -> bananas or strawberr[y/ies] -> strawberry
    public string FormatOptionalWord(string str)
    {
        Regex optionalWordMask = new Regex("[a-zA-Z]*[[][a-zA-Z]*");
        str = optionalWordMask.Match(str).ToString();
        str = str.Replace("[", "");
        return str;
    }

    //Searching words in to text string
    public bool SearchWordInString(string word, string str)
    {
        Regex regexWordInString = new Regex(word + SYMBOLS);
        if (!regexWordInString.Match(str).ToString().Equals(""))
            return true;
        else
            return false;
    }

    public bool SearchWordInString(List<string> word, string str)
    {
        foreach (string buff in word)
        {
            Regex regexWordInString = new Regex(buff + SYMBOLS);
            if (!regexWordInString.Match(str).ToString().Equals(""))
                return true;
        }
        return false;
    }

    //Convert boolstring into bool. Example (0 0) | (0 1) -> 0 or (0 0) | 1 -> 1 
    public bool ConvertBoolString(string str)
    {
        str = str.Replace(" ", "");
        int i = 0;
        while (!(str.Equals("1") || str.Equals("0")))
        {
            i++;
            if (i > 50)
            {
                Debug.LogError("While trouble! Maybe invalid syntaxis in Parsing Rules!");
                break;
            }
            str = str.Replace("!1", "0");
            str = str.Replace("!0", "1");
            str = str.Replace("1|1", "1");
            str = str.Replace("1|0", "1");
            str = str.Replace("0|1", "1");
            str = str.Replace("0|0", "0");
            str = str.Replace("(1)", "1");
            str = str.Replace("(0)", "0");
            str = str.Replace("11", "1");
            str = str.Replace("10", "0");
            str = str.Replace("00", "0");
            str = str.Replace("01", "0");

            
        }
        if (str.Equals("1"))
            return true;
        else
            return false; 
    }
}
