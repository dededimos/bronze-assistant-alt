using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpecificationFilterLibrary.VisitorModels
{
    /// <summary>
    /// An Expression Visitor that swaps the Parameter of an Expression with another Parameter . 
    /// Very Useful to swap parameter on combined expressions so that they use the same object reference as parameter
    /// </summary>
    public class SwapParameterVisitor : ExpressionVisitor
    {
        private readonly Expression oldParam;
        private readonly Expression newParam;

        /// <summary>
        /// A Visitor that Swaps the parameter of an expression with another parameter
        /// Used to Combine Expressions so that their parameter is the same object Reference and wont need Invoking
        /// </summary>
        /// <param name="oldParam">The old Parameter that will be swapped with the new upon Visiting</param>
        /// <param name="newParam">The new Parameter that will swap the Old upon Visiting</param>
        public SwapParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
        {
            this.oldParam = oldParam;
            this.newParam = newParam;
        }

        /// <summary>
        /// <para>This will Visit the Specified Node.</para>
        /// <para>Upon Visiting the Node it will check if it matches the OldParam defined in the Constructor , 
        /// if it does not then it will visit the nodes children until all children are visited , or until it finds the oldParam and changes it with the newParam </para>
        /// <para>An Example is Passing into the Constructor the ParameterExpression of two Expressions and changing the Parameter 
        /// this way the Expressions will use the same reference as parameter and can be combined without invoking them</para>
        /// </summary>
        /// <param name="node">The Expression that will be Visited (usually the body) and after its children get traversed by the base.Visit(node)</param>
        /// <returns></returns>
        [return: NotNullIfNotNull("node")]
        public override Expression? Visit(Expression? node)
        {
            //If the 'node' matches the old parameter it will change it and return the expression with the newParam
            //Otherwise it will traverse the node expression tree until it finds the oldParam to change it .Otherwise it will return the same node as the one specified.
            return node == oldParam ? newParam : base.Visit(node);
        }

    }
}
