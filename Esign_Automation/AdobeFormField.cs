using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins.Esign_Automation
{
   public class FileInfo
   {
      public string transientDocumentId { get; set; }

      public FileInfo(String transDocId)
      {
         this.transientDocumentId = transDocId;
      }
   }

   public class RecipientSetMemberInfo
   {
      public string email { get; set; }

      public RecipientSetMemberInfo(String receiverMail)
      {
         this.email = receiverMail;
      }
   }

   public class RecipientSetInfo
   {
      public string recipientSetRole { get; set; }
      public List<RecipientSetMemberInfo> recipientSetMemberInfos { get; set; }

      public RecipientSetInfo(String receiverMail)
      {
         this.recipientSetRole = "SIGNER";
         this.recipientSetMemberInfos = new List<RecipientSetMemberInfo>();
         this.recipientSetMemberInfos.Add(new RecipientSetMemberInfo(receiverMail));

      }
   }

   public class Location
   {
      public string pageNumber { get; set; }
      public string top { get; set; }
      public string left { get; set; }
      public string width { get; set; }
      public string height { get; set; }

      public Location(String pageNumber, String top, String left)
      {
         this.pageNumber = pageNumber;

         this.top = top;
         this.left = left;
         this.width = "175";
         this.height = "18";
      }
   }

   public class FormField
   {
      public string name { get; set; }
      public string inputType { get; set; }
      public string contentType { get; set; }
      public string fontSize { get; set; }
      public string required { get; set; }
      public string readOnly { get; set; }
      public string defaultValue { get; set; }
      public string recipientIndex { get; set; }
      public List<Location> locations { get; set; }

      public FormField(String pageNumber, String name, String inputType, String contentType, String index, String defaultValue, 
         String top, String left)
      {
         locations = new List<Location>();
         locations.Add(new Location(pageNumber, top, left));

         this.name = name;
         this.inputType = inputType;
         this.contentType = contentType;
         this.fontSize = "8";
         this.required = "true";
         this.recipientIndex = index;
         if (contentType.Equals("TEXT_FIELD"))
         {
            this.defaultValue = defaultValue;
         }
      }
   }

   public class DocumentCreationInfo
   {
      public string name { get; set; }
      public string state { get; set; }
      public string signatureType { get; set; }
      public string message { get; set; }
      public string signatureFlow { get; set; }
      public List<FileInfo> fileInfos { get; set; }
      public List<RecipientSetInfo> recipientSetInfos { get; set; }
      public List<FormField> formFields { get; set; }
      FileInfo newFile;
      RecipientSetInfo newRecipent;
      //FormField newFields;
      public DocumentCreationInfo(String documentId, String receiverMail, String message, String pageNumber)
      {
         this.name = "Metrix Testing";
         this.state = "AUTHORING";
         this.signatureType = "ESIGN";
         this.message = message;
         this.signatureFlow = "SENDER_SIGNATURE_NOT_REQUIRED";
         this.fileInfos = new List<FileInfo>();
         newFile = new FileInfo(documentId);
         this.fileInfos.Add(newFile);

         this.recipientSetInfos = new List<RecipientSetInfo>();
         newRecipent = new RecipientSetInfo(receiverMail);
         this.recipientSetInfos.Add(newRecipent);

         this.formFields = new List<FormField>();

         this.formFields.Add(new FormField(pageNumber,"Signature", "SIGNATURE", "SIGNATURE","1","Signature","400", "135"));
         this.formFields.Add(new FormField(pageNumber, "Email", "TEXT_FIELD", "TEXT_FIELD", "1", "Email","380", "135"));
         this.formFields.Add(new FormField(pageNumber, "Company", "TEXT_FIELD", "TEXT_FIELD", "1","Company","360", "135"));
      }
   }

   public class AdobeFormField
   {
      public DocumentCreationInfo documentCreationInfo { get; set; }

      public AdobeFormField(String documentId, String receiverMail, String message, String pageNumber)
      {
         this.documentCreationInfo = new DocumentCreationInfo(documentId, receiverMail, message, pageNumber);
      }
   }
}
