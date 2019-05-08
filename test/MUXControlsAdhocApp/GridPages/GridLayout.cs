﻿using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;

using Debug = System.Diagnostics.Debug;
using ConditionalAttribute = System.Diagnostics.ConditionalAttribute;

namespace MUXControlsAdhocApp.GridPages
{
    public class GridTrackInfo
    {
        // TODO: a line can have more than one name. For example, here the second line will have two names: row1-end and row2-start:
        // grid-template-rows: [row1-start] 25% [row1-end row2-start] 25% [row2-end];
        // TODO: Line more refers to the space between two tracks, not a cell
        public string LineName { get; set; }

        // TODO: track-size can be a length, a percentage, or a fraction of the free space in the grid (fr)
        // grid-template-columns: 40px 50px auto 50px 40px;
        // grid-template-rows: 25% 100px auto;
        // The fr unit allows you to set the size of a track as a fraction of the free space of the grid container. For example, this will set each item to one third the width of the grid container:
        // grid-template-columns: 1fr 1fr 1fr;
        public double Length { get; set; } = 0;
        public bool Auto { get; set; } = false;
        public double Fraction { get; set; } = 0.0;
        public double Percentage { get; set; } = 0.0;
    }

    public class GridLocation
    {
        public int Index { get; set; } = -1;

        public string LineName { get; set; }

        public int Span { get; set; } = 0;
    }

    public enum GridJustifyItems
    {
        Start,
        End,
        Center,
        Stretch,
    }

    public enum GridAlignItems
    {
        Start,
        End,
        Center,
        Stretch,
    }

    public enum GridJustifyContent
    {
        Start,
        End,
        Center,
        Stretch,
        SpaceAround,
        SpaceBetween,
        SpaceEvenly,
    }

    public enum GridAlignContent
    {
        Start,
        End,
        Center,
        Stretch,
        SpaceAround,
        SpaceBetween,
        SpaceEvenly,
    }

    public enum GridAutoFlow
    {
        Row,
        Column,
        RowDense,
        ColumnDense,
    }

    public enum GridJustifySelf
    {
        Start,
        End,
        Center,
        Stretch,
    }

    public enum GridAlignSelf
    {
        Start,
        End,
        Center,
        Stretch,
    }

    public class GridLayout : NonVirtualizingLayout
    {
        #region ChildProperties

        public static readonly DependencyProperty ColumnStartProperty =
            DependencyProperty.RegisterAttached(
              "ColumnStart",
              typeof(GridLocation),
              typeof(GridLayout),
              new PropertyMetadata(null, new PropertyChangedCallback(InvalidateMeasureOnChildPropertyChanged))
            );
        public static void SetColumnStart(UIElement element, GridLocation value)
        {
            element.SetValue(ColumnStartProperty, value);
        }
        public static GridLocation GetColumnStart(UIElement element)
        {
            return (GridLocation)element.GetValue(ColumnStartProperty);
        }

        public static readonly DependencyProperty ColumnEndProperty =
            DependencyProperty.RegisterAttached(
              "ColumnEnd",
              typeof(GridLocation),
              typeof(GridLayout),
              new PropertyMetadata(null, new PropertyChangedCallback(InvalidateMeasureOnChildPropertyChanged))
            );
        public static void SetColumnEnd(UIElement element, GridLocation value)
        {
            element.SetValue(ColumnEndProperty, value);
        }
        public static GridLocation GetColumnEnd(UIElement element)
        {
            return (GridLocation)element.GetValue(ColumnEndProperty);
        }

        public static readonly DependencyProperty RowStartProperty =
            DependencyProperty.RegisterAttached(
              "RowStart",
              typeof(GridLocation),
              typeof(GridLayout),
              new PropertyMetadata(null, new PropertyChangedCallback(InvalidateMeasureOnChildPropertyChanged))
            );
        public static void SetRowStart(UIElement element, GridLocation value)
        {
            element.SetValue(RowStartProperty, value);
        }
        public static GridLocation GetRowStart(UIElement element)
        {
            return (GridLocation)element.GetValue(RowStartProperty);
        }

        public static readonly DependencyProperty RowEndProperty =
            DependencyProperty.RegisterAttached(
              "RowEnd",
              typeof(GridLocation),
              typeof(GridLayout),
              new PropertyMetadata(null, new PropertyChangedCallback(InvalidateMeasureOnChildPropertyChanged))
            );
        public static void SetRowEnd(UIElement element, GridLocation value)
        {
            element.SetValue(RowEndProperty, value);
        }
        public static GridLocation GetRowEnd(UIElement element)
        {
            return (GridLocation)element.GetValue(RowEndProperty);
        }

        public static readonly DependencyProperty JustifySelfProperty =
            DependencyProperty.RegisterAttached(
              "JustifySelf",
              typeof(GridJustifySelf),
              typeof(GridLayout),
              new PropertyMetadata(null, new PropertyChangedCallback(InvalidateMeasureOnChildPropertyChanged))
            );
        public static void SetJustifySelf(UIElement element, GridJustifySelf value)
        {
            element.SetValue(JustifySelfProperty, value);
        }
        public static GridJustifySelf GetJustifySelf(UIElement element)
        {
            return (GridJustifySelf)element.GetValue(JustifySelfProperty);
        }
        public static GridJustifySelf? TryGetJustifySelf(UIElement element)
        {
            var value = element.GetValue(JustifySelfProperty);
            if (value != null)
            {
                return (GridJustifySelf)value;
            }
            return null;
        }

