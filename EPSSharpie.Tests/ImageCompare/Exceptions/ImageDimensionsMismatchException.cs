﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.Primitives;

namespace EPSSharpie.Tests.ImageCompare.Exceptions
{
    public class ImageDimensionsMismatchException : ImagesSimilarityException
    {
        public ImageDimensionsMismatchException(Size expectedSize, Size actualSize) : base($"The image dimensions {actualSize} do not match the expected {expectedSize}!")
        {
            this.ExpectedSize = expectedSize;
            this.ActualSize = actualSize;
        }

        public Size ExpectedSize { get; }
        public Size ActualSize { get; }
    }
}