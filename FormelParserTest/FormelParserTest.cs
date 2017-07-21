﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormelParser;
using System.Linq;

namespace FormelParserTest
{
    [TestClass]
    public class FormelParserTest
    {
        [TestMethod]
        public void SimpleParse()
        {
            Parser parser = new Parser("5+17*22");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.AreEqual(calculator.Result, 379.0);
        }

        [TestMethod]
        public void SingleNumber()
        {
            Parser parser = new Parser("22");
            var parseTree = parser.Parse();

            var number = parseTree as NumberNode;
            Assert.IsNotNull(number);
            Assert.AreEqual(number.Number, 22.0);

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(calculator.Result, 22.0);

            var parenthizer = new FormelParser.Visitors.FullParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("22", parenthizer.Result);
        }

        [TestMethod]
        public void SingleFloat()
        {
            Parser parser = new Parser("3.14159265");
            var parseTree = parser.Parse();

            var number = parseTree as NumberNode;
            Assert.IsNotNull(number);
            Assert.AreEqual(number.Number, 3.14159265);

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(calculator.Result, 3.14159265);

            var parenthizer = new FormelParser.Visitors.FullParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("3.14159265", parenthizer.Result);
        }

        [TestMethod]
        public void SimpleAddition()
        {
            Parser parser = new Parser("1+2+3+4+5+6+7+8+9+10+11+12+13+14+15+16+17+18+19+20");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(210.0, calculator.Result);
        }

        [TestMethod]
        public void SimpleSubstraction()
        {
            Parser parser = new Parser("100-1-2-3-4-5-6-7-8-9)");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(55.0, calculator.Result);

        }

        [TestMethod]
        public void SimpleMultiplication()
        {
            Parser parser = new Parser("1*2*3*4*5*6*7");

            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(5040.0, calculator.Result);
        }

        [TestMethod]
        public void SimpleDivision()
        {
            Parser parser = new Parser("(1/2)/(1/10)/2");

            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(2.5, calculator.Result);

        }

        [TestMethod]
        public void ComplexExample()
        {
            Parser parser = new Parser("145.2/3+7-(8*45+22*(2-19))-88/8 + 17");

            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);

            Assert.AreEqual(75.4, calculator.Result);

            var parenthizer = new FormelParser.Visitors.FullParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("(((((145.2/3)+7)-((8*45)+(22*(2-19))))-(88/8))+17)", parenthizer.Result);
        }

        [TestMethod]
        public void MinimizeParanthesisAssociativitySumsTest()
        {
            Parser plusParser = new Parser("(1+(2+(3+4)))");
            var plusParseTree = plusParser.Parse();

            var plusParenthizer = new FormelParser.Visitors.MinimalParenthesisVisitor();
            plusParseTree.Accept(plusParenthizer);

            Assert.AreEqual("1+2+3+4", plusParenthizer.Result);

            Parser minusParser = new Parser("(-1-(2-(3-4)))");
            var minusParseTree = minusParser.Parse();

            var minusParenthizer = new FormelParser.Visitors.MinimalParenthesisVisitor();
            minusParseTree.Accept(minusParenthizer);

            Assert.AreEqual("-1-(2-(3-4))", minusParenthizer.Result);
        }

        [TestMethod]
        public void MinimizeParanthesisAssociativityMultiplyTest()
        {
            Parser parser = new Parser("6*(1+2+3)");
            var parseTree = parser.Parse();

            var parenthizer = new FormelParser.Visitors.MinimalParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("6*(1+2+3)", parenthizer.Result);
        }

        [TestMethod]
        public void MinimizeParanthesisAssociativityDivisionTest()
        {
            Parser parser = new Parser("((24/1)/(2/3))");
            var parseTree = parser.Parse();

            var parenthizer = new FormelParser.Visitors.MinimalParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("(24/1)/(2/3)", parenthizer.Result);
        }

        [TestMethod]
        public void MinimizeParanthesisAssociativitySimplifyTest()
        {
            Parser parser = new Parser("(((7*8)+(5*6))+(10/5))");
            var parseTree = parser.Parse();

            var parenthizer = new FormelParser.Visitors.MinimalParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("7*8+5*6+10/5", parenthizer.Result);
        }

        [TestMethod]
        public void MinimizeParanthesisFunctionsAndVariablesTest()
        {
            Parser parser = new Parser("(((7*a)+(5*cos(Pi)))+(10/sqrt(c+(d*5))))");
            var parseTree = parser.Parse();

            var parenthizer = new FormelParser.Visitors.MinimalParenthesisVisitor();
            parseTree.Accept(parenthizer);

            Assert.AreEqual("7*a+5*cos(Pi)+10/sqrt(c+d*5)", parenthizer.Result);
        }

        [TestMethod]
        public void ConstantTest()
        {
            Parser parser = new Parser("2 * Pi * Pi");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.AreEqual(2 * Math.PI * Math.PI, calculator.Result);
        }

        [TestMethod]
        public void FunctionTest()
        {
            Parser parser = new Parser("2 * cos(Pi)");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.AreEqual(2 * Math.Cos(Math.PI), calculator.Result);
        }

        [TestMethod]
        public void FunctionParameterTest()
        {
            Parser parser = new Parser("pow(sqrt(2), 2)");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.AreEqual(2, calculator.Result, 0.00001);
        }

        [TestMethod]
        public void FunctionParameter2Test()
        {
            Parser parser = new Parser("pow(pow(2,2), sqrt(2) * sqrt(2)) * e");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();
            parseTree.Accept(calculator);


            Assert.AreEqual(Math.E * 16, calculator.Result, 0.00001);
        }

        [TestMethod]
        public void AssignVariablesTest()
        {

            Parser parser = new Parser("100 * x");
            var parseTree = parser.Parse();

            var calculator = new FormelParser.Visitors.CalculateVisitor();

            foreach (int x in Enumerable.Range(0, 100))
            {
                calculator.Variables["x"] = x;
                parseTree.Accept(calculator);
                Assert.AreEqual(100 * x, calculator.Result, 0.00001);
            }

        }
    }
}