﻿using System;
using UnityEngine;

namespace RosettaUI
{
    public static class ElementExtensionsMethodChain
    {
        public static T SetEnable<T>(this T e, bool enable)
            where T : Element
        {
            e.Enable = enable;
            return e;
        }

        public static T SetInteractable<T>(this T e, bool interactable)
            where T : Element
        {
            e.Interactable = interactable;
            return e;
        }

        public static T SetWidth<T>(this T element, float? width)
            where T : Element
        {
            element.Style.Width = width;
            return element;
        }

        public static T SetHeight<T>(this T element, float? height)
            where T : Element
        {
            element.Style.Height = height;
            return element;
        }

        public static Element SetMinWidth(this Element element, float? minWidth)
        {
            element.Style.MinWidth = minWidth;
            return element;
        }

        public static Element SetMinHeight(this Element element, float? minHeight)
        {
            element.Style.MinHeight = minHeight;
            return element;
        }
        
        public static Element SetMaxWidth(this Element element, float? maxWidth)
        {
            element.Style.MaxWidth = maxWidth;
            return element;
        }

        public static Element SetMaxHeight(this Element element, float? maxHeight)
        {
            element.Style.MaxHeight = maxHeight;
            return element;
        }

        public static Element SetColor(this Element element, Color? color)
        {
            element.Style.Color = color;
            return element;
        }
        
        public static Element SetBackgroundColor(this Element element, Color? color)
        {
            element.Style.BackgroundColor = color;
            return element;
        }
        
        public static Element RegisterValueChangeCallback(this Element element, Action onValueChanged)
        {
            element.onViewValueChanged += onValueChanged;
            return element; 
        }
        public static Element UnregisterValueChangeCallback(this Element element, Action onValueChanged)
        {
            element.onViewValueChanged -= onValueChanged;
            return element;
        }


        public static Element RegisterUpdateCallback(this Element element, Action<Element> onUpdate)
        {
            element.onUpdate += onUpdate;
            return element;
        }

        public static Element UnregisterUpdateCallback(this Element element, Action<Element> onUpdate)
        {
            element.onUpdate -= onUpdate;
            return element;
        }
        
        
        public static T SetOpenFlag<T>(this T element, bool flag)
            where T : OpenCloseBaseElement
        {
            element.IsOpen = flag;
            return element;
        }

        public static T Open<T>(this T element) where T : OpenCloseBaseElement
            => element.SetOpenFlag(true);

        public static T Close<T>(this T element) where T : OpenCloseBaseElement
            => element.SetOpenFlag(false);

        public static WindowElement SetPosition(this WindowElement windowElement, Vector2? position)
        {
            windowElement.Position = position;
            return windowElement;
        }
    }
}