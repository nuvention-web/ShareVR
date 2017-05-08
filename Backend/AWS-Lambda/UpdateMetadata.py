from __future__ import print_function

import json
import urllib
import boto3
import os

print('Loading function')

s3 = boto3.client('s3')

def uploadMetadata(bucket, key, response):
    metadataFile = bucket + "-metadata.txt" 
    download_path = '/tmp/{}'.format(metadataFile)
    s3.download_file(bucket, metadataFile, download_path)
    
    # deletes file in /tmp folder if there exists one
    try:
        os.remove(download_path)
    except OSError:
        pass
    
    # writes metadata into a txt file and uploads to bucket
    with open(download_path, "a") as file:
        file.write("\n")
        file.write(response['Metadata']['client_id'] + "\t")
        file.write(response['Metadata']['game_name'] + "\t")
        file.write(response['Metadata']['game_string'] + "\t")
        file.write(response['Metadata']['player_name'] + "\t")
        file.write(response['Metadata']['sdk_version'] + "\t")
        file.write(response['Metadata']['unity_version'] + "\t")
        file.write(response['Metadata']['video_id'] + "\t")
        file.close()
    s3.upload_file(download_path, bucket, metadataFile)
    print('Metadafile updated')

def lambda_handler(event, context):

    # Get the object from the event and show its content type
    bucket = event['Records'][0]['s3']['bucket']['name']
    key = urllib.unquote_plus(event['Records'][0]['s3']['object']['key'].encode('utf8'))
    
    response = s3.get_object(Bucket=bucket, Key=key)
    uploadMetadata(bucket, key, response)
