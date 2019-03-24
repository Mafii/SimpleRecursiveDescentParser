﻿using System;
using System.Linq;
using ArithmeticParser.Nodes;
using ArithmeticParser.Visitors;
using Autofac;
using Xunit;

namespace ArithmeticParser.Test
{
    public class ArithmeticParserTest
    {
        private readonly Parser _parser;

        public ArithmeticParserTest()
        {
            _parser = new CompositionRoot()
                .Build()
                .Resolve<Parser>();
        }


        [Fact]
        public void ParseSimpleArithmeticExpressionCorrectly()
        {
            var parseTree = _parser.Parse("5+17*22");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(379.0, calculator.Result);
        }

        [Fact]
        public void ParseSingleNumberCorrectly()
        {
            var parseTree = _parser.Parse("22");

            var number = parseTree as NumberNode;
            Assert.NotNull(number);
            Assert.Equal(22.0, number.Number);

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(22.0, calculator.Result);

            var parenthesisVisitor = new FullParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("22", parenthesisVisitor.Result);
        }

        [Fact]
        public void ParseSimpleFloatCorrectly()
        {
            var parseTree = _parser.Parse("3.14159265");

            var number = parseTree as NumberNode;
            Assert.NotNull(number);
            Assert.Equal(3.14159265, number.Number);

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(3.14159265, calculator.Result);

            var parenthesisVisitor = new FullParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("3.14159265", parenthesisVisitor.Result);
        }

        [Fact]
        public void ParseSimpleAdditionCorrectly()
        {
            var parseTree = _parser.Parse("1+2+3+4+5+6+7+8+9+10+11+12+13+14+15+16+17+18+19+20");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(210.0, calculator.Result);
        }

        [Fact]
        public void ParseSimpleSubtractionCorrectly()
        {
            var parseTree = _parser.Parse("100-1-2-3-4-5-6-7-8-9)");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(55.0, calculator.Result);

        }

        [Fact]
        public void ParseSimpleMultiplicationCorrectly()
        {
            var parseTree = _parser.Parse("1*2*3*4*5*6*7");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(5040.0, calculator.Result);
        }

        [Fact]
        public void ParseSimpleDivisionCorrectly()
        {
            var parseTree = _parser.Parse("(1/2)/(1/10)/2");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(2.5, calculator.Result);
        }

        [Fact]
        public void ParseComplexArithmeticExampleCorrectly()
        {
            var parseTree = _parser.Parse("145.2/3+7-(8*45+22*(2-19))-88/8 + 17");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.Equal(75.4, calculator.Result);

            var parenthesisVisitor = new FullParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("(((((145.2/3)+7)-((8*45)+(22*(2-19))))-(88/8))+17)", parenthesisVisitor.Result);
        }

        [Fact]
        public void CreatesArithmeticExpressionWithMinimalAmountOfParenthesis()
        {
            var plusParseTree = _parser.Parse("(1+(2+(3+4)))");

            var parenthesisVisitor = new MinimalParenthesisVisitor();
            plusParseTree.Accept(parenthesisVisitor);

            Assert.Equal("1+2+3+4", parenthesisVisitor.Result);

            var minusParseTree = _parser.Parse("(-1-(2-(3-4)))");

            var minimalParenthesisVisitor = new MinimalParenthesisVisitor();
            minusParseTree.Accept(minimalParenthesisVisitor);

            Assert.Equal("-1-(2-(3-4))", minimalParenthesisVisitor.Result);
        }

        [Fact]
        public void ShouldCreateMultiplicationWithMinimalAmountOfParenthesis()
        {
            var parseTree = _parser.Parse("6*(1+2+3)");

            var parenthesisVisitor = new MinimalParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("6*(1+2+3)", parenthesisVisitor.Result);
        }

        [Fact]
        public void ShouldCreatesDivisionWithMinimalAmountOfParenthesis()
        {
            var parseTree = _parser.Parse("((24/1)/(2/3))");

            var parenthesisVisitor = new MinimalParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("(24/1)/(2/3)", parenthesisVisitor.Result);
        }

        [Fact]
        public void ShouldSimplifyToExpressionWithoutParenthesis()
        {
            var parseTree = _parser.Parse("(((7*8)+(5*6))+(10/5))");

            var parenthesisVisitor = new MinimalParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("7*8+5*6+10/5", parenthesisVisitor.Result);
        }

        [Fact]
        public void ShouldCreateExpressionWithMinimalParenthesisGivenExpressionWithVariablesAndFunctions()
        {
            var parseTree = _parser.Parse("(((7*a)+(5*cos(Pi)))+(10/sqrt(c+(d*5))))");

            var parenthesisVisitor = new MinimalParenthesisVisitor();
            parseTree.Accept(parenthesisVisitor);

            Assert.Equal("7*a+5*cos(Pi)+10/sqrt(c+d*5)", parenthesisVisitor.Result);
        }

        [Fact]
        public void CalculateExpressionWithConstantsCorrectly()
        {
            var parseTree = _parser.Parse("2 * Pi * Pi");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.Equal(2 * Math.PI * Math.PI, calculator.Result);
        }

        [Fact]
        public void CalculatesAFunctionCallCorrectly()
        {
            var parseTree = _parser.Parse("2 * cos(Pi)");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.Equal(2 * Math.Cos(Math.PI), calculator.Result);
        }

        [Fact]
        public void CalculatesFunctionCallsWithMultipleParametersCorrectly()
        {
            var parseTree = _parser.Parse("pow(sqrt(2), 2)");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.Equal(2, calculator.Result, new MagnitudeDoubleComparer(0.00001));
        }

        [Fact]
        public void CalculatesComplexFunctionCallsWithMultipleParametersCorrectly()
        {
            var parseTree = _parser.Parse("pow(pow(2,2), sqrt(2) * sqrt(2)) * e");

            var calculator = new CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.Equal(Math.E * 16, calculator.Result, new MagnitudeDoubleComparer(0.00001));
        }

        [Fact]
        public void ShouldParseVariableXAndCalculateWithDifferentValuesOfXCorrectly()
        {

            var parseTree = _parser.Parse("100 * x");

            var calculator = new CalculateVisitor();

            foreach (int x in Enumerable.Range(0, 100))
            {
                calculator.Variables["x"] = x;
                parseTree.Accept(calculator);
                Assert.Equal(100 * x, calculator.Result, new MagnitudeDoubleComparer(0.00001));
            }
        }
    }
}