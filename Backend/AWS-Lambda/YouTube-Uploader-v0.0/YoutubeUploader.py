# ShareVR 2017
# AWS Lambda Function
# Purpose: Automatically upload a video to Youtube once video is created
# in S3 bucket

#!/usr/bin/python
# youtube API libraries
from __future__ import print_function
import httplib
import httplib2
import os
import random
import sys
import time

from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from googleapiclient.http import MediaFileUpload
from oauth2client.client import flow_from_clientsecrets
from oauth2client.file import Storage
from oauth2client.tools import argparser, run_flow

# aws libraries
import boto3
import uuid

s3_client = boto3.client('s3')
s3_resource = boto3.resource('s3')

# Explicitly tell the underlying HTTP transport library not to retry, since
# we are handling retry logic ourselves.
httplib2.RETRIES = 1

# Maximum number of times to retry before giving up.
MAX_RETRIES = 10

# Always retry when these exceptions are raised.
RETRIABLE_EXCEPTIONS = (httplib2.HttpLib2Error, IOError, httplib.NotConnected,
  httplib.IncompleteRead, httplib.ImproperConnectionState,
  httplib.CannotSendRequest, httplib.CannotSendHeader,
  httplib.ResponseNotReady, httplib.BadStatusLine)

# Always retry when an apiclient.errors.HttpError with one of these status
# codes is raised.
RETRIABLE_STATUS_CODES = [500, 502, 503, 504]

# The CLIENT_SECRETS_FILE variable specifies the name of a file that contains
# the OAuth 2.0 information for this application, including its client_id and
# client_secret. You can acquire an OAuth 2.0 client ID and client secret from
# the Google Developers Console at
# https://console.developers.google.com/.
# Please ensure that you have enabled the YouTube Data API for your project.
# For more information about using OAuth2 to access the YouTube Data API, see:
#   https://developers.google.com/youtube/v3/guides/authentication
# For more information about the client_secrets.json file format, see:
#   https://developers.google.com/api-client-library/python/guide/aaa_client_secrets
CLIENT_SECRETS_FILE = "client_secrets.json"

# This OAuth 2.0 access scope allows an application to upload files to the
# authenticated user's YouTube channel, but doesn't allow other types of access.
YOUTUBE_UPLOAD_SCOPE = "https://www.googleapis.com/auth/youtube.upload"
YOUTUBE_API_SERVICE_NAME = "youtube"
YOUTUBE_API_VERSION = "v3"

# This variable defines a message to display if the CLIENT_SECRETS_FILE is
# missing.
MISSING_CLIENT_SECRETS_MESSAGE = """
WARNING: Please configure OAuth 2.0

To make this sample run you will need to populate the client_secrets.json file
found at:

   %s

with information from the Developers Console
https://console.developers.google.com/

For more information about the client_secrets.json file format, please visit:
https://developers.google.com/api-client-library/python/guide/aaa_client_secrets
""" % os.path.abspath(os.path.join(os.path.dirname(__file__),
                                   CLIENT_SECRETS_FILE))

VALID_PRIVACY_STATUSES = ("public", "private", "unlisted")


def get_authenticated_service(args):
  flow = flow_from_clientsecrets(CLIENT_SECRETS_FILE,
    scope=YOUTUBE_UPLOAD_SCOPE,
    message=MISSING_CLIENT_SECRETS_MESSAGE)

  storage = Storage("%s-oauth2.json" % os.path.basename(__file__))
  credentials = storage.get()

  if credentials is None or credentials.invalid:
    credentials = run_flow(flow, storage, args)

  return build(YOUTUBE_API_SERVICE_NAME, YOUTUBE_API_VERSION,
    http=credentials.authorize(httplib2.Http()))

