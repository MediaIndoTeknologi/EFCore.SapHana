﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using WonderKidEngine.EntityFrameworkCore.SapHana.Query.ExpressionVisitors.Internal;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Query.Expressions.Internal
{
    public class SapHanaComplexFunctionArgumentExpression : SqlExpression
    {
        public SapHanaComplexFunctionArgumentExpression(
            IEnumerable<SqlExpression> argumentParts,
            string delimiter,
            Type type,
            RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Delimiter = delimiter;
            ArgumentParts = argumentParts.ToList().AsReadOnly();
        }

        /// <summary>
        ///     The arguments parts.
        /// </summary>
        public virtual IReadOnlyList<SqlExpression> ArgumentParts { get; }

        public virtual string Delimiter { get; }

        /// <summary>
        ///     Dispatches to the specific visit method for this node type.
        /// </summary>
        protected override Expression Accept(ExpressionVisitor visitor) =>
            visitor is SapHanaQuerySqlGenerator SapHanaQuerySqlGenerator // TODO: Move to VisitExtensions
                ? SapHanaQuerySqlGenerator.VisitSapHanaComplexFunctionArgumentExpression(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var argumentParts = new SqlExpression[ArgumentParts.Count];

            for (var i = 0; i < argumentParts.Length; i++)
            {
                argumentParts[i] = (SqlExpression) visitor.Visit(ArgumentParts[i]);
            }

            return Update(argumentParts, Delimiter);
        }

        public virtual SapHanaComplexFunctionArgumentExpression Update(IReadOnlyList<SqlExpression> argumentParts, string delimiter)
            => !argumentParts.SequenceEqual(ArgumentParts)
                ? new SapHanaComplexFunctionArgumentExpression(argumentParts, delimiter, Type, TypeMapping)
                : this;

        protected override void Print(ExpressionPrinter expressionPrinter)
            => expressionPrinter.Append(ToString());

        public override bool Equals(object obj)
            => obj != null &&
               (ReferenceEquals(this, obj) ||
                obj is SapHanaComplexFunctionArgumentExpression complexExpression && Equals(complexExpression));

        private bool Equals(SapHanaComplexFunctionArgumentExpression other)
            => base.Equals(other) &&
               Delimiter.Equals(other.Delimiter) &&
               ArgumentParts.SequenceEqual(other.ArgumentParts);

        /// <summary>
        ///     Returns a hash code for this object.
        /// </summary>
        /// <returns>
        ///     A hash code for this object.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(base.GetHashCode());

            foreach (var argumentPart in ArgumentParts)
            {
                hashCode.Add(argumentPart);
            }

            hashCode.Add(Delimiter);
            return hashCode.ToHashCode();
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the Expression.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the Expression.</returns>
        public override string ToString()
            => string.Join(Delimiter, ArgumentParts);
    }
}
