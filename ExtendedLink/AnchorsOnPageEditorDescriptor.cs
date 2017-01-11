using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors.SelectionFactories;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace MarijasPlayground.ExtendedLink
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = ExtendedAnchorUiHints.AnchorsOnPage)]
    public class AnchorsOnPageEditorDescriptor : EditorDescriptor
    {
        public AnchorsOnPageEditorDescriptor()
        {
            SelectionFactoryType = typeof(PropertySettingsSelectionFactory);
            ClientEditingClass = "epi-cms/contentediting/editors/SelectionEditor";
            EditorConfiguration.Add("style", "width: 240px");
        }
    }
}