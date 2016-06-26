using System;

/// <summary>
/// The kind of policy. There are three- one where there is no budget change, no increaes in funding, and no decrease in funding. Basically for itnernal use only.
/// </summary>
public enum PolicyType { NOBUDGET, NOINCREASE, NODECREASE }