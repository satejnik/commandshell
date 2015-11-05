//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CommandShell.Infrastucture
{
    public sealed class AssemblyInfo
    {
        #region Constructor

        public AssemblyInfo(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            Assembly = assembly;
        }

        #endregion

        #region Properties

        public static AssemblyInfo Current
        {
            get
            {
                return new AssemblyInfo(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());
            }
        }

        public Assembly Assembly { get; private set; }

        #region Identity

        /// <summary>
        /// Gets the supported culture of the attributed assembly.
        /// </summary>
        /// <value>
        /// A string containing the name of the supported culture.
        /// </value>
        public CultureInfo Culture
        {
            get { return Assembly.GetName().CultureInfo; }
        }

        /// <summary>
        /// Gets an integer value representing the combination of <see cref="AssemblyNameFlags"/> flags specified when this attribute instance was created.
        /// </summary>
        /// <value>
        /// An integer value representing a bitwise combination of <see cref="AssemblyNameFlags"/> flags.
        /// </value>
        public AssemblyNameFlags Flags
        {
            get { return Assembly.GetName().Flags; }
        }

        /// <summary>
        /// Gets the version number of the attributed assembly.
        /// </summary>
        /// <value>
        /// A string containing the assembly version number.
        /// </value>
        public Version Version
        {
            get { return Assembly.GetName().Version; }
        }

        #endregion

        #region Information

        /// <summary>
        /// Gets company name information.
        /// </summary>
        /// <value>
        /// A string containing the company name.
        /// </value>
        public string Company
        {
            get
            {
                AssemblyCompanyAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Company : string.Empty;
            }
        }

        /// <summary>
        /// Gets copyright information.
        /// </summary>
        /// <value>
        /// A string containing the copyright information.
        /// </value>
        public string Copyright
        {
            get
            {
                AssemblyCopyrightAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Copyright : string.Empty;
            }
        }

        /// <summary>
        /// Gets the Win32 file version resource name.
        /// </summary>
        /// <value>
        /// A string containing the file version resource name.
        /// </value>
        public Version FileVersion
        {
            get
            {
                AssemblyFileVersionAttribute attribute;
                return TryGetAttribute(out attribute) ? new Version(attribute.Version) : new Version();
            }
        }

        /// <summary>
        /// Gets version information.
        /// </summary>
        /// <value>
        /// A string containing the version information.
        /// </value>
        public Version InformationalVersion
        {
            get
            {
                AssemblyInformationalVersionAttribute attribute;
                return TryGetAttribute(out attribute) ? new Version(attribute.InformationalVersion) : null;
            }
        }

        /// <summary>
        /// Gets product name information.
        /// </summary>
        /// <value>
        /// A string containing the product name.
        /// </value>
        public string Product
        {
            get
            {
                AssemblyProductAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Product : string.Empty;
            }
        }

        /// <summary>
        /// Gets trademark information.
        /// </summary>
        /// <value>
        /// A string containing trademark information.
        /// </value>
        public string Trademark
        {
            get
            {
                AssemblyTrademarkAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Trademark : string.Empty;
            }
        }

        #endregion

        #region Manifest

        /// <summary>
        /// Gets assembly configuration information.
        /// </summary>
        /// <value>
        /// A string containing the assembly configuration information.
        /// </value>
        public string Configuration
        {
            get
            {
                AssemblyConfigurationAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Configuration : string.Empty;
            }
        }

        /// <summary>
        /// Gets default alias information.
        /// </summary>
        /// <value>
        /// A string containing the default alias information.
        /// </value>
        public string DefaultAlias
        {
            get
            {
                AssemblyDefaultAliasAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.DefaultAlias : string.Empty;
            }
        }

        /// <summary>
        /// Gets assembly description information.
        /// </summary>
        /// <value>
        /// A string containing the assembly description.
        /// </value>
        public string Description
        {
            get
            {
                AssemblyDescriptionAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Description : string.Empty;
            }
        }

        /// <summary>
        /// Gets assembly title information.
        /// </summary>
        /// <value>
        /// The assembly title. 
        /// </value>
        public string Title
        {
            get
            {
                AssemblyTitleAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Title : string.Empty;
            }
        }

        #endregion

        #region Strong Name

        /// <summary>
        /// Gets a value indicating that delay signing is being used. </summary>
        /// <value>
        /// true if this assembly has been built as delay-signed; otherwise, false.
        /// </value>
        public bool DelaySign
        {
            get
            {
                AssemblyDelaySignAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.DelaySign : default(bool);
            }
        }

        /// <summary> 
        /// Gets the name of the file containing the key pair used to generate a strong name for the attributed assembly.
        /// </summary>
        /// <value>
        /// A string containing the name of the file that contains the key pair.
        /// </value>
        public string KeyFile
        {
            get
            {
                AssemblyKeyFileAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.KeyFile : string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the container having the key pair that is used to generate a strong name for the attributed assembly.
        /// </summary>
        /// <value>
        /// A string containing the name of the container that has the relevant key pair.
        /// </value>
        public string KeyName
        {
            get
            {
                AssemblyKeyNameAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.KeyName : string.Empty;
            }
        }

        #endregion

        #region Others

        /// <summary>
        /// Gets the frienly name of the assembly.
        /// </summary>
        /// <value>
        /// A string containing the friendly name of the given assembly.
        /// </value>
        public string FriendlyName
        {
            get { return Assembly.GetName().Name; }
        }

        /// <summary>
        /// Gets the unique GUID that identifies the assembly.
        /// </summary>
        /// <value>
        /// Unique GUID that identifies the assembly.
        /// </value>
        public Guid Guid
        {
            get
            {
                GuidAttribute attribute;
                return TryGetAttribute(out attribute) ? new Guid(attribute.Value) : Guid.Empty;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the COM type is visible.
        /// </summary>
        /// <value>
        /// true if the type is visible; otherwise, false. The default value is true.
        /// </value>
        public bool ComVisible
        {
            get
            {
                ComVisibleAttribute attribute;
                return TryGetAttribute(out attribute) ? attribute.Value : default(bool);
            }
        }

        #endregion

        #endregion

        #region Helpers

        private TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            return (TAttribute)Assembly.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
        }

        private bool TryGetAttribute<TAttribute>(out TAttribute attribute) where TAttribute : Attribute
        {
            attribute = GetAttribute<TAttribute>();
            return attribute != null;
        }

        #endregion
    }
}
