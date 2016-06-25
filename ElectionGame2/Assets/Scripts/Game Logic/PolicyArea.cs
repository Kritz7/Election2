using System;

/// <summary>
/// These are the 5 cateogories voters fall into. Voters belong to ONLY 1 category and vote based on how well their
/// category has been funded, and if their policy has been met.
/// 
/// All of these are also the policy areas, excluding ECONOMIC. This refers to the budget.
/// </summary>
public enum PolicyArea
{
    DEFENSE, INDUSTRY, PUBLIC, ENVIRONMENT
}