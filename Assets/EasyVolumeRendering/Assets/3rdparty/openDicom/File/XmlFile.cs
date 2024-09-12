/*
   
    openDICOM.NET openDICOM# 0.1.1

    openDICOM# provides a library for DICOM related development on Mono.
    Copyright (C) 2006-2007  Albert Gnandt

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA


    $Id: XmlFile.cs 48 2007-03-28 13:49:15Z agnandt $
*/
using System;
using System.IO;
using System.Xml;
using openDicom;
using openDicom.Registry;
using openDicom.DataStructure;
using openDicom.DataStructure.DataSet;
using openDicom.Encoding;

using DateTime = System.DateTime;


namespace openDicom.File
{

    /// <summary>
    ///     This class represents a DICOM- or ACR-NEMA-XML file.
    /// </summary>
    /// <remarks>
    ///     The representation of DICOM content as XML is not standardized.
    /// </remarks>
    public class XmlFile
    {
        private static readonly string fileComment =
            "This file was automatically generated by openDICOM#.";

        private AcrNemaFile acrNemaFile = null;
        /// <summary>
        ///     ACR-NEMA or DICOM file content provider.
        /// </summary>
        public AcrNemaFile AcrNemaFile
        {
            get 
            { 
                if (acrNemaFile != null)
                    return acrNemaFile;
                else
                    throw new DicomException("ACR-NEMA file is null.",
                        "XmlFile.AcrNemaFile");
            }
        }

        /// <summary>
        ///     Determines, if the used ACR-NEMA file in fact is a DICOM file.
        /// </summary>
        public bool IsDicomFile
        {
            get { return (AcrNemaFile is DicomFile); }
        }

        private bool isPixelDataExcluded = false;
        /// <summary>
        ///     Switch for excluding pixel data from writing to XML file.
        /// </summary>
        /// <remarks>
        ///     If pixel data is supposed to be excluded, the entire DICOM
        ///     group 7FE0 will be excluded from mapping to XML.
        /// </remarks>
        public bool IsPixelDataExcluded
        {
            set { isPixelDataExcluded = value; }
            get { return isPixelDataExcluded; }
        }


        /// <summary>
        ///     Creates a XML file instance from the given ACR-NEMA or
        ///     DICOM file content provider.
        /// </summary>
        public XmlFile(AcrNemaFile acrNemaFile)
        {
            this.acrNemaFile = acrNemaFile;
        }

        /// <summary>
        ///     Creates a XML file instance from the given ACR-NEMA or
        ///     DICOM file content provider in consideration of 
        ///     pixel data exclusion.
        /// </summary>
        /// <remarks>
        ///     If pixel data is supposed to be excluded, the entire DICOM
        ///     group 7FE0 will be excluded from mapping to XML.
        /// </remarks>
        public XmlFile(AcrNemaFile acrNemaFile, bool excludePixelData):
            this(acrNemaFile)
        {
            IsPixelDataExcluded = excludePixelData;
        }

        /// <summary>
        ///     Determines whether a file with specified file name is an
        ///     ACR-NEMA- or DICOM-XML file.
        /// </summary>
        public static bool IsXmlFile(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, 
                FileAccess.Read);
            try
            {
                XmlTextReader xml = new XmlTextReader(fileStream);
                xml.MoveToContent();
                return xml.IsStartElement("AcrNemaFile") || 
                    xml.IsStartElement("DicomFile");
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                fileStream.Close();
            }
        }

