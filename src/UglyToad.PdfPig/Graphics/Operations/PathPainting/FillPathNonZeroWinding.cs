﻿namespace UglyToad.PdfPig.Graphics.Operations.PathPainting
{
    using System.IO;

    /// <inheritdoc />
    /// <summary>
    /// Fill the path, using the nonzero winding number rule to determine the region to fill. 
    /// Any subpaths that are open are implicitly closed before being filled.
    /// </summary>
    public class FillPathNonZeroWinding : IGraphicsStateOperation
    {
        /// <summary>
        /// The symbol for this operation in a stream.
        /// </summary>
        public const string Symbol = "f";

        /// <summary>
        /// The instance of the <see cref="FillPathNonZeroWinding"/> operation.
        /// </summary>
        public static readonly FillPathNonZeroWinding Value = new FillPathNonZeroWinding();

        /// <inheritdoc />
        public string Operator => Symbol;

        private FillPathNonZeroWinding()
        {
        }

        /// <inheritdoc />
        public void Run(IOperationContext operationContext)
        {
            operationContext.FillPath(PdfPig.Core.FillingRule.NonZeroWinding, false);
        }

        /// <inheritdoc />
        public void Write(Stream stream)
        {
            stream.WriteText(Symbol);
            stream.WriteNewLine();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Symbol;
        }
    }
}