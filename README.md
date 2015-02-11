This is a TinyMCE button for EPiServer 7.19.2. It is basically the same button you get when you use standard epilink, but expanded to support linking to a specific place on the page instead of top of it.

Info on how to use it and motivation behind, can be found here:
http://www.mogul.com/en/About-Mogul/Blog/Hooking-to-a-page-dojo-widget-inside-EPiServer-TinyMCE-link/
http://www.mogul.com/en/About-Mogul/Blog/Extending-EPiServer-link-in-TinyMCE-to-support-anchors-on-page/

When using these files, they will work when copied in the same structure, but ofc, you can refactor the code according to your design. Scripts and styles should stay in the same place. 

This is the part of packages.config with the EPiServer versions this plugin is tested on:
  <package id="EPiServer.CMS" version="7.16.1" targetFramework="net452" />
  <package id="EPiServer.CMS.Core" version="7.19.2" targetFramework="net452" />
  <package id="EPiServer.CMS.UI" version="7.19.1" targetFramework="net452" />
  <package id="EPiServer.CMS.UI.Core" version="7.19.1" targetFramework="net452" />
  <package id="EPiServer.Framework" version="7.19.2" targetFramework="net452" />
  <package id="EPiServer.Logging.Log4Net" version="0.1.0" targetFramework="net452" />
  <package id="EPiServer.Packaging" version="3.2.0" targetFramework="net452" />
  <package id="EPiServer.Packaging.UI" version="3.2.0" targetFramework="net452" />
  <package id="EPiServer.Search" version="7.7.0" targetFramework="net452" />