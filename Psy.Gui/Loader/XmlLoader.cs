using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Psy.Core;
using Psy.Core.FileSystem;
using Psy.Gui.Components;
using SlimMath;

namespace Psy.Gui.Loader
{
    internal delegate Widget InternalWidgetFactory(XmlElement xmlElement, Widget parent);
    public delegate Widget WidgetFactory(GuiManager guiManager, XmlElement xmlElement, Widget parent);

    public class XmlLoader
    {
        private readonly GuiManager _guiManager;
        private Dictionary<string, InternalWidgetFactory> _factories;
        public string LastFilename { get; private set; }

        public XmlLoader(GuiManager guiManager)
        {
            _guiManager = guiManager;
            InitializeFactories();
        }

        public void RegisterCustomWidget(string name, WidgetFactory factory)
        {
            _factories[name.ToLower()] =
                (element, parent) =>
                {
                    var widget = factory(_guiManager, element, parent);
                    ApplyStandardWidgetAttributes(widget, element);
                    return widget;
                };
        }

        private void InitializeFactories()
        {
            _factories = new Dictionary<string, InternalWidgetFactory> 
            {
                {"button", ReadButton},    
                {"desktop", ReadWidgets},
                {"image", ReadImage},
                {"label", ReadLabel},
                {"listbox", ReadListbox},
                {"multilinelabel", ReadMultilineLabel},
                {"slider", ReadSlider},
                {"tabbedpanel", ReadTabbedPanel},
                {"textbox", ReadTextbox},
                {"togglebutton", ReadToggleButton},
                {"window", ReadWindow},
                {"textlist", ReadTextList},
            };

            Timeline.Register(this);
            DropdownList.Register(this);
            Checkbox.Register(this);
        }

        private static void ValidateDocument(XmlDocument document)
        {
            if (document.DocumentElement == null)
            {
                throw new GuiException("DocumentElement is null. Something has gone horribly wrong.");   
            }
            if (document.DocumentElement.Name != "gui")
            {
                throw new GuiException("Root element must be <gui>");
            }
            if (document.DocumentElement.GetString("version") != "1")
            {
                throw new GuiException("Version number must be `1`");
            }
        }

        public XmlElement GetDocumentRoot(string xmlFilename)
        {
            var document = new XmlDocument();
            document.Load(Lookup.GetAssetPath(xmlFilename));
            var documentRoot = document.DocumentElement;

            ValidateDocument(document);

            return documentRoot;
        }

        public Widget Load(string xmlFilename, Widget parent)
        {
            LastFilename = xmlFilename;

            var documentRoot = GetDocumentRoot(xmlFilename);

            return ReadWidget(documentRoot, parent);
        }

        private Widget ReadWidget(XmlElement element, Widget parent)
        {
            foreach (var guiElement in element)
            {
                if (!(guiElement is XmlElement))
                    continue;

                return Read((XmlElement)guiElement, parent);
            }

            return null;
        }

        private Widget ReadWidgets(XmlElement element, Widget parent)
        {
            foreach (var guiElement in element)
            {
                if (!(guiElement is XmlElement))
                    continue;

                Read((XmlElement) guiElement, parent);
            }

            return parent;
        }

        private static void ApplyStandardWidgetAttributes(Widget widget, XmlElement xmlElement)
        {
            widget.Anchor = xmlElement.ReadAnchorAttribute();
            widget.AutoSize = xmlElement.ReadAutosizeAttribute();
            widget.Enabled = xmlElement.ReadBooleanAttribute("enabled");
            widget.Visible = xmlElement.ReadBooleanAttribute("visible");
            widget.Position = xmlElement.ReadVector("position");
            
            if (widget.Size.IsZeroLength())
            {
                widget.Size = xmlElement.ReadVector("size");    
            }
            
            widget.Anchor = xmlElement.ReadAnchorAttribute();
            widget.Margin = xmlElement.ReadVector("margin");
            widget.Class = xmlElement.GetString("class");
            widget.Transparent = xmlElement.ReadBooleanAttribute("transparent", false);
            widget.ClickThrough = xmlElement.ReadBooleanAttribute("clickthrough", false);
            widget.Padding = xmlElement.ReadPadding("padding");
            widget.Alpha = xmlElement.ReadFloat("alpha", 1.0f);
            widget.TooltipText = xmlElement.GetString("tooltipText");

            Debug.Assert(widget.Alpha > 0.0f);

            var uniqueName = xmlElement.GetString("name");
            if (uniqueName != "")
            {
                widget.UniqueName = uniqueName;
            }

            widget.Draggable = xmlElement.ReadBooleanAttribute("draggable", false);
        }

        private Widget Read(XmlElement xmlElement, Widget parent)
        {
            var elementName = xmlElement.Name;

            if (!_factories.ContainsKey(elementName.ToLower()))
            {
                throw new Exception(string.Format("XmlElement with name `{0}` does not have a factory registered to construct it", elementName));
            }

            return _factories[elementName.ToLower()](xmlElement, parent);
        }