        public static readonly DependencyProperty AlignSelfProperty =
            DependencyProperty.RegisterAttached(
              "AlignSelf",
              typeof(GridAlignSelf),
              typeof(GridLayout),
              new PropertyMetadata(null, new PropertyChangedCallback(InvalidateMeasureOnChildPropertyChanged))
            );
        public static void SetAlignSelf(UIElement element, GridAlignSelf value)
        {
            element.SetValue(AlignSelfProperty, value);
        }
        public static GridAlignSelf GetAlignSelf(UIElement element)
        {
            return (GridAlignSelf)element.GetValue(AlignSelfProperty);
        }
        public static GridAlignSelf? TryGetAlignSelf(UIElement element)
        {
            var value = element.GetValue(AlignSelfProperty);
            if (value != null)
            {
                return (GridAlignSelf)value;
            }
            return null;
        }

        private static void InvalidateMeasureOnChildPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            Grid parent = VisualTreeHelper.GetParent(source) as Grid;
            if (parent != null)
            {
                parent.InvalidateMeasure();
            }
        }

        #endregion ChildProperties

        #region Properties

        public List<GridTrackInfo> TemplateColumns
        {
            get
            {
                return _templateColumns;
            }
            set
            {
                if (_templateColumns != value)
                {
                    _templateColumns = value;
                    InvalidateMeasure();
                }
            }
        }
        private List<GridTrackInfo> _templateColumns = new List<GridTrackInfo>();

        public List<GridTrackInfo> TemplateRows
        {
            get
            {
                return _templateRows;
            }
            set
            {
                if (_templateRows != value)
                {
                    _templateRows = value;
                    InvalidateMeasure();
                }
            }
        }
        private List<GridTrackInfo> _templateRows = new List<GridTrackInfo>();

        public double ColumnGap
        {
            get
            {
                return _columnGap;
            }
            set
            {
                if (_columnGap != value)
                {
                    _columnGap = value;
                    InvalidateMeasure();
                }
            }
        }
        private double _columnGap = 0.0;

        public double RowGap
        {
            get
            {
                return _rowGap;
            }
            set
            {
                if (_rowGap != value)
                {
                    _rowGap = value;
                    InvalidateMeasure();
                }
            }
        }
        private double _rowGap = 0.0;

        public GridJustifyItems JustifyItems
        {
            get
            {
                return _justifyItems;
            }
            set
            {
                if (_justifyItems != value)
                {
                    _justifyItems = value;
                    InvalidateMeasure();
                }
            }
        }
        private GridJustifyItems _justifyItems = GridJustifyItems.Stretch;

        public GridAlignItems AlignItems
        {
            get
            {
                return _alignItems;
            }
            set
            {
                if (_alignItems != value)
                {
                    _alignItems = value;
                    InvalidateMeasure();
                }
            }
        }
        private GridAlignItems _alignItems = GridAlignItems.Stretch;

        public GridJustifyContent JustifyContent
        {
            get
            {
                return _justifyContent;
            }
            set
            {
                if (_justifyContent != value)
                {
                    _justifyContent = value;
                    InvalidateMeasure();
                }
            }
        }
        private GridJustifyContent _justifyContent = GridJustifyContent.Start;

        public GridAlignContent AlignContent
        {
            get
            {
                return _alignContent;
            }
            set
            {
                if (_alignContent != value)
                {
                    _alignContent = value;
                    InvalidateMeasure();
                }
            }
        }
        private GridAlignContent _alignContent = GridAlignContent.Start;

        public List<GridTrackInfo> AutoColumns
        {
            get
            {
                return _autoColumns;
            }
            set
            {
                if (_autoColumns != value)
                {
                    _autoColumns = value;
                    InvalidateMeasure();
                }
            }
        }
        private List<GridTrackInfo> _autoColumns = new List<GridTrackInfo>();

        public List<GridTrackInfo> AutoRows
        {
            get
            {
                return _autoRows;
            }
            set
            {
                if (_autoRows != value)
                {
                    _autoRows = value;
                    InvalidateMeasure();
                }
            }
        }
        private List<GridTrackInfo> _autoRows = new List<GridTrackInfo>();

        public GridAutoFlow AutoFlow
        {
            get
            {
                return _autoFlow;
            }
            set
            {
                if (_autoFlow != value)
                {
                    _autoFlow = value;
                    InvalidateMeasure();
                }
            }
        }
        private GridAutoFlow _autoFlow = GridAutoFlow.Row;


        #endregion Properties

        private class GridLayoutState
        {
            public Dictionary<int, MeasuredGridTrackInfo> Columns = new Dictionary<int, MeasuredGridTrackInfo>();
            public Dictionary<int, MeasuredGridTrackInfo> Rows = new Dictionary<int, MeasuredGridTrackInfo>();
        }

        protected override void InitializeForContextCore(NonVirtualizingLayoutContext context)
        {
            GridLayoutState state = context.LayoutState as GridLayoutState;
            if (state == null)
            {
                state = new GridLayoutState();
                context.LayoutState = state;
            }
        }

        protected override void UninitializeForContextCore(NonVirtualizingLayoutContext context)
        {
        }

        private static GridTrackInfo _lastInTrack = new GridTrackInfo();

        // Calculated info on one of the grid tracks, used to carry over calculations from Measure to Arrange
        protected class MeasuredGridTrackInfo
        {
            public double Size;

            public double Start;
        }

        private struct ResolvedGridReference
        {
            public ResolvedGridReference(int index, GridTrackInfo info)
            {
                Index = index;
                Info = info;
            }

            public bool IsValid
            {
                get
                {
                    return (Index != -1);
                }
            }

            public static ResolvedGridReference Invalid
            {
                get
                {
                    return new ResolvedGridReference(-1, null);
                }
            }

            public int Index;
            public GridTrackInfo Info;
        }

        // Tracks all the intermediate calculations of one direction (row or column) of the grid
        private struct AxisInfo
        {
            public AxisInfo(List<GridTrackInfo> template, List<GridTrackInfo> auto, Dictionary<int, MeasuredGridTrackInfo> calculated)
            {
                // Add all the items from the markup defined template, plus one more grid line for 
                // the end (unless we have auto tracks, in which case they will define what happens 
                // when we go out of bounds)
                Template = new List<GridTrackInfo>(template);
                if ((auto == null) || (auto.Count == 0))
                {
                    Template.Add(_lastInTrack);
                }

                Auto = auto;

                Calculated = calculated;

                Available = 0.0;
                Remaining = 0.0;
                TotalFixed = 0.0;
                TotalFraction = 0.0;
                TotalAutos = 0;
                Gap = 0.0;
            }

