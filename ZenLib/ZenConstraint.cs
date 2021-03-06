﻿// <copyright file="ZenConstraint.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ZenLib
{
    using System;
    using System.Collections.Generic;
    using ZenLib.ModelChecking;

    /// <summary>
    /// Zen constraint representation.
    /// </summary>
    public class ZenConstraint<T> : ZenFunction<T, bool>
    {
        /// <summary>
        /// Creates a new Zen constraint.
        /// </summary>
        /// <param name="function">The function for the constraint.</param>
        public ZenConstraint(Func<Zen<T>, Zen<bool>> function) : base(function)
        {
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input">Default input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public Option<T> Find(
            Zen<T> input = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.Find((i, o) => o, input, listSize, checkSmallerLists, backend);
        }

        /// <summary>
        /// Find all inputs that solve the constraint.
        /// </summary>
        /// <param name="input">Default input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public IEnumerable<T> FindAll(
            Zen<T> input = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.FindAll((i, o) => o, input, listSize, checkSmallerLists, backend);
        }
    }

    /// <summary>
    /// Zen constraint representation.
    /// </summary>
    public class ZenConstraint<T1, T2> : ZenFunction<T1, T2, bool>
    {
        /// <summary>
        /// Creates a new Zen constraint.
        /// </summary>
        /// <param name="function">The function for the constraint.</param>
        public ZenConstraint(Func<Zen<T1>, Zen<T2>, Zen<bool>> function) : base(function)
        {
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input1">First input that captures structural constraints.</param>
        /// <param name="input2">Second input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public Option<(T1, T2)> Find(
            Zen<T1> input1 = null,
            Zen<T2> input2 = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.Find((i1, i2, o) => o, input1, input2, listSize, checkSmallerLists, backend);
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input1">First input that captures structural constraints.</param>
        /// <param name="input2">Second input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public IEnumerable<(T1, T2)> FindAll(
            Zen<T1> input1 = null,
            Zen<T2> input2 = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.FindAll((i1, i2, o) => o, input1, input2, listSize, checkSmallerLists, backend);
        }
    }

    /// <summary>
    /// Zen constraint representation.
    /// </summary>
    public class ZenConstraint<T1, T2, T3> : ZenFunction<T1, T2, T3, bool>
    {
        /// <summary>
        /// Creates a new Zen constraint.
        /// </summary>
        /// <param name="function">The function for the constraint.</param>
        public ZenConstraint(Func<Zen<T1>, Zen<T2>, Zen<T3>, Zen<bool>> function) : base(function)
        {
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input1">First input that captures structural constraints.</param>
        /// <param name="input2">Second input that captures structural constraints.</param>
        /// <param name="input3">Third input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public Option<(T1, T2, T3)> Find(
            Zen<T1> input1 = null,
            Zen<T2> input2 = null,
            Zen<T3> input3 = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.Find((i1, i2, i3, o) => o, input1, input2, input3, listSize, checkSmallerLists, backend);
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input1">First input that captures structural constraints.</param>
        /// <param name="input2">Second input that captures structural constraints.</param>
        /// <param name="input3">Third input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public IEnumerable<(T1, T2, T3)> FindAll(
            Zen<T1> input1 = null,
            Zen<T2> input2 = null,
            Zen<T3> input3 = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.FindAll((i1, i2, i3, o) => o, input1, input2, input3, listSize, checkSmallerLists, backend);
        }
    }

    /// <summary>
    /// Zen constraint representation.
    /// </summary>
    public class ZenConstraint<T1, T2, T3, T4> : ZenFunction<T1, T2, T3, T4, bool>
    {
        /// <summary>
        /// Creates a new Zen constraint.
        /// </summary>
        /// <param name="function">The function for the constraint.</param>
        public ZenConstraint(Func<Zen<T1>, Zen<T2>, Zen<T3>, Zen<T4>, Zen<bool>> function) : base(function)
        {
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input1">First input that captures structural constraints.</param>
        /// <param name="input2">Second input that captures structural constraints.</param>
        /// <param name="input3">Third input that captures structural constraints.</param>
        /// <param name="input4">Fourth input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public Option<(T1, T2, T3, T4)> Find(
            Zen<T1> input1 = null,
            Zen<T2> input2 = null,
            Zen<T3> input3 = null,
            Zen<T4> input4 = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.Find((i1, i2, i3, i4, o) => o, input1, input2, input3, input4, listSize, checkSmallerLists, backend);
        }

        /// <summary>
        /// Find an input that solves the constraint.
        /// </summary>
        /// <param name="input1">First input that captures structural constraints.</param>
        /// <param name="input2">Second input that captures structural constraints.</param>
        /// <param name="input3">Third input that captures structural constraints.</param>
        /// <param name="input4">Fourth input that captures structural constraints.</param>
        /// <param name="listSize">The maximum number of elements to consider in an input list.</param>
        /// <param name="checkSmallerLists">Whether to check smaller list sizes as well.</param>
        /// <param name="backend">The backend.</param>
        /// <returns>An input if one exists satisfying the constraints.</returns>
        public IEnumerable<(T1, T2, T3, T4)> FindAll(
            Zen<T1> input1 = null,
            Zen<T2> input2 = null,
            Zen<T3> input3 = null,
            Zen<T4> input4 = null,
            int listSize = 5,
            bool checkSmallerLists = true,
            Backend backend = Backend.Z3)
        {
            return this.FindAll((i1, i2, i3, i4, o) => o, input1, input2, input3, input4, listSize, checkSmallerLists, backend);
        }
    }
}