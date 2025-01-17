using System;
using System.Collections.Generic;
using RosettaUI.Builder;
using RosettaUI.UIToolkit.UnityInternalAccess;
using UnityEngine;
using UnityEngine.UIElements;

namespace RosettaUI.UIToolkit.Builder
{
    public partial class UIToolkitBuilder : BuilderBase<VisualElement>
    {
        #region Static

        public static readonly UIToolkitBuilder Instance = new();

        public static VisualElement Build(Element element)
        {
            return Instance.BuildInternal(element);
        }
        
        #endregion
        
        
        private readonly Dictionary<Type, Func<Element, VisualElement>> _buildFuncTable;

        protected UIToolkitBuilder()
        {
            _buildFuncTable = new Dictionary<Type, Func<Element, VisualElement>>
            {
                [typeof(WindowElement)] = Build_Window,
                [typeof(WindowLauncherElement)] = Build_WindowLauncher,
                [typeof(RowElement)] = Build_Row,
                [typeof(ColumnElement)] = Build_Column,
                [typeof(BoxElement)] = Build_Box,
                [typeof(HelpBoxElement)] = Build_HelpBox,
                [typeof(ScrollViewElement)] = Build_ScrollView,
                [typeof(IndentElement)] = Build_Indent,
                [typeof(PageElement)] = Build_Column,
                [typeof(ListViewItemContainerElement)] = Build_ListViewItemContainer,
                
                [typeof(CompositeFieldElement)] = Build_CompositeField,
                [typeof(LabelElement)] = Build_Label,
                [typeof(IntFieldElement)] = Build_Field<int, IntegerField>,
                [typeof(UIntFieldElement)] = Build_Field<uint, UIntField>,
                [typeof(FloatFieldElement)] = Build_Field<float, FloatField>,
                [typeof(TextFieldElement)] = Build_TextField,
                [typeof(BoolFieldElement)] = Build_Field<bool, Toggle>,
                [typeof(ColorFieldElement)] = Build_ColorField,

                [typeof(DropdownElement)] = Build_Dropdown,
                [typeof(IntSliderElement)] = Build_Slider<int, ClampFreeSliderInt>,
                [typeof(FloatSliderElement)] = Build_Slider<float, ClampFreeSlider>,

                [typeof(IntMinMaxSliderElement)] = Build_MinMaxSlider_Int,
                [typeof(FloatMinMaxSliderElement)] = Build_MinMaxSlider_Float,
                [typeof(SpaceElement)] = Build_Space,
                [typeof(ImageElement)] = Build_Image,
                [typeof(ButtonElement)] = Build_Button,
                [typeof(FoldElement)] = Build_Fold,
                [typeof(DynamicElement)] = Build_DynamicElement,
                [typeof(PopupMenuElement)] = Build_PopupElement
            };
        }


        protected override IReadOnlyDictionary<Type, Func<Element, VisualElement>> BuildFuncTable => _buildFuncTable;


        protected override void CalcPrefixLabelWidthWithIndent(LabelElement label, VisualElement ve)
        {
            // ve.ScheduleToUseResolvedLayoutBeforeRendering(() => // こちらだとFold内のListで１つめの要素のインデントが崩れるのでGeometryChangedEventで常に呼んでおく
            ve.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.oldRect == evt.newRect) return;

                var marginLeft = ve.worldBound.xMin;
                
                for (var element = label.Parent;
                     element != null;
                     element = element.Parent)
                {
                    if (LayoutHint.IsIndentOrigin(element))
                    {
                        marginLeft -= GetUIObj(element).worldBound.xMin;
                        break;
                    }
                }

                marginLeft /= ve.worldTransform.lossyScale.x; // ignore rotation

                ve.style.minWidth = LayoutSettings.LabelWidth - marginLeft;
            });
        }

        protected override void OnElementEnableChanged(Element _, VisualElement ve, bool enable)
        {
            var display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            if (ve.resolvedStyle.display != display)
            {
                ve.style.display = display;
            }
        }

        protected override void OnElementInteractableChanged(Element _, VisualElement ve, bool interactable)
        {
            ve.SetEnabled(interactable);
        }


        protected override void OnElementStyleChanged(Element element, VisualElement ve, Style style)
        {
            var veStyle = ve.style;

            veStyle.width = ToStyleLength(style.Width);
            veStyle.height = ToStyleLength(style.Height);
            veStyle.minWidth = ToStyleLength(style.MinWidth);
            veStyle.minHeight = ToStyleLength(style.MinHeight);
            veStyle.maxWidth = ToStyleLength(style.MaxWidth);
            veStyle.maxHeight = ToStyleLength(style.MaxHeight);
            veStyle.color = ToStyleColor(style.Color);
            veStyle.backgroundColor = ToStyleColor(style.BackgroundColor);
   
            // width or height has value
            var isFixedSize = (veStyle.width.keyword == StyleKeyword.Undefined ||
                               veStyle.height.keyword == StyleKeyword.Undefined);
            if (isFixedSize)
            {
                 veStyle.flexGrow = 0;
                 veStyle.flexShrink = 1;
            }
            else
            {
                veStyle.flexGrow = StyleKeyword.Null;
                veStyle.flexShrink = StyleKeyword.Null;
            }
            
            static StyleLength ToStyleLength(float? nullable)
                => (nullable is { } value) ? value : StyleKeyword.Null;
            
            static StyleColor ToStyleColor(Color? nullable)
                => (nullable is { } value) ? value : StyleKeyword.Null;
        }



        protected override void OnRebuildElementGroupChildren(ElementGroup elementGroup)
        {
            var groupVe = GetUIObj(elementGroup);
            Build_ElementGroupContents(groupVe, elementGroup);
        }

        protected override void OnDestroyElement(Element element, bool isDestroyRoot)
        {
            var ve = GetUIObj(element);
            
            if (isDestroyRoot)
            {
                ve?.RemoveFromHierarchy();
            }

            UnregisterUIObj(element);
        }

        private static class UssClassName
        {
            public static readonly string UnityBaseField = "unity-base-field";
            public static readonly string UnityBaseFieldLabel = UnityBaseField + "__label";

            private static readonly string RosettaUI = "rosettaui";
            
            public static readonly string CompositeField = RosettaUI + "-composite-field";
            public static readonly string CompositeFieldContents = CompositeField + "__contents";
            
            public static readonly string Column = RosettaUI + "-column";
            public static readonly string Row = RosettaUI + "-row";

            public static readonly string WindowLauncher = RosettaUI + "-window-launcher";

            public static readonly string MinMaxSlider = RosettaUI + "-min-max-slider";
           
            public static readonly string Space = RosettaUI + "-space";
            
            public static readonly string DynamicElement = RosettaUI + "-dynamic-element";
        }
    }
}