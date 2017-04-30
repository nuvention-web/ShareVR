using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using VRCapture;

namespace ShareVR.Utils
{
	public class S3Uploader : MonoBehaviour
	{
		const string IdentityPoolId = "us-east-1:5b2bd164-6a84-421b-b29f-d400528169aa";
		const string S3BucketName = "sharevr-beta-test-video";
		string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;

		private RegionEndpoint _CognitoIdentityRegion {
			get { return RegionEndpoint.GetBySystemName (CognitoIdentityRegion); }
		}

		string S3Region = RegionEndpoint.USEast1.SystemName;

		private RegionEndpoint _S3Region {
			get { return RegionEndpoint.GetBySystemName (S3Region); }
		}

		//string sharevrPath = VRCaptureUtils.SaveFolder;

		void Start ()
		{
			UnityInitializer.AttachToGameObject (this.gameObject);
		}

		#region private members

		private IAmazonS3 _s3Client;
		private AWSCredentials _credentials;

		private AWSCredentials Credentials {
			get {
				if (_credentials == null)
					_credentials = new CognitoAWSCredentials (IdentityPoolId, _CognitoIdentityRegion);
				return _credentials;
			}
		}

		private IAmazonS3 Client {
			get {
				if (_s3Client == null) {
					_s3Client = new AmazonS3Client (Credentials, _S3Region);
				}
				//test comment
				return _s3Client;
			}
		}

		#endregion

		/// <summary>
		/// Get Object from S3 Bucket
		/// </summary>
		private void GetObject (string objName)
		{
			Debug.Log (string.Format ("fetching {0} from bucket {1}", objName, S3BucketName));
			Client.GetObjectAsync (S3BucketName, objName, (responseObj) => {
				string data = null;
				var response = responseObj.Response;
				if (response.ResponseStream != null) {
					using (StreamReader reader = new StreamReader (response.ResponseStream)) {
						data = reader.ReadToEnd ();
					}

					// ShareVR - will do something here
				}
			});
		}

		/// <summary>
		/// Post Object to S3 Bucket. 
		/// </summary>
		public void PostObject (string objPath, string objName)
		{
			//Debug.Log ("Uploading the file");

			FileStream stream;
			if (File.Exists (objPath + objName))
				stream = new FileStream (objPath + objName, FileMode.Open, FileAccess.Read, FileShare.Read);
			else {
				Debug.LogWarning ("File not found!! " + objPath + objName);
				return;
			}

			var request = new PostObjectRequest () {
				Bucket = S3BucketName,
				Key = objName,
				InputStream = stream,
				CannedACL = S3CannedACL.Private
			};

			Client.PostObjectAsync (request, (responseObj) => {
				if (responseObj.Exception == null) {
					//Debug.Log (string.Format ("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
					Debug.Log ("ShareVR (AWS): Video posted to ShareVR server on AWS! Here's the URL to your video: https://s3.amazonaws.com/sharevr-beta-test-video/" + responseObj.Request.Key);
				} else {
					Debug.LogWarning ("ShareVR (AWS): \nException while posting the result object - " + responseObj.Response.HttpStatusCode.ToString ());
				}
			});
		}

		/// <summary>
		/// Get Objects from S3 Bucket
		/// </summary>
		public void GetObjects ()
		{
			Debug.Log ("Fetching all the Objects from " + S3BucketName);

			var request = new ListObjectsRequest () {
				BucketName = S3BucketName
			};

			Client.ListObjectsAsync (request, (responseObject) => {
				if (responseObject.Exception == null) {
					//ResultText.text += "Got Response \nPrinting now \n";
					responseObject.Response.S3Objects.ForEach ((o) => {
						//ResultText.text += string.Format ("{0}\n", o.Key);
					});
				} else {
					//ResultText.text += "Got Exception \n";
				}
			});
		}

		/// <summary>
		/// Delete Objects in S3 Bucket
		/// </summary>
		public void DeleteObject (string objName)
		{
			//ResultText.text = string.Format ("deleting {0} from bucket {1}", SampleFileName, S3BucketName);
			List<KeyVersion> objects = new List<KeyVersion> ();
			objects.Add (new KeyVersion () {
				Key = objName
			});

			var request = new DeleteObjectsRequest () {
				BucketName = S3BucketName,
				Objects = objects
			};

			Client.DeleteObjectsAsync (request, (responseObj) => {
				//ResultText.text += "\n";
				if (responseObj.Exception == null) {
					//ResultText.text += "Got Response \n \n";

					//ResultText.text += string.Format ("deleted objects \n");

					responseObj.Response.DeletedObjects.ForEach ((dObj) => {
						//ResultText.text += dObj.Key;
					});
				} else {
					//ResultText.text += "Got Exception \n";
				}
			});
		}

		#region helper methods

		private string GetPostPolicy (string bucketName, string key, string contentType)
		{
			bucketName = bucketName.Trim ();

			key = key.Trim ();
			// uploadFileName cannot start with /
			if (!string.IsNullOrEmpty (key) && key [0] == '/') {
				throw new ArgumentException ("uploadFileName cannot start with / ");
			}

			contentType = contentType.Trim ();

			if (string.IsNullOrEmpty (bucketName)) {
				throw new ArgumentException ("bucketName cannot be null or empty. It's required to build post policy");
			}
			if (string.IsNullOrEmpty (key)) {
				throw new ArgumentException ("uploadFileName cannot be null or empty. It's required to build post policy");
			}
			if (string.IsNullOrEmpty (contentType)) {
				throw new ArgumentException ("contentType cannot be null or empty. It's required to build post policy");
			}

			string policyString = null;
			int position = key.LastIndexOf ('/');
			if (position == -1) {
				policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours (24).ToString ("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
				bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
			} else {
				policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours (24).ToString ("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
				bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring (0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
			}

			return policyString;
		}

		#endregion

	}
}