﻿using System;
using System.Collections.Generic;
using Lex.CodeAnalysis.Binding;
using Lex.CodeAnalysis.Symbols;

namespace Lex.CodeAnalysis
{

    internal sealed class Evaluator
    {
        private readonly BoundBlockStatemnet _root;
        private readonly Dictionary<VariableSymble, object> _variables;
        private Random _random;
        private object _lastValue;
        
     
        public Evaluator(BoundBlockStatemnet root, Dictionary<VariableSymble, object> variables) 
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            var labeleToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i<_root.Statements.Length;i++)
            {
                if(_root.Statements[i] is BoundLabelStatement l)
                    labeleToIndex.Add(l.Label,i+1);
            }
            var index = 0;
            while(index <_root.Statements.Length)
            {
                var s = _root.Statements[index];
                switch (s.Kind)
                    {
                       
                        case BoundNodeKind.VariableDeclaration:
                            EvaluateVariableDeclaration((BoundVeriableDeclaration)s);
                            index++;
                            break;
                        case BoundNodeKind.ExpressionStatement:
                            EvaluateExpressiontatement((BoundExpressionStatemnet)s);
                            index++;
                            break;
                        case BoundNodeKind.GotoStatment:
                            var gs = (BoundGotoStatment)s;
                            index = labeleToIndex[gs.Label];
                            break;
                        case BoundNodeKind.ConditionalGotoStatment:
                            var cgs = (BoundConditionalGotoStatment)s;
                            var condition = (bool)EvaluateExpression(cgs.Condition);
                            if(condition == cgs.JumpIfTrue)
                              {
                                index = labeleToIndex[cgs.Label];
                              }
                                
                            else
                            {
                                 index++;
                            }
                               
                            break;
                        case BoundNodeKind.LabelStatement:
                            index++;
                            break;
                        default:
                            throw new Exception($"Unexpected node {s.Kind}");
                    }
            }

            
            return _lastValue;
        }

        
        private void EvaluateVariableDeclaration(BoundVeriableDeclaration node)
        {
           var value = EvaluateExpression(node.Initializer);
           _variables[node.Variable] = value;
           _lastValue = value;
        }

      

        private void EvaluateExpressiontatement(BoundExpressionStatemnet node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateVeriableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssigmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeKind.CallExpression:
                    return EvaluateCallExpression((BoundCallExpression)node);
                case BoundNodeKind.ConversionExpression:
                    return EvaluateConversionExpression((BoundConversionExpression)node);
                default:
                    throw new Exception($"Unexpected node {node.Kind}");
            }
        }

        private object EvaluateConversionExpression(BoundConversionExpression node)
        {
            var value = EvaluateExpression(node.Expression);
            if(node.Type  == TypeSymbol.Bool)
                return  Convert.ToBoolean(value);
            else if(node.Type  == TypeSymbol.Int)
                return  Convert.ToInt32(value);
            else if(node.Type  == TypeSymbol.String)
                return  Convert.ToString(value);
            else
                throw new Exception($"Unexpected Type {node.Type}");
        }

        private object EvaluateCallExpression(BoundCallExpression node)
        {
           if(node.Function == BuiltinFunctions.Input)
           {
               return Console.ReadLine();
           }
           else if(node.Function == BuiltinFunctions.Print)
           {
               var message = (string)EvaluateExpression(node.Argument[0]);
               Console.WriteLine(message);
               return null;
           }
            else if(node.Function == BuiltinFunctions.Rnd)
           {
              var max = (int)EvaluateExpression(node.Argument[0]);
                if (_random == null)
                    _random = new Random();

                return _random.Next(max);
           }
           else
           {
               throw new Exception($"Unexpected function {node.Function}");
           }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression n)
        {
            return n.Value;
        }

         private object EvaluateVeriableExpression(BoundVariableExpression v)
        {
            return _variables[v.Variable];
        }

         private object EvaluateAssigmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _variables[a.Variable] = value;
            return value;
        }

         private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);


            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                case BoundUnaryOperatorKind.OnceComplement:
                    return ~(int)operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.Op.Kind}");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    if(b.Type == TypeSymbol.Int)
                        return (int)left + (int)right;
                    else
                        return (string)left + (string)right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Power:
                    return (int)(Math.Pow((int)left ,(int)right));
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.Remainder:
                    return (int)left % (int)right;

                case BoundBinaryOperatorKind.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.LessThan:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.GreaterOrEqual:
                    return (int)left >= (int)right;
                case BoundBinaryOperatorKind.LessOrEqual:
                    return (int)left <= (int)right;

                case BoundBinaryOperatorKind.BitwiseAnd:
                    if(b.Type == TypeSymbol.Int)
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperatorKind.BitwiseOr:
                   if(b.Type == TypeSymbol.Int)
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperatorKind.BitwiseXor:
                    if(b.Type == TypeSymbol.Int)
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;

                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;

                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);
                default:
                    throw new Exception($"Unexpected binary operator {b.Op.Kind}");
            }
        }
    }
}