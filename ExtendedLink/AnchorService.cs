using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace MarijasPlayground.ExtendedLink
{
    public static class AnchorService
    {
        public static List<AnchorHeadingPair> GetOrSetFromPageIdForTinyMce(string pageIdWithVersion)
        {
            if (string.IsNullOrEmpty(pageIdWithVersion)) return null;

            var pageIdWithoutVersion = pageIdWithVersion;
            var pageId = 0;
            var indexOfUnderscore = pageIdWithVersion.IndexOf('_');
            if (indexOfUnderscore > 0)
            {
                pageIdWithoutVersion = pageIdWithVersion.Substring(0, indexOfUnderscore);
            }

            int.TryParse(pageIdWithoutVersion, out pageId);

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var anchorContent = contentLoader.Get<IContent>(new ContentReference(pageId));
            var localizable = anchorContent as ILocalizable;
            return GetAnchorHeadingPairs(anchorContent, true);
        }

        public static List<AnchorHeadingPair> GetOrSetFromUrlForLinkBlock(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var page = urlResolver.Route(new UrlBuilder(url), ContextMode.Edit) as PageData;

            if (page != null)
            {
                return GetAnchorHeadingPairs(page, true);
            }
            return null;
        }

        private static List<AnchorHeadingPair> GetAnchorHeadingPairs(IContent anchorContentContainer, bool skipTopAnchor = false)
        {
            var anchorHeadingPairList = new List<AnchorHeadingPair>();

            if (anchorContentContainer != null)
            {
                var heading = anchorContentContainer.Property["Heading"] != null ? anchorContentContainer.Property["Heading"].Value.ToString() : "";
                if (!string.IsNullOrEmpty(heading))
                {
                    var anchorLink = GetAnchorString(heading, anchorContentContainer.ContentLink.ID);
                    if (anchorLink != null && !skipTopAnchor)
                    {
                        var topHeadingAnchor = new AnchorHeadingPair
                        {
                            Heading = heading,
                            Anchor = anchorLink,
                        };
                        anchorHeadingPairList.Add(topHeadingAnchor);
                    }
                }

                var contentAreas = anchorContentContainer.Property.Where(p => p != null && p.Value is ContentArea).Select(p => p.Value as ContentArea).ToArray();
                var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

                foreach (var contentArea in contentAreas)
                {
                    if (contentArea.FilteredItems != null && contentArea.FilteredItems.Any())
                    {
                        foreach (var contentAreaItem in contentArea.FilteredItems)
                        {
                            var block = contentLoader.Get<IContent>(contentAreaItem.ContentLink);
                            if (block != null)
                            {
                                var anchorsFromContentArea = GetAnchorHeadingPairs(block);
                                anchorHeadingPairList.AddRange(anchorsFromContentArea);
                            }
                        }
                    }

                }

            }

            return anchorHeadingPairList;
        }

        public static string GetAnchorString(string s, int contentId)
        {
            if (string.IsNullOrEmpty(s)) return null;
            var prevChar = char.MinValue;
            var alphaNumericString = new StringBuilder();
            foreach (var c in s)
            {
                var newChar = char.IsLetterOrDigit(c) ? char.ToLower(c) : '_';
                if (newChar != '_' || newChar != prevChar)
                {
                    if (SwedishReplacingChars.ContainsKey(newChar))
                    {
                        alphaNumericString.Append(SwedishReplacingChars[newChar]);
                    }
                    else
                    {
                        alphaNumericString.Append(newChar);
                        prevChar = newChar;
                    }
                }
            }
            alphaNumericString.Append('_');
            alphaNumericString.Append(contentId);

            return alphaNumericString.ToString();
        }

        private static readonly Dictionary<char, string> SwedishReplacingChars = new Dictionary<char, string>
        {
           {'Å', "Aa"}, {'Ä', "Ae"}, {'Ö', "Oe"}, {'å', "aa"}, {'ä', "ae"}, {'ö', "oe"}
        };
    }
}