        /// <summary>
        ///     Saves <see cref="AcrNemaFile" /> as XML file with the
        ///     specified file name.
        /// </summary>
        public void SaveTo(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create, 
                FileAccess.Write);
            try
            {
                SaveTo(fileStream);
            }
            finally
            {
                fileStream.Close();
            }
        }

        private void AddBase64ValueToXml(XmlTextWriter xml, byte[] bytes)
        {
            string base64Value = Convert.ToBase64String(bytes);
            xml.WriteString(base64Value);
        }

        private void AddValueToXml(XmlTextWriter xml, object value,
            bool isDate)
        {
            if (value is ushort[])
            {
                byte[] bytes = ByteConvert.ToBytes((ushort[]) value);
                xml.WriteAttributeString("encoding", "Base64");
                AddBase64ValueToXml(xml, bytes);
            }
            else if (value is byte[])
            {
                xml.WriteAttributeString("encoding", "Base64");
                AddBase64ValueToXml(xml, (byte[]) value);
            }
            else if (isDate)
            {
                // hide zero valued time
                xml.WriteString(((DateTime) value).ToShortDateString());
            }
            else
                xml.WriteString(value.ToString());
        }

        private void AddDataElementToXml(XmlTextWriter xml, 
            DataElement element)
        {
            xml.WriteStartElement("DataElement");
            xml.WriteAttributeString("streamPosition", element.StreamPosition.ToString());
            xml.WriteElementString("Tag", element.Tag.ToString());
            xml.WriteElementString("VR", element.VR.ToString());
            xml.WriteElementString("VM", 
                element.Tag.GetDictionaryEntry().VM.ToString());
            xml.WriteElementString("Description",
                element.Tag.GetDictionaryEntry().Description);
            xml.WriteElementString("ValueLength", element.ValueLength.ToString());
            if (element.Value.IsSequence)
            {
                xml.WriteStartElement("Sequence");
                xml.WriteAttributeString("elementCount", 
                ((Sequence) element.Value[0]).Count.ToString());
                foreach (DataElement d in (Sequence) element.Value[0])
                    AddDataElementToXml(xml, d);
            }
            else if (element.Value.IsNestedDataSet)
            {
                xml.WriteStartElement("NestedDataSet");
                xml.WriteAttributeString("elementCount", 
                    ((NestedDataSet) element.Value[0]).Count.ToString());
                foreach (DataElement d in (NestedDataSet) element.Value[0])
                    AddDataElementToXml(xml, d);
            }
            else if (element.Value.IsMultiValue)
            {
                xml.WriteStartElement("MultiValue");
                xml.WriteAttributeString("count", element.Value.Count.ToString());
                object[] valueArray = element.Value.ToArray();
                for (int i = 0; i < valueArray.Length; i++)
                {
                    xml.WriteStartElement("Value");
                    xml.WriteAttributeString("order", i.ToString());
                    AddValueToXml(xml, valueArray[i], element.Value.IsDate);
                    xml.WriteEndElement();
                }
            }
            else if (element.Value.IsUid)
            {
                Uid uid = (Uid) element.Value[0];
                xml.WriteStartElement("Uid");
                xml.WriteStartElement("Value");
                AddValueToXml(xml, uid, element.Value.IsDate);
                xml.WriteEndElement();
                xml.WriteStartElement("Name");
                AddValueToXml(xml, uid.GetDictionaryEntry().Name, 
                    element.Value.IsDate);
                xml.WriteEndElement();                
                xml.WriteStartElement("Type");
                AddValueToXml(xml, UidType.GetName(typeof(UidType), 
                    uid.GetDictionaryEntry().Type), element.Value.IsDate);
                xml.WriteEndElement();
            }
            else
            {
                xml.WriteStartElement("Value");
                if ( ! element.Value.IsEmpty)
                    AddValueToXml(xml, element.Value[0], element.Value.IsDate);
            }
            // Value
            xml.WriteEndElement();
            // DataElement
            xml.WriteEndElement();
        }

        private void AddDataSetToXml(XmlTextWriter xml, DataSet dataSet)
        {
            foreach (DataElement element in dataSet)
            {
                if (element.Tag.Group.Equals("7FE0"))
                {
                    if ( ! IsPixelDataExcluded)
                        AddDataElementToXml(xml, element);
                }
                else
                    AddDataElementToXml(xml, element);
            }
        }

        private void AddTransferSyntaxToXml(XmlTextWriter xml, 
            TransferSyntax transferSyntax)
        {
            xml.WriteStartElement("TransferSyntax");
            xml.WriteElementString("Uid", transferSyntax.Uid.ToString());
            xml.WriteElementString("Description", 
                transferSyntax.Uid.GetDictionaryEntry().Name);
            xml.WriteEndElement();
        }

        /// <summary>
        ///     Saves <see cref="AcrNemaFile" /> as XML to specified input stream.
        /// </summary>
        public virtual void SaveTo(Stream stream)
        {
            XmlTextWriter xml = new XmlTextWriter(stream, 
                System.Text.Encoding.UTF8);
            xml.Formatting = Formatting.Indented;
            xml.Indentation = 4;
            try
            {
                xml.WriteStartDocument();
                xml.WriteComment(" " + fileComment + " ");
                if (IsDicomFile)
                {
                    xml.WriteStartElement("DicomFile");
                    xml.WriteStartElement("MetaInformation");
                    AddTransferSyntaxToXml(xml,
                        ((DicomFile) AcrNemaFile).MetaInformation.TransferSyntax);
                    xml.WriteElementString("FilePreamble",
                        ((DicomFile) AcrNemaFile).MetaInformation.FilePreamble);
                    AddDataSetToXml(xml, ((DicomFile) AcrNemaFile).MetaInformation);
                    xml.WriteEndElement();
                    xml.WriteStartElement("DataSet");
                    AddTransferSyntaxToXml(xml,
                        AcrNemaFile.DataSet.TransferSyntax);
                    AddDataSetToXml(xml, AcrNemaFile.DataSet);
                    // DataSet
                    xml.WriteEndElement();
                    // DicomFile
                    xml.WriteEndElement();                
                }
                else
                {
                    xml.WriteStartElement("AcrNemaFile");
                    AddTransferSyntaxToXml(xml,
                        AcrNemaFile.DataSet.TransferSyntax);
                    AddDataSetToXml(xml, AcrNemaFile.DataSet);
                    xml.WriteEndElement();
                }
                xml.WriteEndDocument();
                xml.Close();
            }
            finally
            {
                xml.Close();
            }
        }
    }

}