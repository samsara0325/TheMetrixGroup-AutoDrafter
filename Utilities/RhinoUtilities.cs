using System;
using Rhino;
using System.Drawing;
using Rhino.DocObjects;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MetrixGroupPlugins.MessageBoxes;
using System.Threading;

namespace MetrixGroupPlugins
{
   public static class RhinoUtilities
   {
      public static bool SetActiveLayer(string name, Color color)
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;

         // Does a layer with the same name already exist?
         int layerIndex = doc.Layers.Find(name, true);

         // If layer does not exist
         if (layerIndex == -1)
         {
            // Add a new layer to the document
            layerIndex = doc.Layers.Add(name, color);
         }

         // Set the layer to perforation
         return doc.Layers.SetCurrentLayerIndex(layerIndex, true);
      }

      /**
       * Method used to set the visibility of a layer.
       * @param layerName - name of the layer that needs visibility changed
       * @param visibility - type boolean, the required visibility status of the layer
       * */
      public static void setLayerVisibility(String layerName, Boolean visiblity)
      {
        
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            int currentLayerIndex = doc.Views.ActiveView.Document.Layers.CurrentLayerIndex;
            int index = doc.Views.ActiveView.Document.Layers.Find(layerName, true);
            Layer newLayerSettings = doc.Layers[index];
            newLayerSettings = doc.Layers[index];

            newLayerSettings.IsVisible = visiblity;
            newLayerSettings.SetPersistentVisibility(visiblity);

            doc.Views.ActiveView.Document.Layers.Modify(newLayerSettings, index, true);

            doc.Views.ActiveView.Document.Layers[index].CommitChanges();
            doc.Views.ActiveView.Document.Layers.SetCurrentLayerIndex(currentLayerIndex, true);
         
      }
      //Method used to merge 2 pdfs together
      public static void combinePDF(string targetPath, string[] pdfs, int pdfWithWaterMark, int pdfWithOutWaterMark, String pdfPrintOrder)
      {
         Boolean exceptionThrown = true;
         Boolean addLogoException = true;
         PdfSharp.Pdf.PdfDocument one = null;
         PdfSharp.Pdf.PdfDocument two = null;
            //Messages.showGeneratingPDF();



            int counter = 0;
         while (addLogoException) //try to add the watermark
         {
            try
            {
               AddLogo(pdfs[pdfWithWaterMark], pdfs[pdfWithWaterMark]);  //pass to create water mark on the panel pdf 
               addLogoException = false;
            }
            catch (Exception e)
            {
               addLogoException = true;
               counter += 1;
               if(counter >= 10)
                    {
                        addLogoException = false;
                        System.Windows.Forms.MessageBox.Show("Error adding watermark: " + e.Message);
                    }
               
            }
         }
         while (exceptionThrown) //try to import the documents
         {
            try
            {
               one = PdfSharp.Pdf.IO.PdfReader.Open(pdfs[pdfWithWaterMark], PdfDocumentOpenMode.Import);
               two = PdfSharp.Pdf.IO.PdfReader.Open(pdfs[pdfWithOutWaterMark], PdfDocumentOpenMode.Import);
               exceptionThrown = false;
            }
            catch (Exception e)
            {
                    exceptionThrown = false;
               System.Windows.Forms.MessageBox.Show("Error printing PDF document." + e.Message);

                }
            }
         PdfSharp.Pdf.PdfDocument outPdf = new PdfSharp.Pdf.PdfDocument();
         if(pdfPrintOrder.Equals("Watermark Only")) //If the user only wants to add the water mark only rather than combining 2 PDFS
            {
            CopyPages(one, outPdf);
            }
         if(pdfPrintOrder.Equals("Drawings First")) // If the user wants to draw the drawing first and then the agreement page or tool hit page
         {
            CopyPages(one, outPdf); //copy the content of pdf one in to new pdf 
            CopyPages(two, outPdf); //copy the content of pdf two in to the new pdf (merging)
         }
         if (pdfPrintOrder.Equals("Drawings Second")) // If the user wants to draw the drawings second after the agreement page or tool hit page
         {
            CopyPages(two, outPdf); //copy the content of pdf two in to the new pdf 
            CopyPages(one, outPdf); //copy the content of pdf one in to new pdf (merging)
         }
         File.Delete(pdfs[pdfWithWaterMark]); //delete pdf one        

         /*if (Messages.showCompressionRequired())
         {
            //File.Delete(targetPath); //delete the original file 
            CompressMyPdf(outPdf, targetPath);
           
           // outPdf.Save(targetPath);
            RhinoApp.WriteLine("Compression Successful. Approval Drawing Completed");
         }
         else
         {*/
            File.Delete(targetPath); //delete the original file 
            outPdf.Save(targetPath);
            RhinoApp.WriteLine("PDF Successfully Saved. Approval Drawing Completed");
       //  }

         //Method combines the 2 pdf by copying all pages to a new pdf file
         void CopyPages(PdfSharp.Pdf.PdfDocument from, PdfSharp.Pdf.PdfDocument to) //inner method 
         {
            for (int i = 0; i < from.PageCount; i++)
            {
               to.AddPage(from.Pages[i]);
            }
         }
      }