            public List<GridTrackInfo> Template { get; private set; }
            public List<GridTrackInfo> Auto { get; private set; }
            public Dictionary<int, MeasuredGridTrackInfo> Calculated { get; private set; }

            public double Available;
            public double Remaining;
            public double TotalFixed;
            public double TotalFraction;
            public uint TotalAutos;
            public double Gap;

            public int EnsureIndexAvailable(int index, bool clampIfOutOfBounds = true)
            {
                if (index < Template.Count)
                {
                    return index;
                }

                if (Auto.Count == 0)
                {
                    if (clampIfOutOfBounds)
                    {
                        // Clamp to the known set of indices
                        return Template.Count - 1;
                    }
                    else
                    {
                        // Or if that's disallowed, return an invalid index
                        return -1;
                    }
                }

                // Grow the list of Templates to include this new index
                // FUTURE: Filling each of these in is obviously not the best for virtualization
                while (Template.Count <= index)
                {
                    foreach (var track in Auto)
                    {
                        DumpBegin($"Adding auto track at index {Template.Count}");
                        DumpGridTrackInfo(track);
                        DumpEnd();
                        Template.Add(track);
                    }
                }

                return index;
            }

            public ResolvedGridReference GetTrack(GridLocation location, ResolvedGridReference? previous = null, bool allowOutOfRange = true)
            {
                if (location != null)
                {
                    // Exact track index
                    int index = location.Index;
                    if (index >= 0)
                    {
                        index = EnsureIndexAvailable(index, allowOutOfRange);
                        if (index >= 0)
                        {
                            return new ResolvedGridReference(index, Template[index]);
                        }
                    }

                    // Friendly track name
                    if (!String.IsNullOrEmpty(location.LineName))
                    {
                        for (int i = 0; i < Template.Count; i++)
                        {
                            GridTrackInfo track = Template[i];
                            if (track.LineName == location.LineName)
                            {
                                return new ResolvedGridReference(i, track);
                            }
                        }
                    }
                }

                // Span relative to previous track
                if (previous.HasValue)
                {
                    // By default go 1 beyond the previous one
                    int span = 1;
                    if ((location != null) && (location.Span > 0))
                    {
                        span = location.Span;
                    }

                    int previousIndex = previous.Value.Index;
                    if (previousIndex >= 0)
                    {
                        int spanIndex = previousIndex + span;
                        spanIndex = EnsureIndexAvailable(spanIndex, allowOutOfRange);
                        if (spanIndex >= 0)
                        {
                            return new ResolvedGridReference(spanIndex, Template[spanIndex]);
                        }
                    }
                }

                return ResolvedGridReference.Invalid;
            }

            public MeasuredGridTrackInfo GetMeasuredTrack(int index)
            {
                return Calculated[index];
            }

            public MeasuredGridTrackInfo GetMeasuredTrackSafe(ResolvedGridReference track)
            {
                if (!track.IsValid)
                {
                    // TODO: Is this a programming error?
                    return new MeasuredGridTrackInfo();
                }

                MeasuredGridTrackInfo info;
                if (Calculated.TryGetValue(track.Index, out info))
                {
                    return info;
                }

                // TODO: Is this a programming error?
                return new MeasuredGridTrackInfo();
            }

            public void AddCalculated(int index, GridTrackInfo track, double size)
            {
                Calculated[index] = new MeasuredGridTrackInfo { Size = size, Start = 0.0 };
            }
        }

        private struct ChildGridLocations
        {
            public ResolvedGridReference ColStart;
            public ResolvedGridReference RowStart;
            public ResolvedGridReference ColEnd;
            public ResolvedGridReference RowEnd;
        }
        private struct GridCellIndex
        {
            public int ColumnIndex;
            public int RowIndex;
        }
        private void MarkOccupied(ChildGridLocations childLocation, Dictionary<GridCellIndex, bool> occupied)
        {
            if (!childLocation.ColStart.IsValid || !childLocation.RowStart.IsValid)
            {
                return;
            }

            for (int column = childLocation.ColStart.Index; column < childLocation.ColEnd.Index; column++)
            {
                for (int row = childLocation.RowStart.Index; row < childLocation.RowEnd.Index; row++)
                {
                    DumpInfo($"Mark occupied {{{column},{row}}}");
                    occupied[new GridCellIndex { ColumnIndex = column, RowIndex = row }] = true;
                }
            }
        }

        private static AxisInfo InitializeMeasure(List<GridTrackInfo> template, List<GridTrackInfo> auto, Dictionary<int, MeasuredGridTrackInfo> calculated, double gap, double available)
        {
            int numberOfGaps = (template.Count - 1);
            if ((gap > 0.0) && (numberOfGaps > 0))
            {
                available -= (gap * numberOfGaps);
            }

            AxisInfo measure = new AxisInfo(template, auto, calculated);
            measure.Available = available;
            measure.Gap = gap;

            return measure;
        }

        private static void ProcessFixedSizes(ref AxisInfo measure)
        {
            for (int i = 0; i < measure.Template.Count; i++)
            {
                GridTrackInfo track = measure.Template[i];

                double fixedSize = track.Length;

                // Percentage is effectively a fixed size in that it needs to be applied before any
                // of the more relative sizes (fraction, auto, etc.)
                if ((fixedSize == 0.0) && (track.Percentage != 0.0))
                {
                    fixedSize = (track.Percentage * measure.Available);
                }

                measure.TotalFixed += fixedSize;

                // Accumulate the fractional sizes now so we know how many pieces of pie to dish out
                measure.TotalFraction += track.Fraction;

                if (track.Auto)
                {
                    measure.TotalAutos++;
                }

                measure.AddCalculated(i, track, fixedSize);
            }

            measure.Remaining = Math.Max(measure.Available - measure.TotalFixed, 0.0);
        }

