﻿using Scryber.Expressive.Expressions;
using System;

namespace Scryber.Expressive.Functions.Mathematical
{
    public class FloorFunction : FunctionBase
    {
        #region FunctionBase Members

        public override string Name { get { return "Floor"; } }

        public override object Evaluate(IExpression[] parameters, Context context)
        {
            this.ValidateParameterCount(parameters, 1, 1);

            return Math.Floor(Convert.ToDouble(parameters[0].Evaluate(Variables)));
        }

        #endregion
    }
}
