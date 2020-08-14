using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using RestSharp;
using Newtonsoft.Json;

namespace MetrixGroupPlugins.Esign_Automation
{
   /**
    * @Author - Wilfred (12/09/18)
    * This class contains the method to communicate with the Adobe Sign Rest API.
    * This class is responsible for sending the final PDF with the agreement to the client 
    **/

   public class adobeSignConnector
   {    
      private String clientId = "CBJCHBCAABAAiPCuUl7QatefD7K9zfT_xzLAfvMzw4uu";
      private String clientSecret = "Bl5GsUGBJ_14CXSEmV1R6jdlG5LPcOvQ";
      private String redirectURI = "https://www.google.com";
      private String code = "CBNCKBAAHBCAABAAJRibrMkICSYzDWRcNpIN-d1qV2pI5NXW";
      private String refreshTokenCode = "3AAABLblqZhC2FutAVI6fiqDVssoVUpI5Qh8LZMuCDQJtzPMJ_7wf8dhZTFVeS3Ibn_5Ut1Kei9E*";
      private String accessToken = "3AAABLblqZhDcarlfcZTy3NloKEYxq2NsmBhLxWypo8W3LGP0nBquhI14cEB5DLQDp_9BkSVf0FVCvCDmva8D_ixMDyjsHiMP";
      private String transientDocId;

      //This constructor executes the required methods in a sequential manner
      public adobeSignConnector(String fileLocation, String fileName, String recipientMail, String message, String pageNumber)
      {
         //Request for new Access Token
         accessToken = refreshToken(); //get the response as JSON
         AccessTokenModel accessTokenModel = JsonConvert.DeserializeObject<AccessTokenModel>(accessToken); //Deserialize the JSON response
         accessToken = accessTokenModel.access_token; //assign the access token to the access token variable

         transientDocId = uploadDocument(fileLocation, fileName); //Upload document 
         TransientDocumentModel transientDocumentModel = JsonConvert.DeserializeObject<TransientDocumentModel>(transientDocId); //Deserialize the JSON response
         transientDocId = transientDocumentModel.transientDocumentId; // Asssign the document id to the variable

         //Call the method and pass the parameters to place custom form fields on the uploaded document 
         //and send to the receiver
         placeFormFields_Send(accessToken, transientDocId, recipientMail, message, pageNumber);


      }

      // Method will request the rest service to get a new access token and a refresh token 
      //Execute this method only if there is a problem with the current refresh token
      public string getAccessToken()
      {
         string strResponse = string.Empty;
         //Enable security protocols 
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

         var client = new RestClient("https://api.au1.echosign.com/oauth/token");
         var request = new RestRequest(Method.POST);
         request.AddHeader("content-type", "application/x-www-form-urlencoded");

         request.AddParameter("grant_type", "authorization_code");
         request.AddParameter("client_id", clientId);
         request.AddParameter("client_secret", clientSecret);
         request.AddParameter("redirect_uri", redirectURI);
         request.AddParameter("code", code);

         IRestResponse response = client.Execute(request); //request for a new access with a new refresh token
         return response.Content;
      }

      //Method will request the API using the refresh access token and get a new access token
      public String refreshToken()
      {
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

         var client = new RestClient("https://api.documents.adobe.com/oauth/refresh?");
         var request = new RestRequest(Method.POST);

         request.AddHeader("content-type", "application/x-www-form-urlencoded");
         request.AddParameter("refresh_token", refreshTokenCode); //set the refresh token
         request.AddParameter("grant_type", "refresh_token");
         request.AddParameter("client_id", clientId);
         request.AddParameter("client_secret", clientSecret);
         request.RequestFormat = DataFormat.Json;

         IRestResponse response = client.Execute(request); //request for a new access token using the refresh token
         return response.Content;
      }

      //Method will Upload a document and return a document ID
      //Use this method to upload a document to the cloud.
      public String uploadDocument(String fileLocation, String fileName)
      {
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

         var client = new RestClient("https://api.au1.echosign.com:443/api/rest/v6/transientDocuments");
         var request = new RestRequest(Method.POST);
         request.AddHeader("content-type", "multipart/form-data");
         request.AddHeader("content-disposition", "multipart/form-data");
         request.AddFile("File", File.ReadAllBytes(fileLocation), fileName + ".pdf"); //Read the file from the provided file location
         request.AddHeader("Access-Token", accessToken);
         request.AddHeader("Authorization", "Bearer " + accessToken); //Bearer is required 
         request.RequestFormat = DataFormat.Json;

         IRestResponse objResponse = client.Execute(request); //Post to the server and upload the file
         return objResponse.Content; //get the upload file document Id

      }


      //This method will send a document to an email address with custom placed form fields
      public void placeFormFields_Send(String accessToken, String documentId, String receiverMail, String message, String pageNumber)
      {
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

         //Instantiate the object with the provided parameters
         //this is done to generate the JSON format required to send the document
         AdobeFormField adobeFormField = new AdobeFormField(documentId, receiverMail, message, pageNumber);

         var client = new RestClient("https://api.au1.echosign.com/api/rest/v5");
         var request = new RestRequest("agreements", Method.POST);
         request.AddHeader("Access-Token", accessToken);
         request.RequestFormat = DataFormat.Json; //set the format of the request
         request.AddBody(adobeFormField); //add the instantiated object (will be converted to JSON)

         IRestResponse response = client.Execute(request); //Execute the request
         var content = response.Content; //get the response
      }

      //This is the main method of the adobe sign connector class and it is responsible for initializing a class object
      public static void Main(String fileLocation, String fileName, String recipientMail, String message, String pageNumber)
      {
         adobeSignConnector sign = new adobeSignConnector(fileLocation, fileName, recipientMail, message, pageNumber);

      }
   }
}
