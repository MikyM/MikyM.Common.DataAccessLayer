﻿using System.Linq.Expressions;

namespace MikyM.Common.DataAccessLayer.Specifications.Expressions;

/// <summary>
/// Encapsulates data needed to perform 'SQL LIKE' operation.
/// </summary>
/// <typeparam name="T">Type of the source from which search target should be selected.</typeparam>
public class SearchExpressionInfo<T>
{
    private readonly Lazy<Func<T, string>> _selectorFunc;

    /// <summary>
    /// Creates instance of <see cref="SearchExpressionInfo{T}" />.
    /// </summary>
    /// <param name="selector">The property to apply the SQL LIKE against.</param>
    /// <param name="searchTerm">The value to use for the SQL LIKE.</param>
    /// <param name="searchGroup">The index used to group sets of Selectors and SearchTerms together.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="selector"/> is null.</exception>
    /// <exception cref="ArgumentException">If <paramref name="searchTerm"/> is null or empty.</exception>
    public SearchExpressionInfo(Expression<Func<T, string>> selector, string searchTerm, int searchGroup = 1)
    {
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        if (string.IsNullOrEmpty(searchTerm)) throw new ArgumentException(nameof(searchTerm));

        this.Selector = selector;
        this.SearchTerm = searchTerm;
        this.SearchGroup = searchGroup;

        this._selectorFunc = new Lazy<Func<T, string>>(this.Selector.Compile);
    }

    /// <summary>
    /// The property to apply the SQL LIKE against.
    /// </summary>
    public Expression<Func<T, string>> Selector { get; }

    /// <summary>
    /// The value to use for the SQL LIKE.
    /// </summary>
    public string SearchTerm { get; }

    /// <summary>
    /// The index used to group sets of Selectors and SearchTerms together.
    /// </summary>
    public int SearchGroup { get; }

    /// <summary>
    /// Compiled <see cref="Selector" />.
    /// </summary>
    public Func<T, string> SelectorFunc => this._selectorFunc.Value;
}