def initialize_upload(youtube, options, videoPath):
  tags = None

  # Get tags
  if options["keywords"]:
      tags = options["keywords"].split(",")

  body=dict(
    snippet=dict(
      title=options["title"],
      description=options["description"],
      tags=tags,
      categoryId=options["categoryID"]
    ),
    status=dict(
      privacyStatus=options["privacyStatus"]
    )
  )
  print(body)

  # Call the API's videos.insert method to create and upload the video.
  insert_request = youtube.videos().insert(
    part=",".join(body.keys()),
    body=body,
    # The chunksize parameter specifies the size of each chunk of data, in
    # bytes, that will be uploaded at a time. Set a higher value for
    # reliable connections as fewer chunks lead to faster uploads. Set a lower
    # value for better recovery on less reliable connections.
    #
    # Setting "chunksize" equal to -1 in the code below means that the entire
    # file will be uploaded in a single HTTP request. (If the upload fails,
    # it will still be retried where it left off.) This is usually a best
    # practice, but if you're using Python older than 2.6 or if you're
    # running on App Engine, you should set the chunksize to something like
    # 1024 * 1024 (1 megabyte).
    media_body=MediaFileUpload(videoPath, chunksize=-1, resumable=True)
  )

  resumable_upload(insert_request)

# This method implements an exponential backoff strategy to resume a
# failed upload.
def resumable_upload(insert_request):
  response = None
  error = None
  retry = 0
  while response is None:
    try:
      print ("Uploading file...")
      status, response = insert_request.next_chunk()
      if 'id' in response:
        print ("Video id '%s' was successfully uploaded." % response['id'])
        return
      else:
        exit("The upload failed with an unexpected response: %s" % response)
    except HttpError, e:
      if e.resp.status in RETRIABLE_STATUS_CODES:
        error = "A retriable HTTP error %d occurred:\n%s" % (e.resp.status,
                                                             e.content)
      else:
        raise
    except RETRIABLE_EXCEPTIONS, e:
      error = "A retriable error occurred: %s" % e

    if error is not None:
      print ("%s" % error)
      retry += 1
      if retry > MAX_RETRIES:
        exit("No longer attempting to retry.")

      max_sleep = 1 + retry
      sleep_seconds = random.random() * max_sleep
      print ("Sleeping %f seconds and then retrying..." % sleep_seconds)
      time.sleep(sleep_seconds)

# ShareVR
def parseMetaData(k, metadata):
    # Generate desctiption string
    if "player_name" in metadata.keys():
        desctiptionString = "An awesome VR experience in " + metadata["game_string"] + " featuring " + metadata["player_name"] + " , brought to you by ShareVR (http://sharevr.io/)!"
        titleString = "ShareVR Presents - " + metadata["game_name"] + " Game Experience" + " featuring " + metadata["player_name"]
    else:
        desctiptionString = "An awesome VR experience in " + metadata["game_string"] + " , brought to you by ShareVR (http://sharevr.io/)!"
        titleString = "ShareVR Presents - " + metadata["game_name"] + " Game Experience" + " (#%s)" % metadata["video_id"]

    options = dict(
        title=titleString,
        description=desctiptionString,
        keywords="vr,virtual reality,gaming,"+metadata["game_name"],
        privacyStatus="public",
        categoryID=20 # Gaming
    )

    print("Parsing metadata...")
    print(options)
    return options

def handler(event, context):
    for record in event['Records']:
        bucket = record['s3']['bucket']['name']
        key = record['s3']['object']['key']

        # Get the uploaded video
        # TODO: consider use the url from S3 to directly upload
        videoPath = '/tmp/{}-{}'.format(uuid.uuid4(), key)
        s3_client.download_file(bucket, key, videoPath)

        # Make the video public by default
        print("Received video (%s), making it public..." % videoPath)
        s3_client.put_object_acl(
          ACL='public-read',
          Bucket=bucket,
          Key=key
        )

        # Read video metadata (alt: response = s3.head_object(Bucket=bucket, Key=key), which only read the file head, i.e. metadata)
        # TODO optimize AWS API calls
        #videoObj = s3_resource.Object(bucket, key)
        #metaData = videoObj.metadata
        metaData = s3_client.head_object(Bucket=bucket, Key=key)["Metadata"]
        print("Received video metadata")
        print(metaData)

        options = parseMetaData(key, metaData)

        # TODO: parse more args
        args = argparser.parse_args()
        youtube = get_authenticated_service(args)
        try:
          initialize_upload(youtube, options, videoPath)
          return
        except HttpError, e:
          print ("An HTTP error %d occured:\n%s" % (e.resp.status, e.content))
