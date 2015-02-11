using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.SpecializedProperties;
using EPiServer.Web;

namespace MarijasPlayground.ExtendedLink
{
    public class ExtendedEPiLinkModel
    {
        private readonly IContentRepository _contentRepository;
        private readonly UIDescriptorRegistry _uiDescriptors;
        private readonly string _typeIdentifier;

        [Display(
            Name = "/episerver/cms/widget/editlink/linkname",
            Order = 10)]
        [Required]
        public string Text { get; set; }

        [Display(
            Name = "/episerver/cms/widget/editlink/linktitle",
            Order = 20)]
        public string Title { get; set; }

        [Display(
            Name = "/contenttypes/icontentdata/properties/pagetargetframe/caption",
            Order = 30)]
        [UIHint("TargetFrame")]
        public int? Target { get; set; }

        [Required]
        [Display(Order = 40)]
        [UIHint("HyperLink")]
        public string Href { get; set; }

        [Display(
            Name = "/contenttypes/icontentdata/properties/anchoronpage/caption",
            Order = 50)]
        [UIHint(ExtendedAnchorUiHints.AnchorsOnPage)]
        public string AnchorOnPage { get; set; }

        [ScaffoldColumn(false)]
        public string PublicUrl { get; set; }

        [ScaffoldColumn(false)]
        public string TypeIdentifier { get; set; }

        [ScaffoldColumn(false)]
        public Dictionary<string, string> Attributes { get; set; }

        public ExtendedEPiLinkModel()
            : this(ServiceLocator.Current.GetInstance<IContentRepository>(), ServiceLocator.Current.GetInstance<UIDescriptorRegistry>())
        {
        }

        public ExtendedEPiLinkModel(IContentRepository contentRepository, UIDescriptorRegistry uiDescriptors)
        {
            _contentRepository = contentRepository;
            _uiDescriptors = uiDescriptors;
            _typeIdentifier = _uiDescriptors.GetTypeIdentifiers(typeof(LinkItem)).FirstOrDefault();
        }

        public object ToClientModel(object serverModel)
        {
            var linkItemServerModel = (LinkItem)serverModel;
            var frame = Frame.Load(linkItemServerModel.Target);
            var nullable = frame != (Frame)null ? frame.ID : new int?();
            var href = linkItemServerModel.Href;
            var hrefWithoutHash = href;
            var anchorOnPage = "";
            var indexOfHash = href.IndexOf('#');
            if (indexOfHash > 0)
            {
                hrefWithoutHash = href.Substring(0, indexOfHash - 1);
                anchorOnPage = href.Substring(indexOfHash + 1);
            }

            var clientModel = new ExtendedEPiLinkModel
            {
                Text = linkItemServerModel.Text,
                Title = linkItemServerModel.Title,
                Href = hrefWithoutHash,
                AnchorOnPage = anchorOnPage,
                Target = nullable,
                TypeIdentifier = _typeIdentifier,
                Attributes = linkItemServerModel.Attributes
            };
            ModifyIContentProperties(linkItemServerModel, clientModel);
            return clientModel;
        }

        public object ToServerModel(object clientModel)
        {
            var linkModel = (ExtendedEPiLinkModel)clientModel;
            var linkItem = new LinkItem();
            if (linkModel.Attributes != null)
            {
                foreach (var keyValuePair in linkModel.Attributes)
                {
                    linkItem.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            linkItem.Text = linkModel.Text;
            linkItem.Title = linkModel.Title;
            linkItem.Href = !string.IsNullOrEmpty(linkModel.AnchorOnPage) ?
                string.Format("{0}#{1}", linkModel.Href, linkModel.AnchorOnPage) :
                linkModel.Href;
            linkItem.Target = linkModel.Target.HasValue ? Frame.Load(linkModel.Target.Value).Name : null;
            return linkItem;
        }

        private void ModifyIContentProperties(LinkItem serverModel, ExtendedEPiLinkModel clientModel)
        {
            string mappedHref = serverModel.GetMappedHref();
            if (string.IsNullOrEmpty(mappedHref))
                return;
            var hrefWithoutHash = mappedHref;
            var anchorOnPage = "";
            var indexOfHash = mappedHref.IndexOf('#');
            if (indexOfHash > 0)
            {
                hrefWithoutHash = mappedHref.Substring(0, indexOfHash - 1);
                anchorOnPage = mappedHref.Substring(indexOfHash + 1);
            }

            clientModel.Href = hrefWithoutHash;
            clientModel.AnchorOnPage = anchorOnPage;
            var contentReference = PermanentLinkUtility.GetContentReference(new UrlBuilder(hrefWithoutHash));
            IContent content;
            if (!(contentReference != ContentReference.EmptyReference) || !_contentRepository.TryGet(contentReference, out content))
                return;
            clientModel.TypeIdentifier = _uiDescriptors.GetTypeIdentifiers(content.GetType()).FirstOrDefault();

            var absoluteUriBySettings = UriSupport.AbsoluteUrlBySettings(EPiServer.Cms.Shell.IContentExtensions.PublicUrl(content));
            clientModel.PublicUrl = indexOfHash > 0 ?
                string.Format("{0}#{1}", absoluteUriBySettings, anchorOnPage) :
                absoluteUriBySettings;
        }
    }
}