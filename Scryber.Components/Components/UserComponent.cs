﻿/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using Scryber.Styles;
using Scryber.PDF;

namespace Scryber.Components
{
    [PDFParsableComponent("UserComponent")]
    [PDFRemoteParsableComponent("Component-Ref")]
    public class UserComponent : Panel, IRemoteComponent, IControlledComponent, INamingContainer
    {

        #region public Scryber.Data.PDFXmlNamespaceCollection NamespaceDeclarations

        private Scryber.Data.PDFXmlNamespaceCollection _namespaces;

        public Scryber.Data.PDFXmlNamespaceCollection NamespaceDeclarations
        {
            get
            {
                return _namespaces;
            }
        }

        
        /// <summary>
        /// Returns true if this instance has namespaces declared on it.
        /// </summary>
        public bool HasNamespaceDeclarations
        {
            get { return null != _namespaces && _namespaces.Count > 0; }
        }

        #endregion

        #region public PDFItemCollection Items {get;}

        private ItemCollection _items = null;

        /// <summary>
        /// Gets a page centered collection of objects that can be accessed by name or index. 
        /// The values will only be applied to this page and it's contents, rather than across the whole document
        /// </summary>
        [PDFElement("Params")]
        [PDFArray(typeof(IKeyValueProvider))]
        public ItemCollection Params
        {
            get
            {
                if (null == _items)
                    _items = new ItemCollection(this);
                return _items;
            }
        }

        /// <summary>
        /// Returns true if this page has one or more specific stored items. Otherwise false
        /// </summary>
        public bool HasParams
        {
            get { return null != this._items && _items.Count > 0; }
        }

        #endregion

        #region public Object Controller {get;set;}

        /// <summary>
        /// Gets or sets the controller object for this component. 
        /// Normally set by the XMLParser when it encounters the controller option on the scryber processing instructions.
        /// </summary>
        public Object Controller
        {
            get;
            set;
        }

        #endregion

        // INamingContianer interface

        IComponent INamingContainer.Owner
        {
            get { return this.Parent; }
            set { this.Parent = (Component)value; }
        }

        //
        // .ctor(s)
        //

        #region public PDFUserComponent()

        public UserComponent()
            : this(ObjectTypes.UserComponent)
        {
        }

        #endregion

        #region protected PDFUserComponent(PDFObjectType type)

        protected UserComponent(ObjectType type)
            : base(type)
        {
        }

        #endregion

        #region Scryber.IPDFRemoteComponent members

        void Scryber.IRemoteComponent.RegisterNamespaceDeclaration(string prefix, string ns)
        {
            Scryber.Data.XmlNamespaceDeclaration dec = new Data.XmlNamespaceDeclaration(){
                 NamespaceURI = ns,
                 Prefix = prefix
            };
            if (null == _namespaces)
                _namespaces = new Data.PDFXmlNamespaceCollection();
            this._namespaces.Add(dec);
        }

        IDictionary<string, string> Scryber.IRemoteComponent.GetDeclaredNamespaces()
        {
            Dictionary<string, string> all = new Dictionary<string, string>();
            foreach (Scryber.Data.XmlNamespaceDeclaration dec in this.NamespaceDeclarations)
            {
                all.Add(dec.Prefix, dec.NamespaceURI);
            }
            return all;
        }

        #endregion

        // data bind and layout overrides for updating the item collection on this page to ensure it flows through to other items

        private ItemCollection _orgitems;

        protected override void OnDataBinding(DataContext context)
        {
            _orgitems = context.Items;

            if (this.HasParams)
            {
                ItemCollection updated = _orgitems.Clone();
                updated.Merge(this.Params);
                context.Items = updated;

            }

            base.OnDataBinding(context);
        }

        protected override void OnDataBound(DataContext context)
        {
            base.OnDataBound(context);

            if (null != _orgitems)
                context.Items = _orgitems;
        }


        protected override void DoRegisterArtefacts(PDFLayoutContext context, PDFArtefactRegistrationSet set, Style fullstyle)
        {
            _orgitems = context.Items;

            if (this.HasParams)
            {
                ItemCollection updated = _orgitems.Clone();
                updated.Merge(this.Params);
                context.Items = updated;
            }

            base.DoRegisterArtefacts(context, set, fullstyle);
        }

        protected override void DoCloseLayoutArtefacts(PDFLayoutContext context, PDFArtefactRegistrationSet artefacts, Style fullstyle)
        {
            base.DoCloseLayoutArtefacts(context, artefacts, fullstyle);

            context.Items = _orgitems;
        }
        
    }
}
