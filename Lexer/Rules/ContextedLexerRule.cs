﻿using System;
using System.Collections.Generic;
using Funcky.Monads;

namespace apophis.Lexer.Rules
{
    public class ContextedLexerRule : ILexerRule
    {
        public ContextedLexerRule(Predicate<char> symbolPredicate, Predicate<List<Lexem>> contextPredicate, Func<ILexerReader, Lexem> createToken, int weight)
        {
            SymbolPredicate = symbolPredicate;
            ContextPredicate = contextPredicate;
            CreateToken = createToken;
            Weight = weight;
        }

        public Predicate<char> SymbolPredicate { get; }

        public Predicate<List<Lexem>> ContextPredicate { get; }

        public Func<ILexerReader, Lexem> CreateToken { get; }

        public int Weight { get; }

        public Option<Lexem> Match(ILexerReader reader)
        {
            var predicate =
                from nextCharacter in reader.Peek()
                select SymbolPredicate(nextCharacter);

            return predicate.Match(false, p => p)
                ? Option.Some(CreateToken(reader))
                : Option<Lexem>.None();
        }

        public bool IsActive(List<Lexem> context)
        {
            return ContextPredicate(context);
        }
    }
}