        private Widget ReadSlider(XmlElement xmlElement, Widget parent)
        {
            var widget = new Slider(_guiManager, parent)
            {
                MinimumValue = xmlElement.ReadInteger("minValue", 0),
                MaximumValue = xmlElement.ReadInteger("maxValue", 100),
                Value = xmlElement.ReadInteger("value", 25)
            };

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadImage(XmlElement xmlElement, Widget parent)
        {
            var widget = new Image(_guiManager, parent)
            {
                ImageName = xmlElement.GetString("imageName"),
                Border = xmlElement.ReadBooleanAttribute("border", false)
            };

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadTextList(XmlElement xmlElement, Widget parent)
        {
            var widget = new TextList(_guiManager, parent)
            {
                Direction = xmlElement.ReadDirection(),
                ResizeParent = xmlElement.ReadBooleanAttribute("resizeparent", false),
                MaxLength = xmlElement.ReadInteger("maxlength", 0)
            };

            foreach (XmlLinkedNode node in xmlElement)
            {
                var childNode = node as XmlElement;
                if (childNode == null)
                    continue;

                switch (childNode.Name.ToLower())
                {
                    case "rows":

                        foreach (XmlLinkedNode childChildNode in childNode)
                        {
                            var row = childChildNode as XmlElement;
                            if (row == null)
                                continue;

                            var colourString = row.GetString("colour");
                            var text = row.InnerText;

                            var fontFace = row.GetString("fontface");
                            var fontSize = row.ReadInteger("fontsize", 0);
                            var italic = row.ReadBooleanAttribute("italic", false);

                            var netColour = System.Drawing.ColorTranslator.FromHtml(colourString);

                            widget.AddLine(text, new Color4(netColour.A, netColour.R, netColour.G, netColour.B), fontFace, fontSize, italic);
                        }
                        break;
                }
            }

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadWindow(XmlElement xmlElement, Widget parent)
        {
            var widget = new GuiWindow(_guiManager, parent)
            {
                RenderBackground = xmlElement.ReadBooleanAttribute("renderBackground")
            };

            ReadWidgets(xmlElement, widget);
            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadTextbox(XmlElement xmlElement, Widget parent)
        {
            var widget = new Textbox(_guiManager, parent)
            {
                Value = xmlElement.ReadChildDataElement("text"),
                Background = xmlElement.ReadBooleanAttribute("background"),
                Border = xmlElement.ReadBooleanAttribute("border")
            };

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadMultilineLabel(XmlElement xmlElement, Widget parent)
        {
            var widget = new MultilineLabel(_guiManager, parent)
            {
                Value = xmlElement.ReadChildDataElement("text")
            };

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadListbox(XmlElement xmlElement, Widget parent)
        {
            var widget = new Listbox(_guiManager, parent);
            ApplyStandardWidgetAttributes(widget, xmlElement);

            foreach (XmlLinkedNode node in xmlElement)
            {
                var childNode = node as XmlElement;
                if (childNode == null)
                    continue;

                // read columns
                switch (childNode.Name.ToLower())
                {
                    case "columns":
                        foreach (XmlElement column in childNode)
                        {
                            var columnName = column.GetString("name");
                            var columnWidth = column.GetString("width");

                            widget.AddColumn(columnName, columnWidth == "auto" ? 0 : int.Parse(columnWidth));
                        }
                        break;
                    case "items":
                    {
                        var rowData = new List<List<RowData>>();

                        foreach (XmlElement rawItem in childNode)
                        {
                            var columnData = new List<RowData>();

                            foreach (XmlElement rawItemData in rawItem)
                            {
                                var name = rawItemData.GetString("name");
                                var data = rawItemData.InnerText;

                                columnData.Add(
                                    new RowData
                                    {
                                        Header = name,
                                        Data = data
                                    });
                            }

                            rowData.Add(columnData);
                        }

                        widget.Populate(rowData);
                    }
                        break;
                    default:
                        throw new GuiException(string.Format("Unknown listbox child element `{0}`", childNode.Name));
                }
            }
            return widget;
        }

        private Widget ReadLabel(XmlElement xmlElement, Widget parent)
        {
            var widget = new Label(_guiManager, parent)
            {
                Text = xmlElement.ReadChildDataElement("text"),
                FontFace = xmlElement.GetString("fontface"),
                FontSize = xmlElement.ReadInteger("fontsize", 16),
                Colour = xmlElement.GetString("colour")
            };

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadToggleButton(XmlElement xmlElement, Widget parent)
        {
            var widget = new ToggleButton(_guiManager, parent)
            {
                Label = xmlElement.ReadChildDataElement("label"),
                ImageName = xmlElement.GetString("imageName"),
                GroupName = xmlElement.GetString("groupName"),
                Value = xmlElement.ReadBooleanAttribute("toggled")
            };

            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadButton(XmlElement xmlElement, Widget parent)
        {
            var buttonText = xmlElement.ReadChildDataElement("label");

            var widget = 
                new Button(_guiManager, parent)
                {
                    Label = buttonText,
                    ImageName = xmlElement.GetString("imageName")
                };
            ApplyStandardWidgetAttributes(widget, xmlElement);
            return widget;
        }

        private Widget ReadTabbedPanel(XmlElement xmlElement, Widget parent)
        {
            var widget = new TabbedPanel(_guiManager, parent);
            ApplyStandardWidgetAttributes(widget, xmlElement);

            foreach (XmlElement xmlElementTab in xmlElement)
            {
                var tabLabel = xmlElementTab.GetString("label");

                if (string.IsNullOrEmpty(tabLabel))
                {
                    throw new GuiException("Tab label must be specified");
                }

                var tab = widget.AddTab(tabLabel);
                ReadWidgets(xmlElementTab, tab);
            }
            return widget;
        }

        public void Reload(Widget parent = null)
        {
            if (!string.IsNullOrEmpty(LastFilename))
            {
                Load(LastFilename, parent);
            }
        }
    }
}