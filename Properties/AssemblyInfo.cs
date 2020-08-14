using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rhino.PlugIns;


// Plug-in Description Attributes - all of these are optional
// These will show in Rhino's option dialog, in the tab Plug-ins
[assembly: PlugInDescription(DescriptionType.Address, "51 Holloway Drive Bayswater")]
[assembly: PlugInDescription(DescriptionType.Country, "Australia")]
[assembly: PlugInDescription(DescriptionType.Email, "toby@metrixgroup.com.au")]
[assembly: PlugInDescription(DescriptionType.Phone, "1300 792 493")]
[assembly: PlugInDescription(DescriptionType.Fax, "1300 792 893")]
[assembly: PlugInDescription(DescriptionType.Organization, "Metrix Group Pty. Ltd.")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "-")]
[assembly: PlugInDescription(DescriptionType.WebSite, "http://www.metrixgroup.com.au")]


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MetrixGroupPlugins")] // Plug-In title is extracted from this
[assembly: AssemblyDescription("Metrix Group Utilities to do perforation")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Metrix Group Pty. Ltd.")]
[assembly: AssemblyProduct("MetrixGroupPlugins")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d6d7b139-42ef-4915-818d-4e7fdd5e5f17")] // This will also be the Guid of the Rhino plug-in

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