      //Below code is used to add the watermark on to pdfs
      // the image you use as a watermark 

      private static void AddLogo(String pdfLocation, String savePdf)
      {
         try
         {
            //get the image (watermark) from the location
            iTextSharp.text.Image pageIn = iTextSharp.text.Image.GetInstance("Z:\\Automation\\RequiredDraftingDocuments\\watermark.png");
            //set the settings of the watermark image (opacity, alignment, etc)
            pageIn.Alignment = iTextSharp.text.Image.ALIGN_MIDDLE;
            pageIn.ScaleToFit(300, 300);
            pageIn.Alignment = iTextSharp.text.Image.UNDERLYING;
            pageIn.SetAbsolutePosition(150, 300);
            pageIn.RotationDegrees = 30; // rotates the image 30 degrees as required
            //read the exisitng pdf to add watermark
            System.Threading.Thread.Sleep(4000);
            iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(pdfLocation);
            string FileLocation = savePdf; //place where new pdf will be saved 
            

            //Create a temporary pdf 
            PdfStamper stamp = new PdfStamper(pdfReader, new FileStream(FileLocation.Replace(".pdf", "[temp].pdf"), FileMode.Create));
            //create new graphics state and assign opacity    
            PdfGState graphicsState = new PdfGState();
            graphicsState.FillOpacity = 0.2f;  //set the opacity of watermark 
            graphicsState.StrokeOpacity = 0.2f;
            PdfContentByte waterPDF;
            for (int page = 1; page <= pdfReader.NumberOfPages; page++) //for each page in the pdf add the water mark
            {              
               waterPDF = stamp.GetOverContent(page);
               waterPDF.SetGState(graphicsState);
               waterPDF.AddImage(pageIn);
            }
            stamp.FormFlattening = true;
            stamp.Close();
            pdfReader.Close();
            // now delete the original file and rename the temp file to the original file 
            File.Delete(FileLocation); //delete the original file 
            File.Move(FileLocation.Replace(".pdf", "[temp].pdf"), FileLocation); //rename the temporary created pdf with the original pdf file name
         }
         catch (Exception ex)
         {
            throw new Exception("Error found :" + ex.GetBaseException() + " \n" + ex);
         }
      }

      //Method deletes a layer and all of its objects
      public static void deleteLayer(String layerName)
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         Rhino.DocObjects.RhinoObject[] rhobjs = doc.Objects.FindByLayer(layerName); //get all the objects that belong to the layer
         int layerIndex = doc.Layers.Find(layerName, true); //get the index of the supplied layer
         if (rhobjs != null)
         {
            if (rhobjs.Length > 0)
            {
               for (int i = 0; i < rhobjs.Length; i++)
                  doc.Objects.Delete(rhobjs[i], true); //delete objects
            }
         }

         doc.Layers.Delete(layerIndex, true); //delete the layer name
      }


      ///<summary>This function set the compression parameters of PDF document</summary>
      ///<param name="pdfDoc"></param>
      ///<author>Anubhav Passi, Date - 2/3/2018</author>
    public static void CompressMyPdf(PdfSharp.Pdf.PdfDocument pdfDoc, String directory)
      {

         try
         {
            // Code for PDF Compression
            pdfDoc.Options.NoCompression = false;
            pdfDoc.Options.FlateEncodeMode = PdfFlateEncodeMode.BestCompression;
            pdfDoc.Options.UseFlateDecoderForJpegImages = PdfUseFlateDecoderForJpegImages.Automatic;
            pdfDoc.Options.EnableCcittCompressionForBilevelImages = true;
            pdfDoc.Options.CompressContentStreams = true;
            pdfDoc.Save(directory);
            
         }
         catch (Exception e)
         {
          
         }

      }

      ///<summary>This method returns the page count in a pdf</summary>
      ///<param name="fileLocation"> Location of the PDF </param>
      ///<author>Wilfred, Date - 12/09/2018</author>
      public static int calculatePageNumbers(String fileLocation)
      {
         PdfSharp.Pdf.PdfDocument importedPDF = PdfSharp.Pdf.IO.PdfReader.Open(fileLocation, PdfDocumentOpenMode.Import);
         return importedPDF.PageCount;
      }

   }
}