        private ChildGridLocations GetChildGridLocations(UIElement child, Dictionary<UIElement, ChildGridLocations> cache)
        {
            ChildGridLocations result;
            if (cache.TryGetValue(child, out result))
            {
                return result;
            }

            throw new InvalidOperationException("All children should be processed into the cache before using this method");
        }

        private ChildGridLocations? GetChildGridLocations(UIElement child, ref AxisInfo horizontal, ref AxisInfo vertical)
        {
            ChildGridLocations result;

            // Read preferences off the child
            GridLocation colStart = GetColumnStart(child);
            GridLocation rowStart = GetRowStart(child);

            // We need a starting point in order to resolve the location. Save this item for the
            // second pass (AutoFlow)
            if ((colStart == null) || (rowStart == null))
            {
                return null;
            }

            GridLocation colEnd = GetColumnEnd(child);
            GridLocation rowEnd = GetRowEnd(child);

            // Map the preferences to actual grid lines
            result.ColStart = horizontal.GetTrack(colStart);
            result.RowStart = vertical.GetTrack(rowStart);
            result.ColEnd = horizontal.GetTrack(colEnd, result.ColStart);
            result.RowEnd = vertical.GetTrack(rowEnd, result.RowStart);

            return result;
        }

        private bool ProcessAutoSizes(ref AxisInfo measureHorizontal, ref AxisInfo measureVertical, Dictionary<UIElement, ChildGridLocations> locationCache, NonVirtualizingLayoutContext context)
        {
            if ((measureHorizontal.TotalAutos == 0) && (measureVertical.TotalAutos == 0))
            {
                return false;
            }

            foreach (UIElement child in context.Children)
            {
                ChildGridLocations childLocation = GetChildGridLocations(child, locationCache);

                List<GridTrackInfo> autoHorizontal = new List<GridTrackInfo>();
                List<GridTrackInfo> autoVertical = new List<GridTrackInfo>();

                Func<ResolvedGridReference, ResolvedGridReference, AxisInfo, List<GridTrackInfo>, bool> getAutoTracks = (ResolvedGridReference start, ResolvedGridReference end, AxisInfo measure, List<GridTrackInfo> autoTracks) =>
                {
                    if (!start.IsValid || !end.IsValid)
                    {
                        return false;
                    }

                    int startIndex = start.Index;
                    int endIndex = end.Index;

                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        GridTrackInfo track = measure.Template[i];
                        if (track.Auto)
                        {
                            autoTracks.Add(track);
                        }
                    }
                    return (autoTracks.Count > 0);
                };

                bool affectsHorizontalAuto = getAutoTracks(childLocation.ColStart, childLocation.ColEnd, measureHorizontal, autoHorizontal);
                bool affectsVerticalAuto = getAutoTracks(childLocation.RowStart, childLocation.RowEnd, measureVertical, autoVertical);

                // If none of the grid tracks are Auto then we can skip this item
                if (!affectsHorizontalAuto && !affectsVerticalAuto)
                {
                    continue;
                }

                // Measure this child to see how much of the available space they would take.
                // Note that even if there are multiple Autos we don't attempt to preemptively split 
                // the space between them. They are all given a crack at being greedy.
                Size measureSize = new Size(measureHorizontal.Remaining, measureVertical.Remaining);
                child.Measure(measureSize);
                DumpInfo($"Child inside Auto range measured to {child.DesiredSize}");

                // Update that row/column with the dimensions
                UpdateAutoBasedOnMeasured(autoHorizontal, ref measureHorizontal, child.DesiredSize.Width);
                UpdateAutoBasedOnMeasured(autoVertical, ref measureVertical, child.DesiredSize.Height);
            }

