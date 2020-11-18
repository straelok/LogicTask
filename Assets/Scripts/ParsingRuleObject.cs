using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParsingRule", menuName = "Scriptable Objects/ParsingRule", order = 1)]
public class ParsingRuleObject : ScriptableObject
{
    [Tooltip("& using like a 'and'. Can be multiple. Example: &banana &another &eat. [] using for optional ending of the word. Example: strawberr[y/ies] = &strawberry &strawberries ")]
    public string parsingCondition;
    public int order;
}