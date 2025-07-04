﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;

namespace Telegram.Reactive.Generators
{
    internal static class TypeExtensions
    {
        public static string GetBaseTypeSyntaxName(this BaseTypeSyntax baseClassSyntax)
        {
            if (baseClassSyntax is PrimaryConstructorBaseTypeSyntax parimaryConstructor)
                return parimaryConstructor.Type.ToString();

            if (baseClassSyntax is SimpleBaseTypeSyntax simpleBaseType)
                return simpleBaseType.Type.ToString();

            throw new BaseClassTypeNotFoundException();
        }

        public static bool IsAssignableFrom(this ITypeSymbol symbol, string className)
        {
            if (symbol.BaseType == null)
                return false;
                
            if (symbol.BaseType.Name == className)
                return true;

            return IsAssignableFrom(symbol.BaseType, className);
        }

        public static ITypeSymbol? Cast(this ITypeSymbol symbol, string className)
        {
            if (symbol.BaseType == null)
                return null;

            if (symbol.BaseType.Name == className)
                return symbol.BaseType;

            return Cast(symbol.BaseType, className);
        }

        public static IEnumerable<TResult> WhereCast<TResult>(this IEnumerable source)
        {
            foreach (object value in source)
            {
                if (value is TResult result)
                    yield return result;
            }
        }

        public static CompilationUnitSyntax FindCompilationUnitSyntax(this SyntaxNode syntax)
        {
            while (syntax is not CompilationUnitSyntax)
                syntax = syntax.Parent ?? throw new Exception();

            return (CompilationUnitSyntax)syntax;
        }

        public static IList<TValue> UnionAdd<TValue>(this IList<TValue> source, IEnumerable<TValue> toUnion, IEqualityComparer<TValue> equalityComparer)
        {
            foreach (TValue toUnionValue in toUnion)
            {
                if (!source.Contains(toUnionValue, equalityComparer))
                    source.Add(toUnionValue);
            }

            return source;
        }

        public static IList<TValue> UnionAdd<TValue>(this IList<TValue> source, IEnumerable<TValue> toUnion)
            => UnionAdd(source, toUnion, EqualityComparer<TValue>.Default);
    }
}