            return true;
        }

        // NOTE: Can't do as an anonymous inline Action above because we need to declare the struct AxisInfo as ref (and Actions don't support ref parameters)
        private static void UpdateAutoBasedOnMeasured(List<GridTrackInfo> tracks, ref AxisInfo measure, double childDesired)
        {
            if (tracks.Count == 0)
            {
                return;
            }

            double autoSlice = (childDesired / tracks.Count);

            for (int i = 0; i < tracks.Count; i++)
            {
                MeasuredGridTrackInfo info = measure.GetMeasuredTrack(i);
                double moreSize = (autoSlice - info.Size);
                if (moreSize > 0)
                {
                    DumpInfo($"Increasing Auto size track {i} by {moreSize}");
                    info.Size += moreSize;
                    measure.Remaining = Math.Max(measure.Remaining - moreSize, 0.0);
                }
            }
        }

        private static bool ProcessFractionalSizes(ref AxisInfo measure)
        {
            if (measure.TotalFraction <= 0.0)
            {
                return false;
            }

            // What is the size of each fraction?
            double fractionSlice = measure.Remaining / measure.TotalFraction;

            for (int i = 0; i < measure.Template.Count; i++)
            {
                GridTrackInfo track = measure.Template[i];
                if (track.Fraction == 0.0)
                {
                    continue;
                }

                // We only apply the fraction if the item didn't also have a fixed size
                MeasuredGridTrackInfo info = measure.GetMeasuredTrack(i);
                if (info.Size != 0.0)
                {
                    continue;
                }

                double myFractionSlice = track.Fraction * fractionSlice;
                info.Size = myFractionSlice;
            }

            // Fractions consume all that's left
            measure.Remaining = 0.0;

            return true;
        }

        // Allow any Auto tracks to take the remaining space
        private static void ProcessAutoRemainingSize(ref AxisInfo measure)
        {
            if (measure.TotalAutos <= 0)
            {
                return;
            }

            // If there were fractional elements they would have already taken up all the space
            if (measure.Remaining <= 0.0)
            {
                return;
            }

            // How much extra space do we give up to each Auto element?
            double autoSlice = measure.Remaining / (double)measure.TotalAutos;

            for (int i = 0; i < measure.Template.Count; i++)
            {
                GridTrackInfo track = measure.Template[i];
                if (!track.Auto)
                {
                    continue;
                }

                MeasuredGridTrackInfo info = measure.GetMeasuredTrack(i);
                info.Size += autoSlice;
            }

            // We've consumed all that's left
            measure.Remaining = 0.0;
        }


        private double ProcessOffsets(ref AxisInfo measure)
        {
            double offset = 0.0;
            // TODO: This assumes the dictionary is ordered
            foreach (var entry in measure.Calculated)
            {
                entry.Value.Start = offset;
                offset += entry.Value.Size;
                offset += measure.Gap;
            }

            return offset;
        }

        delegate bool CheckCell(GridCellIndex cell, ref AxisInfo horizontal, ref AxisInfo vertical);

        private void TraverseByColumn(ref AxisInfo horizontal, ref AxisInfo vertical, CheckCell predicate)
        {
            for (int column = 0; column < horizontal.Template.Count; column++)
            {
                // Don't consider the last grid line an option (it's meant to be an upper bound, not a starter).
                if (horizontal.Template[column] == _lastInTrack)
                {
                    break;
                }

                for (int row = 0; row < vertical.Template.Count; row++)
                {
                    // Don't consider the last grid line an option (it's meant to be an upper bound, not a starter).
                    if (vertical.Template[row] == _lastInTrack)
                    {
                        break;
                    }

                    if (predicate(new GridCellIndex { ColumnIndex = column, RowIndex = row }, ref horizontal, ref vertical))
                    {
                        return;
                    }

                    // Ping the next track to potentially fault it in if AutoRows are specified
                    int nextRow = vertical.EnsureIndexAvailable(row + 1);
                }

                // Ping the next track to potentially fault it in if AutoColumns are specified
                int nextColumn = horizontal.EnsureIndexAvailable(column + 1);
            }
        }

        // TODO: This code can be more smartly shared with TraverseByColumn
        private void TraverseByRow(ref AxisInfo horizontal, ref AxisInfo vertical, CheckCell predicate)
        {
            for (int row = 0; row < vertical.Template.Count; row++)
            {
                // Don't consider the last grid line an option (it's meant to be an upper bound, not a starter).
                if (vertical.Template[row] == _lastInTrack)
                {
                    break;
                }

                for (int column = 0; column < horizontal.Template.Count; column++)
                {
                    // Don't consider the last grid line an option (it's meant to be an upper bound, not a starter).
                    if (horizontal.Template[column] == _lastInTrack)
                    {
                        break;
                    }

                    if (predicate(new GridCellIndex { ColumnIndex = column, RowIndex = row }, ref horizontal, ref vertical))
                    {
                        return;
                    }

                    // Ping the next track to potentially fault it in if AutoColumns are specified
                    int nextColumn = horizontal.EnsureIndexAvailable(column + 1);
                }

                // Ping the next track to potentially fault it in if AutoRows are specified
                int nextRow = vertical.EnsureIndexAvailable(row + 1);
            }
        }

        private ChildGridLocations AssignUnoccupiedGridLocation(UIElement child, ref AxisInfo horizontal, ref AxisInfo vertical, GridAutoFlow autoFlow, Dictionary<GridCellIndex, bool> occupied)
        {
            ChildGridLocations result;

            // Read preferences off the child
            GridLocation colStart = GetColumnStart(child);
            GridLocation rowStart = GetRowStart(child);
            GridLocation colEnd = GetColumnEnd(child);
            GridLocation rowEnd = GetRowEnd(child);

            Debug.Assert((colStart == null) || (rowStart == null));

            CheckCell checkUnoccupied = (GridCellIndex topLeft, ref AxisInfo horizontalAxis, ref AxisInfo verticalAxis) =>
            {
                GridCellIndex bottomRight = new GridCellIndex { ColumnIndex = topLeft.ColumnIndex + 1, RowIndex = topLeft.RowIndex + 1 };

                // Handle a specified End paired with our unspecified Start
                if (colEnd != null)
                {
                    ResolvedGridReference right = horizontalAxis.GetTrack(colEnd, new ResolvedGridReference(topLeft.ColumnIndex, null), allowOutOfRange: false);
                    if (!right.IsValid)
                    {
                        return false;
                    }
                    bottomRight.ColumnIndex = right.Index;
                }
                if (rowEnd != null)
                {
                    ResolvedGridReference bottom = verticalAxis.GetTrack(rowEnd, new ResolvedGridReference(topLeft.RowIndex, null), allowOutOfRange: false);
                    if (!bottom.IsValid)
                    {
                        return false;
                    }
                    bottomRight.RowIndex = bottom.Index;
                }

                // Make sure each individual cell is unoccupied in the whole span
                DumpInfo($"Testing {{{topLeft.ColumnIndex},{topLeft.RowIndex}}} to {{{bottomRight.ColumnIndex},{bottomRight.RowIndex}}}");
                for (int col = topLeft.ColumnIndex; col < bottomRight.ColumnIndex; col++)
                {
                    for (int row = topLeft.RowIndex; row < bottomRight.RowIndex; row++)
                    {
                        GridCellIndex testCoordinate = new GridCellIndex { ColumnIndex = col, RowIndex = row };
                        if (occupied.ContainsKey(testCoordinate))
                        {
                            return false;
                        }
                    }
                }

                DumpInfo($"Found unoccupied {{{topLeft.ColumnIndex},{topLeft.RowIndex}}} to {{{bottomRight.ColumnIndex},{bottomRight.RowIndex}}}");
                colStart = new GridLocation { Index = topLeft.ColumnIndex };
                rowStart = new GridLocation { Index = topLeft.RowIndex };

                return true;
            };

            // The child has no preference. Find them the first available spot according to the 
            // AutoFlow policy.
            // TODO: Implement difference between Dense and not Dense
            // TODO: Should Dense be a different enum value or a separate property (bool AutoFlowDense)
            switch (autoFlow)
            {
                case GridAutoFlow.Column:
                case GridAutoFlow.ColumnDense:
                    TraverseByColumn(ref horizontal, ref vertical, checkUnoccupied);
                    break;

                case GridAutoFlow.Row:
                case GridAutoFlow.RowDense:
                    TraverseByRow(ref horizontal, ref vertical, checkUnoccupied);
                    break;
            }

            if (colStart != null)
            {
                DumpGridTrackInfo(horizontal.Template[colStart.Index]);
            }
            else
            {
                DumpInfo("Unable to find column for child");
            }

            if (rowStart != null)
            {
                DumpGridTrackInfo(vertical.Template[rowStart.Index]);
            }
            else
            {
                DumpInfo("Unable to find row for child");
            }

            // Map the preferences to actual grid lines
            result.ColStart = horizontal.GetTrack(colStart);
            result.RowStart = vertical.GetTrack(rowStart);
            result.ColEnd = horizontal.GetTrack(colEnd, result.ColStart);
            result.RowEnd = vertical.GetTrack(rowEnd, result.RowStart);

            return result;
        }

        private Dictionary<UIElement, ChildGridLocations> ResolveGridLocations(ref AxisInfo horizontal, ref AxisInfo vertical, NonVirtualizingLayoutContext context)
        {
            Dictionary<UIElement, ChildGridLocations> locationCache = new Dictionary<UIElement, ChildGridLocations>();
            Dictionary<GridCellIndex, bool> occupied = new Dictionary<GridCellIndex, bool>();

            // Mark any known grid coordinates as occupied
            foreach (UIElement child in context.Children)
            {
                ChildGridLocations? childLocation = GetChildGridLocations(child, ref horizontal, ref vertical);
                if (childLocation.HasValue)
                {
                    MarkOccupied(childLocation.Value, occupied);
                    locationCache[child] = childLocation.Value;
                }
            }

            // Go find places for all the unspecified items
            for (int i = 0; i < context.Children.Count; i++)
            {
                UIElement child = context.Children[i];
                if (locationCache.ContainsKey(child))
                {
                    continue;
                }

                DumpBegin($"Finding space for unspecified child {i} {child.GetType().Name} (according to AutoFlow {_autoFlow})");
                ChildGridLocations childLocation = AssignUnoccupiedGridLocation(child, ref horizontal, ref vertical, _autoFlow, occupied);
                MarkOccupied(childLocation, occupied);
                locationCache[child] = childLocation;
                DumpEnd();
            }

            return locationCache;
        }

        private GridJustifyItems Convert(GridJustifySelf value)
        {
            switch (value)
            {
                case GridJustifySelf.Start: return GridJustifyItems.Start;
                case GridJustifySelf.Center: return GridJustifyItems.Center;
                case GridJustifySelf.End: return GridJustifyItems.End;
                case GridJustifySelf.Stretch: return GridJustifyItems.Stretch;
                default: throw new ArgumentOutOfRangeException("value");
            }
        }

        private GridAlignItems Convert(GridAlignSelf value)
        {
            switch (value)
            {
                case GridAlignSelf.Start: return GridAlignItems.Start;
                case GridAlignSelf.Center: return GridAlignItems.Center;
                case GridAlignSelf.End: return GridAlignItems.End;
                case GridAlignSelf.Stretch: return GridAlignItems.Stretch;
                default: throw new ArgumentOutOfRangeException("value");
            }
        }


        #region Tracing
        [Conditional("GRID_TRACE")]
        private static void DumpConditional(bool condition, string write, ref string separator)
        {
            if (condition)
            {
                Debug.Write(separator + write);
                separator = ", ";
            }
        }

        [Conditional("GRID_TRACE")]
        private static void DumpGridTrackInfo(GridTrackInfo track)
        {
            Debug.Write("{");
            string separator = String.Empty;
            DumpConditional(!String.IsNullOrEmpty(track.LineName), $"LineName='{track.LineName}'", ref separator);
            DumpConditional(track.Length != 0.0, $"Length={track.Length}", ref separator);
            DumpConditional(track.Percentage != 0.0, $"Percentage={track.Percentage}", ref separator);
            DumpConditional(track.Fraction != 0.0, $"Fraction={track.Fraction}", ref separator);
            DumpConditional(track.Auto, $"Auto={track.Auto}", ref separator);
            Debug.WriteLine("}");
        }

        [Conditional("GRID_TRACE")]
        private void DumpTemplates()
        {
            Action<List<GridTrackInfo>> dumpTemplate = (List<GridTrackInfo> template) =>
            {
                for (int i = 0; i < template.Count; i++)
                {
                    GridTrackInfo track = template[i];
                    Debug.Write($"{i} ");
                    DumpGridTrackInfo(track);
                }
            };

            DumpBegin("TemplateColumns");
            dumpTemplate(_templateColumns);
            DumpEnd();

            DumpBegin("TemplateRows");
            dumpTemplate(_templateRows);
            DumpEnd();

            if ((_autoColumns != null) && (_autoColumns.Count > 0))
            {
                DumpBegin("AutoColumns");
                dumpTemplate(_autoColumns);
                DumpEnd();
            }

            if ((_autoRows != null) && (_autoRows.Count > 0))
            {
                DumpBegin("AutoRows");
                dumpTemplate(_autoRows);
                DumpEnd();
            }
        }

        [Conditional("GRID_TRACE")]
        private void DumpChildren(ref AxisInfo measureHorizontal, ref AxisInfo measureVertical, NonVirtualizingLayoutContext context)
        {
            DumpBegin("Children");
            foreach (UIElement child in context.Children)
            {
                Debug.WriteLine(child.GetType().Name + " {");
                Debug.Indent();

                Action<GridLocation, string> dumpLocation = (GridLocation location, string info) =>
                {
                    Debug.Write(info);
                    if (location == null)
                    {
                        return;
                    }

                    string separator = String.Empty;
                    DumpConditional(location.Index >= 0, $"Index={location.Index}", ref separator);
                    DumpConditional(!String.IsNullOrEmpty(location.LineName), $"LineName='{location.LineName}'", ref separator);
                    DumpConditional(location.Span > 0, $"Span={location.Span}", ref separator);
                };

                dumpLocation(GetColumnStart(child), "Column {");
                dumpLocation(GetColumnEnd(child), "} to {");
                Debug.WriteLine("}");
                dumpLocation(GetRowStart(child), "Row {");
                dumpLocation(GetRowEnd(child), "} to {");
                Debug.WriteLine("}");
                Debug.Unindent();
                Debug.WriteLine("}");
            }
            DumpEnd();
        }

        [Conditional("GRID_TRACE")]
        private static void DumpMeasureInfo(ref AxisInfo measure, string info, bool includeOffset = false)
        {
            DumpBegin(info);
            foreach (var entry in measure.Calculated)
            {
                int trackIndex = entry.Key;
                if (includeOffset)
                {
                    Debug.WriteLine($"{trackIndex} {{Size={entry.Value.Size}, Start={entry.Value.Start}}}");
                }
                else
                {
                    Debug.WriteLine($"{trackIndex} {{Size={entry.Value.Size}}}");
                }

            }
            DumpEnd();
        }

        [Conditional("GRID_TRACE")]
        private static void DumpMeasureInfo(ref AxisInfo measureHorizontal, ref AxisInfo measureVertical, string info, bool includeOffset = false)
        {
            DumpBegin(info);
            DumpMeasureInfo(ref measureHorizontal, "Columns", includeOffset);
            Debug.WriteLine($"Remaining={measureHorizontal.Remaining}");
            DumpMeasureInfo(ref measureVertical, "Rows", includeOffset);
            Debug.WriteLine($"Remaining={measureVertical.Remaining}");
            DumpEnd();
        }

        [Conditional("GRID_TRACE")]
        private static void DumpBegin(Size size, string info)
        {
            Debug.WriteLine($"{info}({size.Width}, {size.Height}) {{");
            Debug.Indent();
        }

        [Conditional("GRID_TRACE")]
        private static void DumpBegin(string info)
        {
            Debug.WriteLine($"{info} {{");
            Debug.Indent();
        }

        [Conditional("GRID_TRACE")]
        private static void DumpEnd()
        {
            Debug.Unindent();
            Debug.WriteLine("}");
        }

        [Conditional("GRID_TRACE")]
        private static void DumpInfo(string info)
        {
            Debug.WriteLine(info);
        }
        #endregion

        protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
        {
            DumpBegin(availableSize, "Measure");
            DumpTemplates();
            DumpInfo($"ColumnGap={_columnGap}, RowGap={_rowGap}");
            DumpInfo($"AutoFlow={_autoFlow}");

            GridLayoutState state = (GridLayoutState)(context.LayoutState);
            state.Columns.Clear();
            state.Rows.Clear();

            AxisInfo measureHorizontal = InitializeMeasure(_templateColumns, _autoColumns, state.Columns, _columnGap, availableSize.Width);
            AxisInfo measureVertical = InitializeMeasure(_templateRows, _autoRows, state.Rows, _rowGap, availableSize.Height);
            DumpChildren(ref measureHorizontal, ref measureVertical, context);

            // Resolve all grid references
            Dictionary<UIElement, ChildGridLocations> locationCache = ResolveGridLocations(ref measureHorizontal, ref measureVertical, context);

            // First process any fixed sizes
            ProcessFixedSizes(ref measureHorizontal);
            ProcessFixedSizes(ref measureVertical);
            DumpMeasureInfo(ref measureHorizontal, ref measureVertical, "Fixed");

            // Next we need to know how large the auto sizes are
            bool anyAuto = ProcessAutoSizes(ref measureHorizontal, ref measureVertical, locationCache, context);
            if (anyAuto)
            {
                DumpMeasureInfo(ref measureHorizontal, ref measureVertical, "Auto");
            }

            // Then we can figure out how large the fractional sizes should be
            bool anyFractional = false;
            anyFractional |= ProcessFractionalSizes(ref measureHorizontal);
            anyFractional |= ProcessFractionalSizes(ref measureVertical);
            if (anyFractional)
            {
                DumpMeasureInfo(ref measureHorizontal, ref measureVertical, "Fractional");
            }

            // And then the auto elements can claim any remaining sizes
            ProcessAutoRemainingSize(ref measureHorizontal);
            ProcessAutoRemainingSize(ref measureVertical);
            if (anyAuto)
            {
                DumpMeasureInfo(ref measureHorizontal, ref measureVertical, "Auto remainder");
            }

            foreach (UIElement child in context.Children)
            {
                ChildGridLocations childLocation = GetChildGridLocations(child, locationCache);

                MeasuredGridTrackInfo colMeasure = measureHorizontal.GetMeasuredTrackSafe(childLocation.ColStart);
                MeasuredGridTrackInfo rowMeasure = measureVertical.GetMeasuredTrackSafe(childLocation.RowStart);

                // TODO: This isn't measuring them against their entire span
                Size measureSize = new Size(colMeasure.Size, rowMeasure.Size);
                child.Measure(measureSize);
            }

            // Now that the sizes are known we can calculate the offsets for the grid tracks
            double width = ProcessOffsets(ref measureHorizontal);
            double height = ProcessOffsets(ref measureVertical);
            DumpMeasureInfo(ref measureHorizontal, ref measureVertical, "Calculate offsets", includeOffset: true);

            // If there's no entry for columns/rows use the minimal size, otherwise use the whole space.
            // TODO: This should be derived from our calculated Remaining numbers
            //if (_templateColumns.Count > 0)
            //{
            //    width = availableSize.Width;
            //}
            //if (_templateRows.Count > 0)
            //{
            //    height = availableSize.Height;
            //}

            DumpEnd();
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
        {
            DumpBegin(finalSize, "Arrange");

            double extraWidth = 0.0; // finalSize.Width - DesiredSize.Width; // TODO: How to get DesiredSize from context?
            double rootOffsetX = 0.0;

            switch (_justifyContent)
            {
                case GridJustifyContent.Start:
                    break;

                case GridJustifyContent.End:
                    rootOffsetX = extraWidth;
                    break;

                case GridJustifyContent.Center:
                    rootOffsetX = extraWidth * 0.5;
                    break;

                case GridJustifyContent.SpaceAround:
                case GridJustifyContent.SpaceBetween:
                case GridJustifyContent.SpaceEvenly:
                    // TODO: Implement
                    break;
            }

            double extraHeight = 0.0f; //  finalSize.Height - DesiredSize.Height; // TODO: How to get DesiredSize from context?
            double rootOffsetY = 0.0;

            switch (_alignContent)
            {
                case GridAlignContent.Start:
                    break;

                case GridAlignContent.End:
                    rootOffsetY = extraHeight;
                    break;

                case GridAlignContent.Center:
                    rootOffsetY = extraHeight * 0.5;
                    break;

                case GridAlignContent.SpaceAround:
                case GridAlignContent.SpaceBetween:
                case GridAlignContent.SpaceEvenly:
                    // TODO: Implement
                    break;
            }

            GridLayoutState state = (GridLayoutState)(context.LayoutState);

            // TODO: Avoid recreating these lists
            AxisInfo measureHorizontal = InitializeMeasure(_templateColumns, _autoColumns, state.Columns, _columnGap, finalSize.Width);
            AxisInfo measureVertical = InitializeMeasure(_templateRows, _autoRows, state.Rows, _rowGap, finalSize.Height);

            // Resolve all grid references
            Dictionary<UIElement, ChildGridLocations> locationCache = ResolveGridLocations(ref measureHorizontal, ref measureVertical, context);

            foreach (UIElement child in context.Children)
            {
                ChildGridLocations childLocation = GetChildGridLocations(child, locationCache);

                if (!childLocation.ColStart.IsValid || !childLocation.RowStart.IsValid)
                {
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                MeasuredGridTrackInfo colMeasure = measureHorizontal.GetMeasuredTrackSafe(childLocation.ColStart);
                MeasuredGridTrackInfo rowMeasure = measureVertical.GetMeasuredTrackSafe(childLocation.RowStart);
                MeasuredGridTrackInfo colEndMesure = measureHorizontal.GetMeasuredTrackSafe(childLocation.ColEnd);
                MeasuredGridTrackInfo rowEndMesure = measureVertical.GetMeasuredTrackSafe(childLocation.RowEnd);

                double left = colMeasure.Start;
                double top = rowMeasure.Start;
                double right = colEndMesure.Start;
                double bottom = rowEndMesure.Start;

                // The left edge of a grid line includes the gap amount. As long as we have any
                // coordinates at all we should be trimming that off.
                if (right > 0.0)
                {
                    right -= _columnGap;
                }
                if (bottom > 0.0)
                {
                    bottom -= _rowGap;
                }

                // They might have specified grid references that were inverted. If so, collapse
                // them and essentially zero out the child's arrange size.
                if (right < left)
                {
                    right = left;
                }
                if (bottom < top)
                {
                    bottom = top;
                }

                double width = (right - left);
                double height = (bottom - top);

                double desiredWidth = Math.Min(child.DesiredSize.Width, width);
                double unusedWidth = (width - desiredWidth);
                GridJustifyItems justify = _justifyItems;
                GridJustifySelf? justifySelf = TryGetJustifySelf(child);
                if (justifySelf.HasValue)
                {
                    justify = Convert(justifySelf.Value);
                }
                switch (justify)
                {
                    case GridJustifyItems.Start:
                        width = desiredWidth;
                        break;
                    case GridJustifyItems.End:
                        left += unusedWidth;
                        width = desiredWidth;
                        break;
                    case GridJustifyItems.Center:
                        left += unusedWidth * 0.5;
                        width = desiredWidth;
                        break;
                    case GridJustifyItems.Stretch:
                        break;
                }

                double desiredHeight = Math.Min(child.DesiredSize.Height, height);
                double unusedHeight = (height - desiredHeight);
                GridAlignItems align = _alignItems;
                GridAlignSelf? alignSelf = TryGetAlignSelf(child);
                if (alignSelf.HasValue)
                {
                    align = Convert(alignSelf.Value);
                }
                switch (align)
                {
                    case GridAlignItems.Start:
                        height = desiredHeight;
                        break;
                    case GridAlignItems.End:
                        top += unusedHeight;
                        height = desiredHeight;
                        break;
                    case GridAlignItems.Center:
                        top += unusedHeight * 0.5;
                        height = desiredHeight;
                        break;
                    case GridAlignItems.Stretch:
                        break;
                }

                DumpBegin(child.GetType().Name);
                DumpInfo("leftTrack=" + childLocation.ColStart.Index);
                DumpInfo("topTrack=" + childLocation.RowStart.Index);
                DumpInfo("rightTrack=" + childLocation.ColEnd.Index);
                DumpInfo("bottomTrack=" + childLocation.RowEnd.Index);
                DumpInfo($"left={left}, top={top}, right={right}, bottom={bottom}");
                DumpEnd();

                Rect arrangeRect = new Rect(rootOffsetX + left, rootOffsetY + top, width, height);
                child.Arrange(arrangeRect);
            }

            DumpEnd();
            return finalSize;
        }
    }
}