﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Linq;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using OpenTK;
using OpenTK.Graphics;

namespace osu.Framework.Graphics.Visualisation
{
    internal enum TreeContainerStatus
    {
        Onscreen,
        Offscreen
    }

    internal class TreeContainer : Container, IStateful<TreeContainerStatus>
    {
        private readonly ScrollContainer scroll;

        private readonly SpriteText waitingText;

        public Action ChooseTarget;
        public Action GoUpOneParent;
        public Action ToggleProperties;

        protected override Container<Drawable> Content => scroll;

        private readonly Box titleBar;

        private const float width = 400;
        private const float height = 600;

        internal PropertyDisplay PropertyDisplay { get; private set; }

        private TreeContainerStatus state;

        public TreeContainerStatus State
        {
            get { return state; }

            set
            {
                state = value;

                switch (state)
                {
                    case TreeContainerStatus.Offscreen:
                        using (BeginDelayedSequence(500, true))
                        {
                            FadeTo(0.7f, 300);
                        }
                        break;
                    case TreeContainerStatus.Onscreen:
                        FadeIn(300);
                        break;
                }
            }
        }

        public TreeContainer()
        {
            Masking = true;
            CornerRadius = 5;
            Position = new Vector2(100, 100);

            AutoSizeAxes = Axes.X;
            Height = height;

            AddInternal(new Drawable[]
            {
                new Box
                {
                    Colour = new Color4(30, 30, 30, 240),
                    RelativeSizeAxes = Axes.Both,
                    Depth = 0
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        titleBar = new Box //title decoration
                        {
                            Colour = Color4.DarkBlue,
                            RelativeSizeAxes = Axes.X,
                            Size = new Vector2(1, 20),
                        },
                        new Container //toolbar
                        {
                            RelativeSizeAxes = Axes.X,
                            Size = new Vector2(1, 40),
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    Colour = new Color4(20, 20, 20, 255),
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Spacing = new Vector2(1),
                                    Children = new Drawable[]
                                    {
                                        new Button
                                        {
                                            BackgroundColour = Color4.DarkGray,
                                            Size = new Vector2(100, 1),
                                            RelativeSizeAxes = Axes.Y,
                                            Text = @"Choose Target",
                                            Action = delegate { ChooseTarget?.Invoke(); }
                                        },
                                        new Button
                                        {
                                            BackgroundColour = Color4.DarkGray,
                                            Size = new Vector2(100, 1),
                                            RelativeSizeAxes = Axes.Y,
                                            Text = @"Up one parent",
                                            Action = delegate { GoUpOneParent?.Invoke(); },
                                        },
                                        new Button
                                        {
                                            BackgroundColour = Color4.DarkGray,
                                            Size = new Vector2(100, 1),
                                            RelativeSizeAxes = Axes.Y,
                                            Text = @"Properties",
                                            Action = delegate { ToggleProperties?.Invoke(); },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Padding = new MarginPadding { Top = 65 },
                    Children = new Drawable[]
                    {
                        scroll = new ScrollContainer
                        {
                            Padding = new MarginPadding(10),
                            RelativeSizeAxes = Axes.Y,
                            Width = width
                        },
                        PropertyDisplay = new PropertyDisplay()
                    }
                },
                waitingText = new SpriteText
                {
                    Text = @"Waiting for target selection...",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            });
        }

        protected override void Update()
        {
            waitingText.Alpha = scroll.Children.Any() ? 0 : 1;
            base.Update();
        }

        protected override bool OnHover(InputState state)
        {
            State = TreeContainerStatus.Onscreen;
            return true;
        }

        protected override void OnHoverLost(InputState state)
        {
            State = TreeContainerStatus.Offscreen;
            base.OnHoverLost(state);
        }

        protected override bool OnDragStart(InputState state) => titleBar.Contains(state.Mouse.NativeState.Position);

        protected override bool OnDrag(InputState state)
        {
            Position += state.Mouse.Delta;
            return base.OnDrag(state);
        }

        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args) => true;

        protected override bool OnClick(InputState state) => true;

        protected override void LoadComplete()
        {
            base.LoadComplete();
            State = TreeContainerStatus.Offscreen;
        }
    }
}
